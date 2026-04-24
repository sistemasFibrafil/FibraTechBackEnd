namespace Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Entities
{
    public class StockTransfers1Entity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ObjType { get; set; } = string.Empty;
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; } = null;
        public int? BaseLine { get; set; } = null;
        public string? U_FIB_FromPkg { get; set; }
        public string LineStatus { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string? Dscription { get; set; }
        public string FromWhsCod { get; set; } = string.Empty;
        public string WhsCode { get; set; } = string.Empty;
        public string? U_tipoOpT12 { get; set; }
        public string? UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal? U_FIB_NBulto { get; set; }
        public decimal? U_FIB_PesoKg { get; set; }


        // 🔗 N → 1 (WTR1 → TipoOperacion)
        public OperationTypeEntity? OperationType { get; set; } = null;
    }
}
