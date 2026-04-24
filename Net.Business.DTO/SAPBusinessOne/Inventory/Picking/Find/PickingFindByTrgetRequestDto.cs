using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities;
namespace Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Find
{
    public class PickingFindByTrgetRequestDto
    {
        public int U_TrgetEntry { get; set; }
        public int U_TargetType { get; set; }
        public int U_TrgetLine { get; set; }
        public string? U_CodeBar { get; set; }

        public PickingEntity ReturnValue()
        {
            return new PickingEntity()
            {
                U_TrgetEntry = U_TrgetEntry,
                U_TargetType = U_TargetType,
                U_TrgetLine = U_TrgetLine,
                U_CodeBar = U_CodeBar,
            };
        }
    }
}
