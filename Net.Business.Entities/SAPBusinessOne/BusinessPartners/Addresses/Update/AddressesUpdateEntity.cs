namespace Net.Business.Entities.SAPBusinessOne.BusinessPartners.Addresses.Update
{
    public class AddressesUpdateEntity
    {
        public int LineNum { get; set; }
        public string? Address { get; set; }
        public string? AdresType { get; set; }
        public string? Street { get; set; }
        public string? Block { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? County { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? BuildingFloorRoom { get; set; }
        public string? GlblLocNum { get; set; }
        public string? TaxCode { get; set; }
        public int Record { get; set; } = 2;
    }
}
