using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class OrdersCreateEntity
    {
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string DocType { get; set; }

        public string U_FIB_DocStPkg { get; set; } = null;
        public string U_FIB_IsPkg { get; set; } = null;

        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int? CntctCode { get; set; } = 0;
        public string NumAtCard { get; set; }
        public string DocCur { get; set; } = null;
        public decimal DocRate { get; set; }

        public int GroupNum { get; set; }

        public string PayToCode { get; set; } = null;
        public string Address { get; set; } = null;
        public string ShipToCode { get; set; } = null;
        public string Address2 { get; set; } = null;

        public string U_BPP_MDCT { get; set; } = null;
        public string U_BPP_MDRT { get; set; } = null;
        public string U_BPP_MDNT { get; set; } = null;
        public string U_FIB_AgencyToCode { get; set; } = null;
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

        public decimal DiscPrcnt { get; set; } = 0;
        public decimal DiscSum { get; set; } = 0;
        public decimal VatSum { get; set; } = 0;
        public decimal DocTotal { get; set; } = 0;
        public int? U_UsrCreate { get; set; } = 0;
        public List<Orders1CreateEntity> Lines { get; set; } = new List<Orders1CreateEntity>();
    }

    public class Orders1CreateEntity
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string WhsCode { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; } = null;
        public string Currency { get; set; }
        public decimal PriceBefDi { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal Price { get; set; }
        public string TaxCode { get; set; }
        public decimal VatPrcnt { get; set; }
        public decimal VatSum { get; set; }
        public string U_tipoOpT12 { get; set; } = null;
        public decimal LineTotal { get; set; }
    }
}
