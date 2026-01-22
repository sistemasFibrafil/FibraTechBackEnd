using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class TakeInventoryFinishedProductsQueryEntity
    {
        public int DocEntry { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string WhsCode { get; set; }
        public string CodeBar { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string UnitMsr { get; set; }
        public decimal? OnHandPhy { get; set; }
        public decimal? OnHandSys { get; set; }
        public decimal? Difference { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? WeightKg { get; set; }
        public string TipoOpT12 { get; set; }
        public string TipoOpT12Nam { get; set; }
        public int UsrCreate { get; set; }
        public string UsrNameCreate { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16 CreateTime { get; set; }

        // Resto de columnas dinámicas (usuarios pivot)
        public Dictionary<string, object> DynamicColumns { get; set; } = new();
    }
}
