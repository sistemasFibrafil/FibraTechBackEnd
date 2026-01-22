using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class TakeInventoryFinishedProductsEntity
    {
        public int DocEntry { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string WhsCode { get; set; }
        public string UnitMsr { get; set; }
        public decimal? OnHandSys { get; set; }
        public string IsDelete { get; set; }
        public int UsrCreate { get; set; }
        public int? UsrDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Int16 CreateTime { get; set; }
        public Int16? DeleteTime { get; set; }


        // 🔹 Relación 1:N — un artículo de toma de inventario tiene muchos codigos de barras
        public ICollection<TakeInventoryFinishedProducts1Entity> TakeInventoryFinishedProducts1 { get; set; }
    }
}
