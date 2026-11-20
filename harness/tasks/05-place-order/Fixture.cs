using System;

public sealed record Customer(Guid Id, string Name, decimal CreditLimit);

public sealed record Order(Guid Id, Guid CustomerId, string Sku, int Quantity, decimal Total);

public interface ICustomerDirectory
{
    /// <summary>Returns the customer with this customer number, or null when there is none.</summary>
    Customer? Find(string customerNumber);
}

public interface ICatalog
{
    /// <summary>Returns the unit price of the SKU, or null when the SKU is unknown.</summary>
    decimal? GetUnitPrice(string sku);
}

public interface IOrderBook
{
    /// <summary>Records the order and returns it with its generated id.</summary>
    Order Place(Guid customerId, string sku, int quantity, decimal total);
}
