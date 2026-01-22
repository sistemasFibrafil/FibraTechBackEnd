using AutoMapper;
using Net.Business.Entities.Web;
namespace Net.Data.AppContext
{
    public class AutoMapperProfileSeg : Profile
    {
        public AutoMapperProfileSeg()
        {
            CreateMap<LogisticUserEntity, LogisticUserCreateEntity>().ReverseMap();
            CreateMap<LogisticUserEntity, LogisticUserEntity>().ReverseMap();

            CreateMap<LogisticUserPermissionEntity, LogisticUserPermissionCreateEntity>().ReverseMap();
            CreateMap<LogisticUserPermissionCreateEntity, LogisticUserPermissionEntity>().ReverseMap();
        }
    }
}
