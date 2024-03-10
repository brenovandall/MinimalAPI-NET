using FluentValidation;
using MinimalAPI.Infra.DTO;

namespace MinimalAPI.Infra.Validations;

public class ProductCreateValidation : AbstractValidator<ProductCreateDTO>
{
    public ProductCreateValidation()
    {
        RuleFor(model => model.Name).NotEmpty();
        RuleFor(model => model.Price).NotEmpty().InclusiveBetween(0, 1000);
    }
}
