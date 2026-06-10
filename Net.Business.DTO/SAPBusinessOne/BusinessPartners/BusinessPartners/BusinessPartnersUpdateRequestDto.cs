using System.Linq;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Addresses.Update;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Addresses.Update;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.ContactEmployees.Update;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.ContactEmployees.Update;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class BusinessPartnersUpdateRequestDto
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
        public List<ContactEmployeesUpdateRequestDto> ContactEmployeesLines { get; set; } = [];

        public string? BillToDef { get; set; }
        public string? Address { get; set; }
        public string? ShipToDef { get; set; }
        public string? MailAddres { get; set; }
        public List<AddressesUpdateRequestDto> AddressesLines { get; set; } = [];

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


        public BusinessPartnersUpdateEntity ReturnValue()
        {
            return new BusinessPartnersUpdateEntity
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
                .Select(l => new ContactEmployeesUpdateEntity
                {
                    CntctCode = l.CntctCode,
                    Name = l.Name,
                    FirstName = l.FirstName,
                    MiddleName = l.MiddleName,
                    LastName = l.LastName,
                    Title = l.Title,
                    Position = l.Position,
                    Address = l.Address,
                    Phone1 = l.Phone1,
                    Phone2 = l.Phone2,
                    MobilePhone = l.MobilePhone,
                    E_MailL = l.E_MailL,
                    Record = l.Record
                })
                .ToList() ?? [],

                BillToDef = BillToDef,
                Address = Address,
                ShipToDef = ShipToDef,
                MailAddres = MailAddres,
                AddressesLines = AddressesLines?
                .Select(l => new AddressesUpdateEntity
                {
                    LineNum = l.LineNum,
                    Address = l.Address,
                    AdresType = l.AdresType,
                    Street = l.Street,
                    Block = l.Block,
                    City = l.City,
                    ZipCode = l.ZipCode,
                    County = l.County,
                    State = l.State,
                    Country = l.Country,
                    BuildingFloorRoom = l.BuildingFloorRoom,
                    GlblLocNum = l.GlblLocNum,
                    TaxCode = l.TaxCode,
                    Record = l.Record
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
