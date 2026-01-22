using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
{
    public class PickingFindDto
    {
        public string U_Status { get; set; }
        public int U_BaseEntry { get; set; }
        public int U_BaseType { get; set; }
        public int U_BaseLine { get; set; }
        public string U_CodeBar { get; set; }

        public PickingEntity ReturnValue()
        {
            return new PickingEntity()
            {
                U_Status = this.U_Status,
                U_BaseEntry = this.U_BaseEntry,
                U_BaseType = this.U_BaseType,
                U_BaseLine = this.U_BaseLine,
                U_CodeBar = this.U_CodeBar
            };
        }
    }
}
