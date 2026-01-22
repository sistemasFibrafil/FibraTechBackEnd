using System;
namespace Net.Business.Entities.Sap
{
    public class NumeracionDocumentoEntity
    {
        public string ObjectCode { get; set; }
        public string DocSubType { get; set; }
        public Int16 DfltSeries { get; set; }
        public NumeracionDocumento1Entity NumeracionDocumento1 { get; set; }
    }

    public class NumeracionDocumento1Entity
    {
        public string ObjectCode { get; set; }
        public Int16 Series { get; set; }
        public string SeriesName { get; set; }
        public int NextNumber { get; set; }
    }
}
