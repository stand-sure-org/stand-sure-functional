namespace ProbeRunner;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading.Tasks;

/// <summary>Loads the compiled solution and invokes its entry method once with the probe's stub arguments.</summary>
/// <remarks>
/// Failures of the runner itself (load, lookup, binding) are <c>crashed</c> with a reason. Only exceptions
/// raised by model code (constructor or entry method, synchronously or from an awaited result) are <c>threw</c>.
/// </remarks>
internal static class ProbeInvoker
{
    private const string ProbeTypeName = "HarnessProbe";

    public static async Task<ProbeOutput> RunAsync(string assemblyPath, string entryType, string entryMethod)
    {
        Assembly assembly;

        try
        {
            var context = new AssemblyLoadContext("probe", isCollectible: true);
            assembly = context.LoadFromAssemblyPath(assemblyPath);
        }
        catch (Exception ex)
        {
            return ProbeOutput.Fail("load-failed: " + ex.GetType().FullName);
        }

        var types = LoadableTypes(assembly);
        var solutions = types.Where(t => t.IsPublic && t.IsClass && t.Name == entryType).ToList();

        if (solutions.Count != 1)
        {
            return ProbeOutput.Fail(solutions.Count == 0 ? "entry-type-not-found" : "entry-type-ambiguous");
        }

        var probe = assembly.GetType(ProbeTypeName);

        if (probe is null)
        {
            return ProbeOutput.Fail("probe-type-not-found");
        }

        object?[] ctorArguments;
        object?[] arguments;

        try
        {
            ctorArguments = (object?[]?)probe.GetMethod("ConstructorArguments")?.Invoke(null, null) ?? throw new InvalidOperationException();
            arguments = (object?[]?)probe.GetMethod("Arguments")?.Invoke(null, null) ?? throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            return ProbeOutput.Fail("probe-arguments-failed: " + ex.GetType().FullName);
        }

        var solutionType = solutions[0];

        var candidates = solutionType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name == entryMethod && !m.ContainsGenericParameters && m.GetParameters().Length == arguments.Length)
            .ToList();

        if (candidates.Count != 1)
        {
            return ProbeOutput.Fail(candidates.Count == 0 ? "entry-method-not-found" : "entry-method-ambiguous");
        }

        object solution;

        try
        {
            solution = Activator.CreateInstance(solutionType, ctorArguments)
                       ?? throw new InvalidOperationException();
        }
        catch (TargetInvocationException ex)
        {
            return Threw(ex.InnerException ?? ex);
        }
        catch (Exception ex)
        {
            return ProbeOutput.Fail("constructor-binding-failed: " + ex.GetType().FullName);
        }

        object? returned;

        try
        {
            returned = candidates[0].Invoke(solution, arguments);
        }
        catch (TargetInvocationException ex)
        {
            return Threw(ex.InnerException ?? ex);
        }
        catch (Exception ex)
        {
            return ProbeOutput.Fail("entry-method-binding-failed: " + ex.GetType().FullName);
        }

        try
        {
            await AwaitIfAwaitable(returned).ConfigureAwait(false);
        }
        catch (TargetInvocationException ex)
        {
            return Threw(ex.InnerException ?? ex);
        }
        catch (Exception ex)
        {
            return Threw(ex);
        }

        return new ProbeOutput(ProbeOutput.Returned);
    }

    private static ProbeOutput Threw(Exception exception) =>
        new(ProbeOutput.Threw, ExceptionType: exception.GetType().FullName);

    private static IReadOnlyList<Type> LoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return [.. ex.Types.OfType<Type>()];
        }
    }

    /// <summary>Awaits <c>Task</c>, <c>Task&lt;T&gt;</c>, <c>ValueTask</c>, <c>ValueTask&lt;T&gt;</c>, or any value with a <c>GetAwaiter()</c> pattern.</summary>
    private static Task AwaitIfAwaitable(object? value)
    {
        switch (value)
        {
            case null:
                return Task.CompletedTask;
            case Task task:
                return task;
            case ValueTask valueTask:
                return valueTask.AsTask();
        }

        var type = value.GetType();

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            return (Task)type.GetMethod(nameof(ValueTask<int>.AsTask))!.Invoke(value, null)!;
        }

        var getAwaiter = type.GetMethod("GetAwaiter", BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes);

        if (getAwaiter is null)
        {
            return Task.CompletedTask;
        }

        return AwaitPattern(getAwaiter.Invoke(value, null));
    }

    private static async Task AwaitPattern(object? awaiter)
    {
        if (awaiter is not INotifyCompletion notify)
        {
            return;
        }

        var awaiterType = awaiter.GetType();
        var isCompleted = awaiterType.GetProperty("IsCompleted");
        var getResult = awaiterType.GetMethod("GetResult", Type.EmptyTypes);

        if (isCompleted is null || getResult is null)
        {
            return;
        }

        if (!(bool)isCompleted.GetValue(awaiter)!)
        {
            var completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            notify.OnCompleted(completed.SetResult);
            await completed.Task.ConfigureAwait(false);
        }

        getResult.Invoke(awaiter, null);
    }
}
