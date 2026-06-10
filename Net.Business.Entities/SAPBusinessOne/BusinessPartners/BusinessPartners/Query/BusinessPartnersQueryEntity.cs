using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerGroups;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class BusinessPartnersQueryEntity
    {
        public string? CardCode { get; set; }
        public string? LicTradNum { get; set; }
        public string? CardName { get; set; }
        public string? CardType { get; set; }
        public short GroupCode { get; set; }
        public List<BusinessPartnerGroupsEntity> GroupsLines { get; set; } = [];
        public string? Currency { get; set; }
        public List<CurrencyCodesEntity> CurrencyCodesLines { get; set; } = [];

        /// <summary>
        /// TIPO DE DOCUMENTO DE IDENTIDAD
        /// </summary>
        public string? U_BPP_BPTP { get; set; }
        public string? U_BPP_BPTD { get; set; }
        public string? U_FIB_Divi { get; set; }
        public string? U_FIB_Sector { get; set; }

        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? Cellular { get; set; }
        public string? Email { get; set; }
        public string? ValidFor { get; set; }
        public int SlpCode { get; set; }
        public string? SlpName { get; set; }
        public string? Notes { get; set; }

        public int CntctCode { get; set; }
        public string? CntctPrsn { get; set; }
        public List<ContactEmployeesQueryEntity> ContactEmployeesLines { get; set; } = [];

        public string? BillToDef { get; set; }
        public List<AddressesQueryEntity> PayAddressLines { get; set; } = [];
        public string? Address { get; set; }
        public string? ShipToDef { get; set; }
        public List<AddressesQueryEntity> ShipAddressLines { get; set; } = [];
        public string? Address2 { get; set; }

        public short GroupNum { get; set; }
        public string? GroupName { get; set; }
        public short ListNum { get; set; }
        public decimal CreditLine { get; set; }
        public decimal DebtLine { get; set; }

        public string? U_BPP_BPAT { get; set; }
        public string? U_FIB_EMAIL2 { get; set; }
        public string? U_FIB_EMAIL3 { get; set; }
        public string? U_BPP_BPNO { get; set; }
        public string? U_BPP_BPAP { get; set; }
        public string? U_BPP_BPAM { get; set; }

        public List<VehiclesQueryEntity> VehiclesLines { get; set; } = [];
        public List<DriversQueryEntity> DriversLines { get; set; } = [];
    }
}
