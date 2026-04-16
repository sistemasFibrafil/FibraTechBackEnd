using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class BusinessPartnersModalFilterRequestDto
    {
        public string? BusinessPartner { get; set; }
        public string? CardType { get; set; }
        public string? TransType { get; set; }
        public BusinessPartnersModalFilterEntity ReturnValue()
        {
            return new BusinessPartnersModalFilterEntity
            {
                BusinessPartner = BusinessPartner,
                CardType = CardType,
                TransType = TransType
            };
        }
    }
}
