using Net.Business.Entities.Sap;
namespace Net.Business.DTO
{
    public class TipoCambioFindRequestDto
    {
        public decimal RateDate { get; set; }
        public string Currency { get; set; }

        public TipoCambioSapEntity ReturnValue()
        {
            return new TipoCambioSapEntity
            {
                RateDate = RateDate,
                Currency = Currency
            };
        }
    }
}
