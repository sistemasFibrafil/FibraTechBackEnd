using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ContactEmployeesFilterRequestDto
    {
        public string? CardCode { get; set; }
        public string? SearchText { get; set; }

        public ContactEmployeesFilterEntity ReturnValue()
        {
            return new ContactEmployeesFilterEntity
            {
                CardCode = this.CardCode,
                SearchText = this.SearchText
            };
        }
    }
}
