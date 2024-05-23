using api.Dtos;
using AutoMapper;

namespace api.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Product, ProductModel>();
            CreateMap<Category,CategoryModel>();
        }
    }
}