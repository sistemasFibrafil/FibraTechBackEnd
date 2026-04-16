using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
{
    public class PickingFindMapper
    {
        public static PickingFindEntity ToEntity(PickingFindRequestDto dto)
        {
            return new PickingFindEntity
            {
                U_Status = dto.U_Status,
                U_BaseEntry = dto.U_BaseEntry,
                U_BaseType = dto.U_BaseType,
                U_BaseLine = dto.U_BaseLine,
                U_CodeBar = dto.U_CodeBar
            };
        }
    }
}
