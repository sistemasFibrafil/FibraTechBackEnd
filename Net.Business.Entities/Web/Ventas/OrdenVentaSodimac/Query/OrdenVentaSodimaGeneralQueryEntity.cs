using System;
namespace Net.Business.Entities.Web
{
    public class OrdenVentaSodimaGeneralQueryEntity
    {
        public int Id { get; set; }
        public int Line1 { get; set; }
        public int Line2 { get; set; }
        public int DocNum { get; set; }
        public string? NumOrdenCompra { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public int NumLocal { get; set; }
        public string? NomLocal { get; set; }
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? ItemCode { get; set; }
        public string? Sku { get; set; }
        public string? Dscription { get; set; }
        public string? DscriptionLarga { get; set; }
        public string? Ean { get; set; }
        public string? Lpn { get; set; }
        public decimal Quantity { get; set; }
    }
}
