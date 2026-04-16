using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ContactEmployeesFindRequestDto
    {
        public string? CardCode { get; set; }
        public int CntctCode { get; set; }

        public ContactEmployeesFindEntity ReturnValue()
        {
            return new ContactEmployeesFindEntity
            {
                CardCode = this.CardCode,
                CntctCode = this.CntctCode
            };
        }
    }
}
