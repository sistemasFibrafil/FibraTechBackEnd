using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class SolicitudTrasladoFilterDto
    {
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public string DocStatus { get; set; } = null;
        public string SearchText { get; set; } = null;
        public SolicitudTrasladoFilterEntity ReturnValue()
        {
            return new SolicitudTrasladoFilterEntity()
            {
                SearchText = SearchText,
                DocStatus = DocStatus,
                StartDate = StartDate,
                EndDate = EndDate,
            };
        }
    }
}
