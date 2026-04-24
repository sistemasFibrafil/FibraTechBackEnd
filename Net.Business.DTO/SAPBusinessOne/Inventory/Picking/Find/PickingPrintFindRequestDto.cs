using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities;
namespace Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Find
{ 
    public class PickingPrintFindRequestDto
    {
        public int U_TrgetEntry { get; set; }
        public int U_TargetType { get; set; }

        public PickingEntity ReturnValue()
        {
            return new PickingEntity()
            {
                U_TrgetEntry  = U_TrgetEntry,
                U_TargetType = U_TargetType,
            };
        }
    }
}
