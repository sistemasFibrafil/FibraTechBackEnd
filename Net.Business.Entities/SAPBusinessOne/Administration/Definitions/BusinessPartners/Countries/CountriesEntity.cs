using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.Countries
{
    public class CountriesEntity
    {
        public string Code { get; set; } = string.Empty;
        public string? Name { get; set; }

        public ICollection<AdminInfo1Entity> AdminInfosLines { get; set; } = [];
    }
}
