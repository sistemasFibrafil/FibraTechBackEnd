using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TaxGroupsFindRequestDto
    {
        public string CardCode { get; set; }
        public string Address { get; set; }
        public int SlpCode { get; set; }

        public TaxGroupsFindEntity ReturnValue()
        {
            return new TaxGroupsFindEntity
            {
                CardCode = CardCode,
                Address = Address,
                SlpCode = SlpCode
            };
        }
    }
}
