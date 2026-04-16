using AutoMapper;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.AppContext
{
    public class AutoMapperProfileSAPBusinessOne : Profile
    {
        public AutoMapperProfileSAPBusinessOne()
        {
            CreateMap<OSKCEntity, OSKCViewEntity>().ReverseMap();
            CreateMap<OSKCViewEntity, OSKCEntity>().ReverseMap();

            CreateMap<OSKPEntity, OSKPViewEntity>().ReverseMap();
            CreateMap<OSKPViewEntity, OSKPEntity>().ReverseMap();

            CreateMap<VehiclesEntity, Vehicles1CreateEntity>().ReverseMap();
            CreateMap<Vehicles1CreateEntity, VehiclesEntity>().ReverseMap();

            CreateMap<DriversEntity, Drivers1CreateEntity>().ReverseMap();
            CreateMap<Drivers1CreateEntity, DriversEntity>().ReverseMap();
        }
    }
}
