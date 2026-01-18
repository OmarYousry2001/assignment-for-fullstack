using AutoMapper;
using BL.DTO.Entities;
using EcommerceAPI.Domain.Entities;

namespace BL.Mapper
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {

            #region Product   
            CreateMap<Product, ProductDTO>().ReverseMap();
            #endregion


        }
    }
}
