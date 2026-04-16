using System;
namespace Net.Business.Entities.Web
{
    public class OrdenVentaSodimacQueryEntity
    {
        public int Id { get; set; }
        public int DocNum { get; set; }
        public string? NumOrdenCompra { get; set; }
        public string? DocStatus { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
    }
}
