using System;
namespace Net.Business.Entities.Web
{
    public class OrdenVentaSodimacFilterEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Tipo { get; set; }
        public string? SearchText { get; set; }
    }
}
