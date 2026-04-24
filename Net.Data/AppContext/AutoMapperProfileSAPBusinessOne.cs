using AutoMapper;
using Net.Business.Entities.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Driver.Create;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Driver.Entities;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Vehicle.Entities;
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

            CreateMap<VehiclesEntity, VehiclesLinesCreateEntity>().ReverseMap();
            CreateMap<VehiclesLinesCreateEntity, VehiclesEntity>().ReverseMap();

            CreateMap<DriversEntity, DriversLinesCreateEntity>().ReverseMap();
            CreateMap<DriversLinesCreateEntity, DriversEntity>().ReverseMap();
        }
    }
}
