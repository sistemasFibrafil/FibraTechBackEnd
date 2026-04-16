using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class DocumentNumberingSeries1Entity
    {
        public string ObjectCode { get; set; } = string.Empty;
        public string DocSubType { get; set; } = string.Empty;
        public Int16 Series { get; set; }

        public string SeriesName { get; set; } = string.Empty;
        public int? NextNumber { get; set; }
    }
}
