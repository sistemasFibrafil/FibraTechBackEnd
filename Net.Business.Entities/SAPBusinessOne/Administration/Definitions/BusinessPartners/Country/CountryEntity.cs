using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class CountryEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public ICollection<AdminInfo1Entity> AdminInfos1 { get; set; }
    }
}
