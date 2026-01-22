using System;
namespace Net.Business.Entities.Sap
{
    public class ChartOfAccountsEntity
    {
        public string AcctCode { get; set; }
        public string Segment_0 { get; set; }
        public string Segment_1 { get; set; }
        public string Segment_2 { get; set; }
        public string AcctName { get; set; }
        public string FrozenFor { get; set; }
        public Int16 Levels { get; set; }
        public string Postable { get; set; }
    }
}
