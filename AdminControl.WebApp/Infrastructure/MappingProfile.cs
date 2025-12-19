using AdminControl.DALEF.Models;
using AdminControl.DTO;
using AutoMapper;

namespace AdminControl.WebApp.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<RoleCreateDto, Role>();
            CreateMap<RoleUpdateDto, Role>();

        }
    }
}