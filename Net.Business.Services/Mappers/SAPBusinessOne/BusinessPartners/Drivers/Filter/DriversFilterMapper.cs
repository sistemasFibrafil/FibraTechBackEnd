using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;

namespace Net.Business.Services.Mappers.SAPBusinessOne
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
