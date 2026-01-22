using AutoMapper;
using Net.Business.Entities.Sap;
namespace Net.Data.AppContext
{
    public class AutoMapperProfileSap : Profile
    {
        public AutoMapperProfileSap()
        {
            CreateMap<OSKCEntity, OSKCViewEntity>().ReverseMap();
            CreateMap<OSKCViewEntity, OSKCEntity>().ReverseMap();

            CreateMap<OSKPEntity, OSKPViewEntity>().ReverseMap();
            CreateMap<OSKPViewEntity, OSKPEntity>().ReverseMap();
        }
    }
}
