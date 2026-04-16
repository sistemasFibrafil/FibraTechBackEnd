namespace Net.Business.Entities.SAPBusinessOne
{
    public class DocumentSeriesConfiguration1QueryEntity
    {
        public string Code { get; set; } = string.Empty;
        public int LineId { get; set; }
        public string? U_Type { get; set; }
        public string? U_Series { get; set; }
        public bool U_SalesInvoices { get; set; }
        public bool U_Delivery { get; set; }
        public bool U_Transfer { get; set; }
        public bool U_Default { get; set; }
        public bool U_Active { get; set; }
        public int Record { get; set; } = 2;
    }
}
