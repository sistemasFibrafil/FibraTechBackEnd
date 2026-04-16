using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class DocumentNumberingSeriesEntity
    {
        public string ObjectCode { get; set; } = string.Empty;
        public string DocSubType { get; set; } = string.Empty;
        public Int16 DfltSeries { get; set; }

        public DocumentNumberingSeries1Entity DocumentNumberingSeries1 { get; set; } = null!;
    }
}
