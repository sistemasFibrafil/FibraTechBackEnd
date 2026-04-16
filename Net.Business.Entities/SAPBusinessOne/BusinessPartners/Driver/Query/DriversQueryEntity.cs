namespace Net.Business.Entities.SAPBusinessOne
{
    public class DriversQueryEntity
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? U_BPP_CHNO { get; set; }
        public string? U_FIB_CHAP { get; set; }
        public string? U_FIB_CHTD { get; set; }
        public string? U_FIB_CHND { get; set; }
        public string? U_BPP_CHLI { get; set; }
        public string? U_FIB_COTR { get; set; }
        public int Record { get; set; } = 2;
    }
}
