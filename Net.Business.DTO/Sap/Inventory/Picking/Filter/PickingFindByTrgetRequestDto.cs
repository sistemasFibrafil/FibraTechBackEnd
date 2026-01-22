using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
{
    public class PickingFindByTrgetRequestDto
    {
        public int U_TrgetEntry { get; set; }
        public int U_TargetType { get; set; }
        public int U_TrgetLine { get; set; }
        public string U_CodeBar { get; set; } = null;

        public PickingEntity ReturnValue()
        {
            return new PickingEntity()
            {
                U_TrgetEntry = this.U_TrgetEntry,
                U_TargetType = this.U_TargetType,
                U_TrgetLine = this.U_TrgetLine,
                U_CodeBar = this.U_CodeBar,
            };
        }
    }
}
