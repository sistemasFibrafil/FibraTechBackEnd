using System;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class CostCentersEntity
    {
        public string OcrCode { get; set; } = string.Empty;
        public string? OcrName { get; set; }
        public short DimCode { get; set; }
        public string? Active { get; set; }
    }
}
