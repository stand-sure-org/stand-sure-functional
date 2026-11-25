using System;
using System.Diagnostics;

namespace StandSure.Functional;

// The one place where void becomes Unit. Each adapter is a delegate bound to the caller's delegate (a static method
// closed over its first argument), so adapting costs one delegate and no closure object. A missing delegate stays
// missing, so the target method's null-delegate rules still apply and name the parameter the caller wrote.
[StackTraceHidden]
internal static class UnitAdapters
{
    internal static Func<T, Unit> ToFunc<T>(Action<T> action) =>
        ResultErrors.IsMissing(action) ? null! : new Func<T, Unit>(action.InvokeReturningUnit);

    internal static Func<Unit> ToFunc(Action action) =>
        ResultErrors.IsMissing(action) ? null! : new Func<Unit>(action.InvokeReturningUnit);

    internal static Func<Unit, Result<TOut>> IgnoringUnit<TOut>(Func<Result<TOut>> next) =>
        ResultErrors.IsMissing(next) ? null! : new Func<Unit, Result<TOut>>(next.InvokeIgnoringUnit);

    private static Unit InvokeReturningUnit<T>(this Action<T> action, T value)
    {
        action(value);
        return Unit.Value;
    }

    private static Unit InvokeReturningUnit(this Action action)
    {
        action();
        return Unit.Value;
    }

    private static Result<TOut> InvokeIgnoringUnit<TOut>(this Func<Result<TOut>> next, Unit unit) => next();
}
