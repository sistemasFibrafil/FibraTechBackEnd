using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Vehicles.Filter;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Vehicle.Filter;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.BusinessPartners.Vehicles.Filter
{
    public class VehiclesFilterMapper
    {
        public static VehiclesFilterEntity ToEntity(VehiclesFilterRequestDto dto)
        {
            return new VehiclesFilterEntity
            {
                U_FIB_COTR = dto.U_FIB_COTR,
                SearchText = dto.SearchText
            };
        }
    }
}
