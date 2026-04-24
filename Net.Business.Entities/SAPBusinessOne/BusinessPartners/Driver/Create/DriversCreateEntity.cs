using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.BusinessPartners.Driver.Create
{
    public class DriversCreateEntity
    {
        public string? CardCode { get; set; }
        public List<DriversLinesCreateEntity> Lines { get; set; } = [];
    }
}
