using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class CostCentersFilterRequestDto
    {
        public string CostCenter { get; set; }
        public string Active { get; set; }

        public CostCentersFilterEntity ReturnValue()
        {
            return new CostCentersFilterEntity
            {
                CostCenter = CostCenter,
                Active = Active
            };
        }
    }
}
