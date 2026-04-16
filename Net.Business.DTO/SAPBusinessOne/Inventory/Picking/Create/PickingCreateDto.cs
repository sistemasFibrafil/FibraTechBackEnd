using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
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
                U_BaseType = U_BaseType,
                U_BaseEntry = U_BaseEntry,
                U_FromWhsCod = U_FromWhsCod,
                U_CodeBar = U_CodeBar,
                U_UsrCreate = U_UsrCreate,
            };
        }
    }
}
