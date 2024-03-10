namespace MinimalAPI.Infra.DTO;

public class ProductDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
}
