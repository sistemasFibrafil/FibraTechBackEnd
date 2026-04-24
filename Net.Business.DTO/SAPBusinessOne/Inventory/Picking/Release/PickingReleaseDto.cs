using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities;
namespace Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Release
{
    public class PickingReleaseDto
    {
        public string? U_IsReleased { get; set; }
        public int U_BaseType { get; set; }
        public string? U_CodeBar { get; set; }
        public int? U_UsrRelease { get; set; }
        public PickingEntity ReturnValue()
        {
            return new PickingEntity()
            {
                U_IsReleased = U_IsReleased,
                U_BaseType = U_BaseType,
                U_CodeBar = U_CodeBar,
                U_UsrRelease = U_UsrRelease
            };
        }
    }
}
