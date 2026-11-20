using System;

public sealed record PaymentReceipt(string TransactionId, decimal Amount, string Currency);

/// <summary>Thrown by the gateway when the card issuer declines the charge.</summary>
public sealed class PaymentDeclinedException(string reason) : Exception($"Payment declined: {reason}")
{
    /// <summary>The issuer's decline reason, e.g. "insufficient_funds".</summary>
    public string Reason { get; } = reason;
}

/// <summary>Thrown by the gateway when its service cannot be reached.</summary>
public sealed class GatewayUnavailableException(string message) : Exception(message);

/// <summary>Vendor payment SDK. Cannot be changed.</summary>
public interface IPaymentGateway
{
    /// <summary>Charges the card and returns the gateway's transaction id.</summary>
    /// <exception cref="PaymentDeclinedException">The card issuer declined the charge.</exception>
    /// <exception cref="GatewayUnavailableException">The gateway could not be reached.</exception>
    /// <exception cref="TimeoutException">The gateway did not answer in time.</exception>
    string Charge(string cardToken, decimal amount, string currency);
}
