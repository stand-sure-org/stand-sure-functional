public sealed record Product(int Id, string Name, decimal Price);

public interface IProductRepository
{
    /// <summary>Returns the product with this id, or null when there is none.</summary>
    Product? FindById(int id);
}
