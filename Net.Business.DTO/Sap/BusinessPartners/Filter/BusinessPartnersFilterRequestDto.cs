using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
{
    public class BusinessPartnersFilterRequestDto
    {
        public string BusinessPartner { get; set; }
        public string CardType { get; set; }
        public string TransType { get; set; }
        public BusinessPartnersFilterEntity ReturnValue()
        {
            return new BusinessPartnersFilterEntity
            {
                BusinessPartner = BusinessPartner,
                CardType = CardType,
                TransType = TransType
            };
        }
    }
}
