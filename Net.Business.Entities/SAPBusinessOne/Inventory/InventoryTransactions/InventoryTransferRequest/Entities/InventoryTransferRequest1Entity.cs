namespace Net.Business.Entities.SAPBusinessOne
{
    public class InventoryTransferRequest1Entity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string? LineStatus { get; set; }
        public string? ObjType { get; set; }
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string? Dscription { get; set; }
        public string? FromWhsCod { get; set; }
        public string? WhsCode { get; set; }
        public string? U_tipoOpT12 { get; set; }
        public string? UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }

        public string? U_FIB_FromPkg { get; set; }
        public string? U_FIB_LinStPkg { get; set; }
        public decimal? U_FIB_NBulto { get; set; }
        public decimal? U_FIB_PesoKg { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; }


        // 🔗 N → 1 (WTQ1 → TipoOperacion)
        public OperationTypeEntity? OperationType { get; set; } = null;
    }
}
