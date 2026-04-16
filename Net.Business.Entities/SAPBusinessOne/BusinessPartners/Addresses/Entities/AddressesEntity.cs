namespace Net.Business.Entities.SAPBusinessOne
{
    public class AddressesEntity
    {
        public string? Address { get; set; }
        public string? CardCode { get; set; }
        public string? Street { get; set; }
        public int LineNum { get; set; }
        public string? AdresType { get; set; }
        public string? County { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? TaxCode { get; set; }

        public StatesEntity? States { get; set; } = null;

        public BusinessPartnersEntity? BusinessPartner { get; set; } = null;

        public TaxGroupsEntity? TaxGroup { get; set; } = null;
    }
}
