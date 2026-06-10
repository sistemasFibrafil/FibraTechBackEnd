using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.Countries;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class AdminInfo1Entity
    {
        public int Code { get; set; }
        public string? Street { get; set; }
        public string? County { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public AdminInfoEntity? AdminInfo { get; set; }
        public CountriesEntity? Countries { get; set; }
    }
}
