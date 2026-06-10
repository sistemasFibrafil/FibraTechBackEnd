using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Addresses.Create;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.ContactEmployees.Create;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class BusinessPartnersCreateEntity
    {
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? CardType { get; set; }
        public int GroupCode { get; set; }
        public string? LicTradNum { get; set; }
        public string? Currency { get; set; }

        public string? U_BPP_BPTP { get; set; } // Tipo Persona
        public string? U_BPP_BPTD { get; set; } // Tipo de Documento
        public string? U_FIB_Divi { get; set; } // División de Negocio
        public string? U_FIB_Sector { get; set; } // Sector / Tipo Cliente


        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? Cellular { get; set; }
        public string? Email { get; set; }
        public string? ValidFor { get; set; } // Activo (Y/N)
        public int SlpCode { get; set; }
        public string? Notes { get; set; }

        public string? CntctPrsn { get; set; }
        public List<ContactEmployeesCreateEntity> ContactEmployeesLines { get; set; } = [];

        public string? BillToDef { get; set; }
        public string? Address { get; set; }
        public string? ShipToDef { get; set; }
        public string? MailAddres { get; set; }
        public List<AddressesCreateEntity> AddressesLines { get; set; } = [];

        public int GroupNum { get; set; }
        public short ListNum { get; set; }
        public decimal CreditLine { get; set; }
        public decimal DebitLine { get; set; }

        public string? U_BPP_BPAT { get; set; } // Acreedor de Transporte
        public string? U_FIB_EMAIL2 { get; set; } // Email 2
        public string? U_FIB_EMAIL3 { get; set; } // Email 3
        public string? U_BPP_BPNO { get; set; } // Nombres (Concatenado para Persona Natural)
        public string? U_BPP_BPAP { get; set; } // Apellido Paterno
        public string? U_BPP_BPAM { get; set; } // Apellido Materno

    }
}
