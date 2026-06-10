using AutoMapper;
using Net.Business.Entities.Web;
namespace Net.Data.Mapper
{
    public class SecurityMapperProfile : Profile
    {
        public SecurityMapperProfile()
        {
            CreateMap<UsuarioEntity, UsuarioCreateEntity>().ReverseMap();
            CreateMap<UsuarioCreateEntity, UsuarioEntity>().ReverseMap();

            CreateMap<LogisticUserEntity, LogisticUserCreateEntity>().ReverseMap();
            CreateMap<LogisticUserCreateEntity, LogisticUserEntity>().ReverseMap();

            CreateMap<LogisticUserPermissionEntity, LogisticUserPermissionCreateEntity>().ReverseMap();
            CreateMap<LogisticUserPermissionCreateEntity, LogisticUserPermissionEntity>().ReverseMap();
        }
    }
}
