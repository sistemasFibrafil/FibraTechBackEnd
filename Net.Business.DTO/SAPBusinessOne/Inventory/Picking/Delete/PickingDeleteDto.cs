using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities;
namespace Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Delete
{
    public class PickingDeleteDto
    {
        public int DocEntry { get; set; }
        public int U_BaseEntry { get; set; }
        public int U_BaseType { get; set; }
        public int U_BaseLine { get; set; }

        public PickingEntity ReturnValue()
        {
            return new PickingEntity()
            {
                DocEntry = DocEntry,
                U_BaseEntry = U_BaseEntry,
                U_BaseType = U_BaseType,
                U_BaseLine = U_BaseLine
            };
        }
    }
}
