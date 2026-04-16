using System;
namespace Net.Business.Entities.Web
{
    public class OrdenVentaSodimacSelvaFilterEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? SearchText { get; set; }
    }
}
