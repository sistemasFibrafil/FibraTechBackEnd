using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class BusinessPartnersViewEntity
    {
        public string? CardCode { get; set; }
        public string? LicTradNum { get; set; }
        public string? DocType { get; set; }
        public string? CardName { get; set; }
        public string? CardType { get; set; }
        public string? TransType { get; set; }
        public string? UnidadNegocio { get; set; }
        public string? Currency { get; set; }
        public decimal? CreditLine { get; set; }
        public string? CodStatus { get; set; }
        public string? NomStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LowDate { get; set; }
        public int? SlpCode { get; set; }
        public string? SlpName { get; set; }
        public string? Tel1 { get; set; }
        public string? Tel2 { get; set; }
        public string? Movil { get; set; }
        public string? Email { get; set; }
        public int? CntctCode { get; set; }
        public string? CntctPrsn { get; set; }
        public string? NomContacto { get; set; }
        public string? TelContacto1 { get; set; }
        public string? TelContacto2 { get; set; }
        public string? MovilContacto { get; set; }
        public string? EmailContacto { get; set; }
        public string? BillToDef { get; set; }
        public string? Address { get; set; }
        public string? Pais { get; set; }
        public string? NomDepartamento { get; set; }
        public string? NomProvincia { get; set; }
        public string? NomDistrito { get; set; }
        public string? ShipToDef { get; set; }
        public string? Address2 { get; set; }
        public string? Ubigeo { get; set; }
        public short? GroupNum { get; set; }
        public string? CodSector { get; set; }
        public string? NomSector { get; set; }
        public string? NomDivision { get; set; }
        public DateTime? FechaUltimaVenta { get; set; }
    }
}
