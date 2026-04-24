namespace Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create
{
    public class DocumentSeriesConfigurationLinesCreateEntity
    {
        public string? Code { get; set; }
        public int LineId { get; set; }
        public string? U_Type { get; set; }
        public string? U_Series { get; set; }
        public string? U_SalesInvoices { get; set; }
        public string? U_Delivery { get; set; }
        public string? U_Transfer { get; set; }
        public string? U_Default { get; set; }
        public string? U_Active { get; set; }
        public int Record { get; set; }
    }
}
