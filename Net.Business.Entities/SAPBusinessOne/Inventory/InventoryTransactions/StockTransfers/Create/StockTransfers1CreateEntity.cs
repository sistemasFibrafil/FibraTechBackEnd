namespace Net.Business.Entities.SAPBusinessOne
{
    public class StockTransfers1CreateEntity
    {
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public int? BaseType { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string? Dscription { get; set; }
        public string FromWhsCod { get; set; } = string.Empty;
        public string WhsCode { get; set; } = string.Empty;
        public string? UnitMsr { get; set; }
        public double Quantity { get; set; }
        public double OpenQty { get; set; }

        public string? U_FIB_FromPkg { get; set; }
        public string? U_tipoOpT12 { get; set; }
        public double U_FIB_NBulto { get; set; }
        public double U_FIB_PesoKg { get; set; }
    }
}
