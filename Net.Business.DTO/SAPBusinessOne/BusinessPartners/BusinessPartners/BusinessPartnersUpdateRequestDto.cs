using System;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne;

namespace Net.Business.DTO.SAPBusinessOne
{
    public class BusinessPartnersUpdateRequestDto
    {
        public string CardCode { get; set; } = string.Empty;
        public string? CardName { get; set; }
        public string? CardType { get; set; }
        public int? GroupCode { get; set; }
        public string? LicTradNum { get; set; }
        public string? Phone1 { get; set; }
        public string? Cellular { get; set; }
        public string? EmailAddress { get; set; }
        public string? Address { get; set; }
        public int? SlpCode { get; set; }
        public int? GroupNum { get; set; }
        public decimal? CreditLine { get; set; }
        public string? Currency { get; set; }
        public string? CntctPrsn { get; set; }
        public decimal? DebitLine { get; set; }
        public string? Notes { get; set; }
        public string? U_BPP_BPAT { get; set; }
        public string? U_BPP_BPTD { get; set; }
        public string? U_BPP_BPTP { get; set; }
        public string? U_BPP_BPNO { get; set; }
        public string? U_BPP_BPAP { get; set; }
        public string? U_BPP_BPAM { get; set; }
        public string? U_FIB_Divi { get; set; }
        public string? U_FIB_Sector { get; set; }
        public string? ValidFor { get; set; }
        public string? U_FIB_Email2 { get; set; }
        public string? U_FIB_Email3 { get; set; }

        public List<BPAddressesUpdateRequestDto>? Addresses { get; set; }
        public List<BPContactEmployeesUpdateRequestDto>? ContactEmployees { get; set; }

        public BusinessPartnersUpdateEntity ReturnValue()
        {
            var entity = new BusinessPartnersUpdateEntity
            {
                CardCode = this.CardCode,
                CardName = this.CardName,
                CardType = this.CardType,
                GroupCode = this.GroupCode,
                LicTradNum = this.LicTradNum,
                Phone1 = this.Phone1,
                Cellular = this.Cellular,
                EmailAddress = this.EmailAddress,
                Address = this.Address,
                SlpCode = this.SlpCode,
                GroupNum = this.GroupNum,
                CreditLine = this.CreditLine ?? 0,
                Currency = this.Currency,
                CntctPrsn = this.CntctPrsn,
                DebitLine = this.DebitLine,
                Notes = this.Notes,
                U_BPP_BPAT = this.U_BPP_BPAT,
                U_BPP_BPTD = this.U_BPP_BPTD,
                U_BPP_BPTP = this.U_BPP_BPTP,
                U_BPP_BPNO = this.U_BPP_BPNO,
                U_BPP_BPAP = this.U_BPP_BPAP,
                U_BPP_BPAM = this.U_BPP_BPAM,
                U_FIB_Divi = this.U_FIB_Divi,
                U_FIB_Sector = this.U_FIB_Sector,
                ValidFor = this.ValidFor,
                U_FIB_Email2 = this.U_FIB_Email2,
                U_FIB_Email3 = this.U_FIB_Email3,
                Addresses = new List<BPAddressesUpdateEntity>(),
                ContactEmployees = new List<BPContactEmployeesUpdateEntity>()
            };

            if (this.Addresses != null)
            {
                foreach (var addr in this.Addresses)
                {
                    entity.Addresses.Add(new BPAddressesUpdateEntity
                    {
                        AddressName = addr.AddressName ?? string.Empty,
                        AddressType = addr.AddressType ?? "B",
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
                    });
                }
            }

            if (this.ContactEmployees != null)
            {
                foreach (var contact in this.ContactEmployees)
                {
                    entity.ContactEmployees.Add(new BPContactEmployeesUpdateEntity
                    {
                        Name = contact.Name ?? string.Empty,
                        FirstName = contact.FirstName,
                        MiddleName = contact.MiddleName,
                        LastName = contact.LastName,
                        Title = contact.Title,
                        Position = contact.Position,
                        Address = contact.Address,
                        Phone1 = contact.Phone1,
                        Phone2 = contact.Phone2,
                        MobilePhone = contact.MobilePhone,
                        E_Mail = contact.E_Mail
                    });
                }
            }

            return entity;
        }
    }

    public class BPAddressesUpdateRequestDto
    {
        public string? AddressName { get; set; }
        public string? AddressType { get; set; } 
        public string? Street { get; set; }
        public string? Block { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? County { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? BuildingFloorRoom { get; set; }
        public string? GlblLocNum { get; set; }
        public string? TaxCode { get; set; }
    }

    public class BPContactEmployeesUpdateRequestDto
    {
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Title { get; set; }
        public string? Position { get; set; }
        public string? Address { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? MobilePhone { get; set; }
        public string? E_Mail { get; set; }
    }
}
