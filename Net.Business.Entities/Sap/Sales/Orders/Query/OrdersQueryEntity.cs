using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class OrdersQueryEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ObjType { get; set; }
        public string DocType { get; set; }
        public string DocStatus { get; set; } = null;

        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        
        public string U_FIB_DocStPkg { get; set; } = null;
        public string U_FIB_IsPkg { get; set; } = null;

        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int? CntctCode { get; set; } = 0;
        public string NumAtCard { get; set; }
        public string DocCur { get; set; } = null;
        public List<CurrencyCodesEntity> CurrencyList { get; set; } = new List<CurrencyCodesEntity>();
        public decimal DocRate { get; set; }


        public string PayToCode { get; set; } = null;
        public List<DireccionEntity> PayAddressList { get; set; } = new List<DireccionEntity>();
        public string Address { get; set; } = null;
        public string ShipToCode { get; set; } = null;
        public List<DireccionEntity> ShipAddressList { get; set; } = new List<DireccionEntity>();
        public string Address2 { get; set; } = null;

        public Int16 GroupNum { get; set; }

        public string U_BPP_MDCT { get; set; } = null;
        public string U_BPP_MDRT { get; set; } = null;
        public string U_BPP_MDNT { get; set; } = null;
        public string U_FIB_AgencyToCode { get; set; } = null;
        public List<DireccionEntity> AgencyAddressList { get; set; } = new List<DireccionEntity>();
        public string U_BPP_MDDT { get; set; } = null;

        public string U_TipoFlete { get; set; } = null;
        public int? U_ValorFlete { get; set; } = 0;
        public decimal? U_FIB_TFLETE { get; set; } = 0;
        public decimal? U_FIB_IMPSEG { get; set; } = 0;
        public string U_FIB_PUERTO { get; set; } = null;

        public string U_STR_TVENTA { get; set; } = null;

        public int? SlpCode { get; set; } = -1;
        public string U_OrdenCompra { get; set; } = null;
        public string Comments { get; set; } = null;

        public decimal SubTotal { get; set; } = 0;
        public decimal DiscPrcnt { get; set; } = 0;
        public decimal DiscSum { get; set; } = 0;
        public decimal VatSum { get; set; } = 0;
        public decimal DocTotal { get; set; } = 0;


        // 🔗 1 → N (ORDR → RDR1)
        public ICollection<Orders1QueryEntity> Lines { get; set; } = new List<Orders1QueryEntity>();
    }

    public class Orders1QueryEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string LineStatus { get; set; }
        public string ObjType { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string AcctCode { get; set; }
        public string FormatCode { get; set; }
        public string AcctName { get; set; }
        public string WhsCode { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; } = null;
        public decimal? OnHand { get; set; } = 0;
        public string Currency { get; set; }
        public decimal PriceBefDi { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal Price { get; set; }
        public string TaxCode { get; set; }
        public decimal VatPrcnt { get; set; }
        public decimal VatSum { get; set; }
        public string U_tipoOpT12 { get; set; } = null;
        public string U_tipoOpT12Nam { get; set; } = null;
        public decimal LineTotal { get; set; }

        // 🔗 N → 1 (PRQ1 → ChartOfAccounts)
        public ChartOfAccountsEntity ChartOfAccounts { get; set; }


        // 🔗 N → 1 (RDR1 → TipoOperacion)
        public TipoOperacionEntity TipoOperacion { get; set; }

        public int Record { get; set; } = 2;
    }
}
