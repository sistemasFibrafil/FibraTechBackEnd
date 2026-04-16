using System;
namespace Net.Business.Entities.Web
{
    public class OrdenVentaSodimacSelvaQueryEntity
    {
        public string? Proveedor { get; set; }
        public string? NumOrdenCompra { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public int NumPallet { get; set; }
        public int NumLocal { get; set; }
        public string? NomLocal { get; set; }
        public string? Ean { get; set; }
        public string? Sku { get; set; }
        public string? DscriptionLarga { get; set; }
        public decimal Quantity { get; set; }
    }
}
