using System.Collections.Generic;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class DocumentSeriesConfigurationCreateRequestDto
    {
        public string? Code { get; set; }
        public int? U_IdUser { get; set; }
        public string? U_Active { get; set; }

        public List<DocumentSeriesConfiguration1CreateRequestDto> Lines { get; set; } = new List<DocumentSeriesConfiguration1CreateRequestDto>();
    }
}
