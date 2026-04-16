namespace Net.Business.Entities.SAPBusinessOne
{
    public class Orders1UpdateEntity
    {
        public string? LineStatus { get; set; }
        public int LineNum { get; set; }
        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? AcctCode { get; set; }
        public string? WhsCode { get; set; }

        public string? UnitMsr { get; set; }
        public double Quantity { get; set; }

        public string? Currency { get; set; }
        public double PriceBefDi { get; set; }
        public double DiscPrcnt { get; set; }
        public double Price { get; set; }

        public string? TaxCode { get; set; }
        public double LineTotal { get; set; }

        public string? U_FIB_LinStPkg { get; set; }
        public double U_FIB_OpQtyPkg { get; set; }
        public string? U_tipoOpT12 { get; set; }

        public int Record { get; set; }
    }
}
