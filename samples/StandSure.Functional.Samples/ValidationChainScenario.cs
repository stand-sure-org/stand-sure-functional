namespace StandSure.Functional.Samples;

/// <summary>Scenario 2: a synchronous validation chain where one check fails, plus Combine over several orders.</summary>
internal static class ValidationChainScenario
{
    private static readonly Order[] Orders =
    [
        new("o-1", "USD", 10m, "a@example.test"),
        new("o-2", "USD", -5m, "b@example.test"),
        new("o-3", "USD", 7m, null),
    ];

    public static Result<IReadOnlyList<Order>> Run()
    {
        using var activity = SampleTelemetry.Source.StartActivity();

        var result = Orders.Traverse(Validate);
        SampleTelemetry.RecordOutcome(activity, result);
        return result;
    }

    private static Result<Order> Validate(Order order) =>
        Result.Success(order)
            .Ensure(o => o.Amount > 0, o => Error.Validation("order.amount_not_positive", $"Order {o.Id} has amount {o.Amount}."))
            .Ensure(o => o.CustomerEmail is not null, o => Error.Validation("order.email_missing", $"Order {o.Id} has no email."))
            .Map(o => o with { Amount = decimal.Round(o.Amount, 2) });
}
