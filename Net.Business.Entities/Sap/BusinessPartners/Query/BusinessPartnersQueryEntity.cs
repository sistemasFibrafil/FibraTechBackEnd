using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class BusinessPartnersQueryEntity
    {
        public string CardCode { get; set; }
        public string LicTradNum { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string Currency { get; set; }
        public List<CurrencyCodesEntity> CurrencyList { get; set; } = new List<CurrencyCodesEntity>();
        public int SlpCode { get; set; }
        public int CntctCode { get; set; }
        public string CntctPrsn { get; set; }
        public string BillToDef { get; set; }
        public List<DireccionEntity> PayAddressList { get; set; } = new List<DireccionEntity>();
        public string Address { get; set; }
        public string ShipToDef { get; set; }
        public List<DireccionEntity> ShipAddressList { get; set; } = new List<DireccionEntity>();
        public string Address2 { get; set; }
        public string U_BPP_BPAT { get; set; }
        public Int16 GroupNum { get; set; }
        public Int16 ListNum { get; set; }
    }
}
