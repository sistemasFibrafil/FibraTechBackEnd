using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Drivers.Filter;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Driver.Filter;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.BusinessPartners.Drivers.Filter
{
    public class DriversFilterMapper
    {
        public static DriversFilterEntity ToEntity(DriversFilterRequestDto dto)
        {
            return new DriversFilterEntity
            {
                U_FIB_COTR = dto.U_FIB_COTR,
                SearchText = dto.SearchText
            };
        }
    }
}
