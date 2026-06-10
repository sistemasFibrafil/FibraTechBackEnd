namespace Net.Business.Entities.SAPBusinessOne
{
    public class AddressesQueryEntity
    {
        public int LineNum { get; set; }
        public string? Address { get; set; }
        public string? CardCode { get; set; }
        public string? Street { get; set; }
        public string? AdresType { get; set; }
        public string? County { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? GlblLocNum { get; set; }
        public string? TaxCode { get; set; }
        public string? FullAddress { get; set; }
        public string? Default { get; set; }
        public int Record { get; set; } = 2;
    }
}
