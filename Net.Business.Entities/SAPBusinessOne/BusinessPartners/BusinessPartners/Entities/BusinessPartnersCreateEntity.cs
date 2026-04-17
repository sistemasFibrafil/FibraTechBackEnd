using System;
using System.Collections.Generic;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class BusinessPartnersCreateEntity
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
        public string U_BPP_BPAT { get; set; } // Tipo de persona (Natural/Juridica, etc)
        public string U_BPP_BPTD { get; set; } // Tipo de Documento de Identidad (RUC, DNI, etc)
        public string U_BPP_BPTP { get; set; } // Tipo Persona ('TPJ', 'TPN')
        public string U_BPP_BPN1 { get; set; } // Primer Nombre
        public string U_BPP_BPN2 { get; set; } // Segundo Nombre
        public string U_BPP_BPNO { get; set; } // Nombres (Concatenado para Persona Natural)
        public string U_BPP_BPAP { get; set; } // Apellido Paterno
        public string U_BPP_BPAM { get; set; } // Apellido Materno
        public string U_FIB_Divi { get; set; } // División de Negocio
        public string U_FIB_Sector { get; set; } // Sector / Tipo Cliente

        public List<BPAddressesCreateEntity> Addresses { get; set; }
        public List<BPContactEmployeesCreateEntity> ContactEmployees { get; set; }
    }

    public class BPAddressesCreateEntity
    {
        public string AddressName { get; set; }
        public string AddressType { get; set; } // bo_BillTo = 'B', bo_ShipTo = 'S'
        public string Street { get; set; }
        public string Block { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string BuildingFloorRoom { get; set; }
        public string GlblLocNum { get; set; }
    }

    public class BPContactEmployeesCreateEntity
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
