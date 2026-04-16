using System;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class OrdenVentaSodimacSelvaFilterRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? SearchText { get; set; }

        public OrdenVentaSodimacSelvaFilterEntity ReturnValue()
        {
            return new OrdenVentaSodimacSelvaFilterEntity()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                SearchText = SearchText
            };
        }
    }
}
