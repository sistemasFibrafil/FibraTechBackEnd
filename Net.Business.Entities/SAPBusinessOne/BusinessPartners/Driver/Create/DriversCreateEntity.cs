using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class DriversCreateEntity
    {
        public string? CardCode { get; set; }
        public List<Drivers1CreateEntity> Lines { get; set; } = new List<Drivers1CreateEntity>();
    }
}
