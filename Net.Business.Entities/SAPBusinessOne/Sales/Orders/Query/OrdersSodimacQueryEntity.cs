using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class OrdersSodimacQueryEntity
    {
        public int DocEntry { get; set; }
        public string? NumOrdenCompra { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public int CntctCode { get; set; }
        public string? CntctName { get; set; }
        public string? Address2 { get; set; }
    }
}
