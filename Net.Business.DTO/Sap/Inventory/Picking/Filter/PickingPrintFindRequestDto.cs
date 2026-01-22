using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
{ 
    public class PickingPrintFindRequestDto
    {
        public int U_TrgetEntry { get; set; }
        public int U_TargetType { get; set; }

        public PickingEntity ReturnValue()
        {
            return new PickingEntity()
            {
                U_TrgetEntry  = this.U_TrgetEntry,
                U_TargetType = this.U_TargetType,
            };
        }
    }
}
