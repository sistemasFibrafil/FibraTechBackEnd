using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
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
