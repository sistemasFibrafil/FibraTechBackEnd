using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class BusinessPartnersEntity
    {
        public string CardCode { get; set; } = string.Empty;
        public string? LicTradNum { get; set; }
        public string? CardName { get; set; }
        public string? CardType { get; set; }
        public short GroupCode { get; set; }
        public string? U_BPP_BPTD { get; set; }
        public string? U_BPP_BPTP { get; set; }
        public string? U_BPP_BPNO { get; set; }
        public string? U_BPP_BPAP { get; set; }
        public string? U_BPP_BPAM { get; set; }
        public string? U_BPP_BPAT { get; set; }
        public string? U_FIB_Divi { get; set; }
        public string? U_FIB_Sector { get; set; }

        [Column("U_FIB_EMAIL2")]
        public string? U_FIB_Email2 { get; set; }

        [Column("U_FIB_EMAIL3")]
        public string? U_FIB_Email3 { get; set; }

        public string? Notes { get; set; }
        public string? ValidFor { get; set; }
        public string? Currency { get; set; }
        public string? Phone1 { get; set; }
        public string? Cellular { get; set; }
        public string? E_Mail { get; set; }
        public int SlpCode { get; set; }
        public string? CntctPrsn { get; set; }
        public string? BillToDef { get; set; }
        public string? Address { get; set; }
        public string? ShipToDef { get; set; }
        public string? MailAddres { get; set; }
        public string? State2 { get; set; }
        public string? MailCounty { get; set; }
        public string? MailCity { get; set; }
        public short GroupNum { get; set; }
        public short ListNum { get; set; }
        public decimal? CreditLine { get; set; }
        public decimal? Balance { get; set; }
        public decimal? OrdersBal { get; set; }

        // 🔗 1 → N (OCRD → OCRG)
        public BusinessPartnerGroupsEntity? BusinessPartnerGroups { get; set; } = null;

        // 🔗 1 → N (OCRD → OSLP)
        public SalesPersonsEntity? SalesPersons { get; set; } = null;

        // 🔗 1 → N (OCRD → OCPR)
        public ContactEmployeesEntity? ContactEmployees { get; set; } = null;

        // 🔗 1 → N (OCRD → OCST)
        public StatesEntity? State { get; set; } = null;

        // 🔗 1 → N (OCRD → CRD1)
        public ICollection<AddressesEntity> LinesAddresses { get; set; } = new List<AddressesEntity>();

        // 🔗 1 → N (OCRD → @BPP_VEHICU)
        public ICollection<VehiclesEntity> LinesVehicles { get; set; } = new List<VehiclesEntity>();

        // 🔗 1 → N (OCRD → @BPP_CONDUC)
        public ICollection<DriversEntity> LinesDrivers { get; set; } = new List<DriversEntity>();
    }

    public class SocioNegocioQueryEntity
    {
        public string CardCode { get; set; }
        public string LicTradNum { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string TransType { get; set; }
        public string Currency { get; set; }
        public string CodStatus { get; set; }
        public string NomStatus { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Movil { get; set; }
        public string Email { get; set; }
        public int SlpCode { get; set; }
        public string SlpName { get; set; }
        public int CntctCode { get; set; }
        public string CntctPrsn { get; set; }
        public string NomContacto { get; set; }
        public string TelContacto1 { get; set; }
        public string TelContacto2 { get; set; }
        public string MovilContacto { get; set; }
        public string EmailContacto { get; set; }
        public string BillToDef { get; set; }
        public string Address { get; set; }
        public string Pais { get; set; }
        public string NomDepartamento { get; set; }
        public string NomProvincia { get; set; }
        public string NomDistrito { get; set; }
        public string ShipToDef { get; set; }
        public string Address2 { get; set; }
        public string Ubigeo { get; set; }
        public int GroupNum { get; set; }
        public string NomSector { get; set; }
        public string NomDivision { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LowDate { get; set; }
        public DateTime? FechaUltimaVenta { get; set; }
    }
}