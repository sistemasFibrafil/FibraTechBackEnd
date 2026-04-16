using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{ 
    public class PickingPrintFindRequestDto
    {
        public int U_TrgetEntry { get; set; }
        public int U_TargetType { get; set; }

        public PickingEntity ReturnValue()
        {
            return new PickingEntity()
            {
                U_TrgetEntry  = U_TrgetEntry,
                U_TargetType = U_TargetType,
            };
        }
    }
}
