using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class BusinessPartnersFilterRequestDto
    {
        public string? CardType { get; set; }
        public string? SearchText { get; set; }

        public BusinessPartnersFilterEntity ReturnValue()
        {
            return new BusinessPartnersFilterEntity
            {
                CardType = CardType,
                SearchText = SearchText
            };
        }
    }
}
