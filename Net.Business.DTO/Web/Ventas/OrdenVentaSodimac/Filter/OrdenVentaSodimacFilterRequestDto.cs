using System;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class OrdenVentaSodimacFilterRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Tipo { get; set; }
        public string? SearchText { get; set; }

        public OrdenVentaSodimacFilterEntity ReturnValue()
        {
            return new OrdenVentaSodimacFilterEntity()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                Tipo = Tipo,
                SearchText = SearchText
            };
        }
    }
}
