using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class VehiclesCreateEntity
    {
        public string? CardCode { get; set; }
        public List<Vehicles1CreateEntity> Lines { get; set; } = new List<Vehicles1CreateEntity>();
    }
}
