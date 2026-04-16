using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class ChartOfAccountsEntity
    {
        public string AcctCode { get; set; } = string.Empty;
        public string? Segment_0 { get; set; }
        public string? Segment_1 { get; set; }
        public string? Segment_2 { get; set; }
        public string? AcctName { get; set; }
        public string? FrozenFor { get; set; }
        public short Levels { get; set; }
        public string? Postable { get; set; }
    }
}
