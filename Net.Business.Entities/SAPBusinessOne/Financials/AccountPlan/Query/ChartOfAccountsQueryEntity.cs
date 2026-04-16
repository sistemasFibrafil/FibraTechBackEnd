namespace Net.Business.Entities.SAPBusinessOne
{
    public class ChartOfAccountsQueryEntity
    {
        public string AcctCode { get; set; } = string.Empty;
        public string FormatCode { get; set; } = string.Empty;
        public string? AcctName { get; set; }
    }
}
