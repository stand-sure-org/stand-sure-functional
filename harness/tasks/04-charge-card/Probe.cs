public sealed class ProbePaymentGateway : IPaymentGateway
{
    public string Charge(string cardToken, decimal amount, string currency) =>
        throw new PaymentDeclinedException("insufficient_funds");
}

/// <summary>Injected failure: the dependency throws (the card is declined).</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbePaymentGateway()];

    public static object?[] Arguments() => ["tok_visa_4242", 25.00m, "GBP"];
}
