namespace Net.Business.Entities.SAPBusinessOne
{
    public class Orders1Entity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }

        public string LineStatus { get; set; } = string.Empty;
        public string ObjType { get; set; } = string.Empty;
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }

        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? AcctCode { get; set; }
        public string? WhsCode { get; set; }

        public string? UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public string? Currency { get; set; }
        public decimal PriceBefDi { get; set; }
        public decimal? DiscPrcnt { get; set; }
        public decimal Price { get; set; }
        public string? TaxCode { get; set; }
        public decimal? VatPrcnt { get; set; }
        public decimal VatSum { get; set; }
        public decimal VatSumSy { get; set; }
        public decimal LineTotal { get; set; }
        public decimal TotalSumSy { get; set; }

        public string? U_FIB_LinStPkg { get; set; }
        public string? U_FIB_FromPkg { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; }
        public string? U_tipoOpT12 { get; set; }


        // 🔗 N → 1 (RDR1 → OITM)
        public ItemsEntity Item { get; set; } = null!;


        // 🔗 N → 1 (RDR1 → OACT)
        public ChartOfAccountsEntity ChartOfAccounts { get; set; } = null!;


        // 🔗 N → 1 (RDR1 → TipoOperacion)
        public OperationTypeEntity OperationType { get; set; } = null!;
    }
}
