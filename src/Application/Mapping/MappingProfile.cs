using AutoMapper;
using Application.DTOs;
using Domain.Entities;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();

            CreateMap<Item, ItemDto>();
        }
    }
}