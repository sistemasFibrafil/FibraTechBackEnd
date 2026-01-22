using System;
namespace Net.Business.Entities.Sap
{
    public class PriceListsEntity
    {
        public string ItemCode { get; set; }
        public Int16 PriceList { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }

        // 🔹 Relación N:1
        public ItemsEntity Item { get; set; }
    }
}
