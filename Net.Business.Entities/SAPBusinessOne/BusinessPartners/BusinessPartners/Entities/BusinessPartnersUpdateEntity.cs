using System;
using System.Collections.Generic;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class BusinessPartnersUpdateEntity
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public int? GroupCode { get; set; }
        public string LicTradNum { get; set; }
        public string Phone1 { get; set; }
        public string Cellular { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public int? SlpCode { get; set; }
        public int? GroupNum { get; set; }
        public decimal? CreditLine { get; set; }
        public string Currency { get; set; }
        public string CntctPrsn { get; set; }
        public decimal? DebitLine { get; set; }
        public string Notes { get; set; }
        public string U_BPP_BPAT { get; set; } 
        public string U_BPP_BPTD { get; set; } 
        public string U_BPP_BPTP { get; set; } 
        public string U_BPP_BPNO { get; set; } 
        public string U_BPP_BPAP { get; set; } 
        public string U_BPP_BPAM { get; set; } 
        public string U_FIB_Divi { get; set; } 
        public string U_FIB_Sector { get; set; } 
        public string ValidFor { get; set; } 
        public string U_FIB_Email2 { get; set; } 
        public string U_FIB_Email3 { get; set; } 

        public List<BPAddressesUpdateEntity> Addresses { get; set; }
        public List<BPContactEmployeesUpdateEntity> ContactEmployees { get; set; }
    }

    public class BPAddressesUpdateEntity
    {
        public string AddressName { get; set; }
        public string AddressType { get; set; } 
        public string Street { get; set; }
        public string Block { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string BuildingFloorRoom { get; set; }
        public string GlblLocNum { get; set; }
        public string TaxCode { get; set; }
    }

    public class BPContactEmployeesUpdateEntity
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Position { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string MobilePhone { get; set; }
        public string E_Mail { get; set; }
    }
}
