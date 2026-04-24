using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create
{
    public class DocumentSeriesConfigurationCreateEntity
    {
        public string? Code { get; set; }
        public int? U_IdUser { get; set; }
        public string? U_Active { get; set; }

        public List<DocumentSeriesConfigurationLinesCreateEntity> Lines { get; set; } = [];
    }
}
