using AutoMapper;
using MinimalAPI.Infra.DTO;
using MinimalAPI.Infra.Models;

namespace MinimalAPI.Infra.Helper;

public class MapProfiles : Profile
{
    public MapProfiles()
    {
        CreateMap<ProductCreateDTO, ProductDTO>();
        CreateMap<ProductDTO, ProductDto>();
    }
}
