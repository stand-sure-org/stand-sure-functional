using System;
using System.Collections.Generic;

public sealed record OrderLine(string Sku, int Quantity, decimal UnitPrice);

public sealed record CustomerOrder(Guid Id, string Region, IReadOnlyList<OrderLine> Lines);

public sealed record Invoice(Guid OrderId, decimal Net, decimal Tax, decimal Gross);

public interface IOrderSource
{
    /// <summary>Returns the order with this id, or null when there is none.</summary>
    CustomerOrder? Get(Guid orderId);
}

public interface ITaxRateTable
{
    /// <summary>Returns the tax rate for the region (0.20 means 20%), or null when the region is unknown.</summary>
    decimal? FindRate(string region);
}
