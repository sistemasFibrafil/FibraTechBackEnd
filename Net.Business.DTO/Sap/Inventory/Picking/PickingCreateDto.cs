using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
{
    public class PickingCreateDto
    {
        public int U_BaseEntry { get; set; }
        public int U_BaseType { get; set; }
        public string U_FromWhsCod { get; set; }
        public string U_CodeBar { get; set; }
        public int? U_UsrCreate { get; set; }

        public PickingEntity ReturnValue()
        {
            return new PickingEntity()
            {
                U_BaseType = this.U_BaseType,
                U_BaseEntry = this.U_BaseEntry,
                U_FromWhsCod = this.U_FromWhsCod,
                U_CodeBar = this.U_CodeBar,
                U_UsrCreate = this.U_UsrCreate,
            };
        }
    }
}
