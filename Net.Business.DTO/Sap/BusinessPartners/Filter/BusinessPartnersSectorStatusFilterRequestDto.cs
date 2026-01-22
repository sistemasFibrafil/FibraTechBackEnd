using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class BusinessPartnersSectorStatusFilterRequestDto
    {
        public string BusinessPartner { get; set; }
        public string Sector { get; set; }
        public string Status { get; set; }
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
