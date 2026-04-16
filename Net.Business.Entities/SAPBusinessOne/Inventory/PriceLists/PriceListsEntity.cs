using System;
using Net.Business.Entities.SAPBusinessOne;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class PriceListsEntity
    {
        public string ItemCode { get; set; }
        public short PriceList { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }

        // 🔹 Relación N:1
        public ItemsEntity Item { get; set; }
    }
}
