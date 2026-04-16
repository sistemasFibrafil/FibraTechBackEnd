using System.Collections.Generic;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class DocumentSeriesConfigurationQueryEntity
    {
        public string? Code { get; set; }
        public int? U_IdUser { get; set; }
        public bool U_Active { get; set; }

        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }

        public List<DocumentSeriesConfiguration1QueryEntity> Lines { get; set; } = new List<DocumentSeriesConfiguration1QueryEntity>();
    }
}
