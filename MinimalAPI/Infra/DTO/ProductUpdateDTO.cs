namespace MinimalAPI.Infra.DTO;

public class ProductUpdateDTO
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
