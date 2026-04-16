using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class CostCentersFilterRequestDto
    {
        public string? CostCenter { get; set; }
        public string? Active { get; set; }

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
