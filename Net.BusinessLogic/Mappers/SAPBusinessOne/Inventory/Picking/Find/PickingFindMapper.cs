using Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Find;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Find;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Inventory.Picking.Find
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
