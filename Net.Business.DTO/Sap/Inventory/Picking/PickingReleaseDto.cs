using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class PickingReleaseDto
    {
        public string U_IsReleased { get; set; }
        public int U_BaseType { get; set; }
        public string U_CodeBar { get; set; }
        public int? U_UsrRelease { get; set; }
        public PickingEntity ReturnValue()
        {
            return new PickingEntity()
            {
                U_IsReleased = this.U_IsReleased,
                U_BaseType = this.U_BaseType,
                U_CodeBar = this.U_CodeBar,
                U_UsrRelease = this.U_UsrRelease
            };
        }
    }
}
