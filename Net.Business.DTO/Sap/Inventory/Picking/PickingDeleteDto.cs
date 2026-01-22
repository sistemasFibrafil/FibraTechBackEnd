using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
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
                DocEntry = this.DocEntry,
                U_BaseEntry = this.U_BaseEntry,
                U_BaseType = this.U_BaseType,
                U_BaseLine = this.U_BaseLine
            };
        }
    }
}
