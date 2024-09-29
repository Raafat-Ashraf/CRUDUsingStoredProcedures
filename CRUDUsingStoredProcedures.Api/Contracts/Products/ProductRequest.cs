namespace CRUDUsingStoredProcedures.Api.Contracts.Products;

public class ProductRequest
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}
