public sealed record OrderSummary(int Id, string Status, decimal Total);

public interface IOrderQueries
{
    /// <summary>Returns the order with this id, or null when there is none.</summary>
    OrderSummary? FindById(int id);
}
