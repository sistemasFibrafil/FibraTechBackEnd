namespace Net.Business.Entities.Web
{
    public class OrdenVentaSodimacLinesEntity
    {
        public int Id { get; set; }
        public int Line1 { get; set; }
        public int Line2 { get; set; }
        public int NumLocal { get; set; }
        public bool IsOriente { get; set; } = false;
        public string? NomLocal { get; set; }
        public string? LineStatus { get; set; }
        public string? ItemCode { get; set; }
        public string? Sku { get; set; }
        public string? Dscription { get; set; }
        public string? DscriptionLarga { get; set; }
        public string? Ean { get; set; } = null;
        public string? Lpn { get; set; } = null;
        public decimal Quantity { get; set; }
    }
}
