using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class BusinessPartnersSectorStatusFilterRequestDto
    {
        public string? BusinessPartner { get; set; }
        public string? Sector { get; set; }
        public string? Status { get; set; }
        public BusinessPartnersSectorStatusFilterEntity ReturnValue()
        {
            return new BusinessPartnersSectorStatusFilterEntity
            {
                BusinessPartner = BusinessPartner,
                Sector = Sector,
                Status = Status
            };
        }
    }
}
