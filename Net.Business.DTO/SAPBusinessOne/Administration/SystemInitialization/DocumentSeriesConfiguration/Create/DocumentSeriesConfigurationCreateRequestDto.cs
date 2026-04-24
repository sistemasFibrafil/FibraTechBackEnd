using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create
{
    public class DocumentSeriesConfigurationCreateRequestDto
    {
        public string? Code { get; set; }
        public int? U_IdUser { get; set; }
        public string? U_Active { get; set; }

        public List<DocumentSeriesConfigurationLinesCreateRequestDto> Lines { get; set; } = [];
    }
}
