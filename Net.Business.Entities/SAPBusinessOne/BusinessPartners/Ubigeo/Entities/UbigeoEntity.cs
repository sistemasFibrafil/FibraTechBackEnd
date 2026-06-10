namespace Net.Business.Entities.SAPBusinessOne.BusinessPartners.Ubigeo.Entities
{
    public class UbigeoEntity
    {
        public string Code { get; set; } = string.Empty;
        public string? U_NomDistrito { get; set; }
        public string? U_NomProvincia { get; set; }
        public string? U_CodDepartamento { get; set; }
        public string? U_NomDepartamento { get; set; }
    }
}
