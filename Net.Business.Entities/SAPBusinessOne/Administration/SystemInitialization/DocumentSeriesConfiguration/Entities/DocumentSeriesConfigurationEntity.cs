using System.Collections.Generic;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class DocumentSeriesConfigurationEntity
    {
        public string Code { get; set; } = string.Empty;
        public int? U_IdUser { get; set; }
        public string? U_Active { get; set; }


        // 🔗 1 → N (@FIB_OCSD → @FIB_CSD1)
        public ICollection<DocumentSeriesConfiguration1Entity> Lines { get; set; } = new List<DocumentSeriesConfiguration1Entity>();
    }
}
