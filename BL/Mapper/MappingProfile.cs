using AutoMapper;
using BL.DTO.Entities;
using Domains.Entities;
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

            #region Project 

            //CreateMap<Project, GetProjectDTO>()
            //.ForMember(des => des.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            //.ForMember(des => des.Images, opt => opt.MapFrom(src => src.Images))
            //.ForMember(des => des.Id, opt => opt.MapFrom(src => src.Id)).ReverseMap();

            //CreateMap<ProjectDTO, Project>().ReverseMap();

            #endregion


        }
    }
}
