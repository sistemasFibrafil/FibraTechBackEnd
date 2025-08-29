using AutoMapper;
using Net.Business.Entities.Sap;
namespace Net.Data.AppContext
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<OSKCEntity, OSKCViewEntity>().ReverseMap();
            CreateMap<OSKCViewEntity, OSKCEntity>().ReverseMap();

            CreateMap<OSKPEntity, OSKPViewEntity>().ReverseMap();
            CreateMap<OSKPViewEntity, OSKPEntity>().ReverseMap();
        }
    }
}
