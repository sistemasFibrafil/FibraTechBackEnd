namespace Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Create
{
    public class StockTransfersLinesCreateEntity
    {
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public int? BaseType { get; set; }
        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? FromWhsCod { get; set; }
        public string? WhsCode { get; set; }
        public string? UnitMsr { get; set; }
        public double Quantity { get; set; }
        public double OpenQty { get; set; }

        public string? U_FIB_FromPkg { get; set; }
        public string? U_tipoOpT12 { get; set; }
        public double U_FIB_NBulto { get; set; }
        public double U_FIB_PesoKg { get; set; }
    }
}
