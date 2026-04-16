using System;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class TakeInventoryFinishedProducts1Entity
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public string CodeBar { get; set; }
        public DateTime? ProductionDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal WeightKg { get; set; }
        public string IsDelete { get; set; }
        public int UsrCreate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public short CreateTime { get; set; }
        public short? DeleteTime { get; set; }


        // 🔹 Relación inversa: cada línea pertenece a una toma de inventario
        public TakeInventoryFinishedProductsEntity TakeInventoryFinishedProducts { get; set; }
    }
}
