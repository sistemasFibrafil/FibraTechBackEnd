using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.BusinessPartners.Vehicles.Create
{
    public class VehiclesCreateEntity
    {
        public string? CardCode { get; set; }
        public List<VehiclesLinesCreateEntity> Lines { get; set; } = [];
    }
}
