using System.Linq;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Addresses.Create;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Addresses.Create;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.ContactEmployees.Create;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.ContactEmployees.Create;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class BusinessPartnersCreateRequestDto
    {
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? CardType { get; set; }
        public int GroupCode { get; set; }
        public string? LicTradNum { get; set; }
        public string? Currency { get; set; }

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
        public string? Notes { get; set; }

        public string? CntctPrsn { get; set; }
        public List<ContactEmployeesCreateRequestDto> ContactEmployeesLines { get; set; } = [];        

        public string? BillToDef { get; set; }
        public string? Address { get; set; }
        public string? ShipToDef { get; set; }
        public string? MailAddres { get; set; }
        public List<AddressesCreateRequestDto> AddressesLines { get; set; } = [];

        public int GroupNum { get; set; }
        public short ListNum { get; set; }
        public decimal CreditLine { get; set; }
        public decimal DebitLine { get; set; }

        public string? U_BPP_BPAT { get; set; }
        public string? U_BPP_BPNO { get; set; }
        public string? U_BPP_BPAP { get; set; }
        public string? U_BPP_BPAM { get; set; }
        public string? U_FIB_EMAIL2 { get; set; }
        public string? U_FIB_EMAIL3 { get; set; }


        public BusinessPartnersCreateEntity ReturnValue()
        {
            return new BusinessPartnersCreateEntity
            {
                CardCode = CardCode,
                CardName = CardName,
                CardType = CardType,
                GroupCode = GroupCode,
                LicTradNum = LicTradNum,
                Currency = Currency,

                U_BPP_BPTP = U_BPP_BPTP,
                U_BPP_BPTD = U_BPP_BPTD,
                U_FIB_Divi = U_FIB_Divi,
                U_FIB_Sector = U_FIB_Sector,

                Phone1 = Phone1,
                Phone2 = Phone2,
                Cellular = Cellular,
                Email = Email,
                ValidFor = ValidFor,
                SlpCode = SlpCode,
                Notes = Notes,

                CntctPrsn = CntctPrsn,
                ContactEmployeesLines = ContactEmployeesLines?
                .Select(contact => new ContactEmployeesCreateEntity
                {
                    Name = contact.Name,
                    FirstName = contact.FirstName,
                    MiddleName = contact.MiddleName,
                    LastName = contact.LastName,
                    Title = contact.Title,
                    Position = contact.Position,
                    Address = contact.Address,
                    Phone1 = contact.Phone1,
                    Phone2 = contact.Phone2,
                    MobilePhone = contact.MobilePhone,
                    E_MailL = contact.E_MailL
                })
                .ToList() ?? [],

                BillToDef = BillToDef,
                Address = Address,
                ShipToDef = ShipToDef,
                MailAddres = MailAddres,
                AddressesLines = AddressesLines?
                .Select(addr => new AddressesCreateEntity
                {
                    Address = addr.Address,
                    AdresType = addr.AdresType,
                    Street = addr.Street,
                    Block = addr.Block,
                    City = addr.City,
                    ZipCode = addr.ZipCode,
                    County = addr.County,
                    State = addr.State,
                    Country = addr.Country,
                    BuildingFloorRoom = addr.BuildingFloorRoom,
                    GlblLocNum = addr.GlblLocNum,
                    TaxCode = addr.TaxCode
                })
                .ToList() ?? [],

                GroupNum = GroupNum,
                ListNum = ListNum,
                CreditLine = CreditLine,
                DebitLine = DebitLine,

                U_BPP_BPAT = U_BPP_BPAT,
                U_FIB_EMAIL2 = U_FIB_EMAIL2,
                U_FIB_EMAIL3 = U_FIB_EMAIL3,
                U_BPP_BPNO = U_BPP_BPNO,
                U_BPP_BPAP = U_BPP_BPAP,
                U_BPP_BPAM = U_BPP_BPAM
            };
        }
    }
}
