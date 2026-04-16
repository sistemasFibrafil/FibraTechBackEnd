using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class AddressesFindRequestDto
    {
        public string? CardCode { get; set; }
        public string? Address { get; set; }
        public string? AdresType { get; set; }
        public AddressesEntity ReturnValue()
        {
            return new AddressesEntity
            {
                CardCode = CardCode,
                AdresType = AdresType,
                Address = Address,
            };
        }
    }
}
