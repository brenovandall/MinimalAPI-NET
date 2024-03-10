using MinimalAPI.Infra.Models;

namespace MinimalAPI.Infra.Data;

public class ProductStorage
{
    public static List<ProductDTO> productsList = new List<ProductDTO>
    {
        new ProductDTO { Id = 1, Name = "Banana", Price = 1.99M, IsActive = true }, 
        new ProductDTO { Id = 2, Name = "Apple", Price = 0.99M, IsActive = false }
    };
}
