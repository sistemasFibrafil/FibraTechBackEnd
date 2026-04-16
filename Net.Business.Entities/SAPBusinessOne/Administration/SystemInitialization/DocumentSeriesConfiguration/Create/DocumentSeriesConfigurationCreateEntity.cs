using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class DocumentSeriesConfigurationCreateEntity
    {
        public string? Code { get; set; }
        public int? U_IdUser { get; set; }
        public string? U_Active { get; set; }

        public List<DocumentSeriesConfiguration1CreateEntity> Lines { get; set; } = new List<DocumentSeriesConfiguration1CreateEntity>();
    }
}
