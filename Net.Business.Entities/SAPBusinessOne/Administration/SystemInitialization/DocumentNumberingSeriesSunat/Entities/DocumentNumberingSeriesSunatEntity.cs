namespace Net.Business.Entities.SAPBusinessOne
{
    public class DocumentNumberingSeriesSunatEntity
    {
        public string Code { get; set; } = string.Empty;
        public string? U_BPP_NDTD { get; set; }
        public string? U_BPP_NDSD { get; set; }
        public string? U_BPP_NDCD { get; set; }
    }
}
