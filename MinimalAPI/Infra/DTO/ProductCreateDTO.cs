namespace MinimalAPI.Infra.DTO;

public class ProductCreateDTO
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastUpdate { get; set; }
}
