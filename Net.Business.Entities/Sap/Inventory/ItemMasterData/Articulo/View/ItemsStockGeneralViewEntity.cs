using System;
namespace Net.Business.Entities.Sap
{
    public class ItemsStockGeneralViewEntity
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public string InvntryUom { get; set; }
        public decimal OnHand { get; set; }
        public decimal IsCommited { get; set; }
        public decimal OnOrder { get; set; }
        public decimal Available { get; set; }
        public decimal PesoPromedioKg { get; set; }
        public decimal PesoKg { get; set; }
        public DateTime? FecProduccion { get; set; }
        public string frozenFor { get; set; }
    }
}
