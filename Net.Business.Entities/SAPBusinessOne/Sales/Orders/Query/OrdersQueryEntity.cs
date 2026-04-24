using System;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Entities;
using Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Query;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class OrdersQueryEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string? ObjType { get; set; }
        public string? DocType { get; set; }
        public string? Canceled { get; set; }
        public string? DocStatus { get; set; }
        public string? WddStatus { get; set; }

        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string? DocTime { get; set; }


        /// <summary>
        /// PICKING
        /// </summary>
        public string? U_FIB_DocStPkg { get; set; }
        public string? U_FIB_IsPkg { get; set; }


        /// <summary>
        /// SOCIO DE NEGOCIO
        /// </summary>
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? LicTradNum { get; set; }
        public short? GroupCode { get; set; }
        public string? GroupName { get; set; }
        public int? CntctCode { get; set; }
        public string? NumAtCard { get; set; }
        public string? DocCur { get; set; }
        public string? CurrName { get; set; }
        public List<CurrencyCodesEntity> CurrencyList { get; set; } = [];
        public decimal DocRate { get; set; }


        /// <summary>
        /// LOGISTICA
        /// </summary>
        public string? PayToCode { get; set; }
        public List<AddressesEntity> PayAddressList { get; set; } = [];
        public string? Address { get; set; }
        public string? ShipToCode { get; set; }
        public List<AddressesEntity> ShipAddressList { get; set; } = [];
        public string? Address2 { get; set; }


        /// <summary>
        /// FINANZAS
        /// </summary>
        public short GroupNum { get; set; }
        public string? PymntGroup { get; set; }


        /// <summary>
        /// AGENCIA
        /// </summary>
        public string? U_BPP_MDCT { get; set; }
        public string? U_BPP_MDRT { get; set; }
        public string? U_BPP_MDNT { get; set; }
        public string? U_FIB_CODT { get; set; }
        public List<AddressesEntity> AgencyAddressList { get; set; } = [];
        public string? U_BPP_MDDT { get; set; }

        /// <summary>
        /// EXPORTACION
        /// </summary>
        public string? U_TipoFlete { get; set; }
        public string? TipoFleteDescr { get; set; }
        public int? U_ValorFlete { get; set; }
        public decimal? U_FIB_TFLETE { get; set; }
        public decimal? U_FIB_IMPSEG { get; set; }
        public string? U_FIB_PUERTO { get; set; }
        public string? U_STR_FEMB { get; set; }


        /// <summary>
        /// OTROS
        /// </summary>
        public string? U_STR_TVENTA { get; set; }


        public int? SlpCode { get; set; }
        public string? SlpName { get; set; }
        public decimal? U_FIB_NBULTOS { get; set; }
        public decimal? U_FIB_KG { get; set; }
        public string? U_NroOrden { get; set; }
        public string? U_OrdenCompra { get; set; }
        public string? Comments { get; set; }

        /// <summary>
        /// TOTALES
        /// </summary>
        public decimal SubTotal { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal DiscSum { get; set; }
        public decimal VatSum { get; set; }
        public decimal DocTotal { get; set; }
        public decimal DocTotalSy { get; set; }


        // 🔗 1 → N (ORDR → OATC)
        public Attachments2QueryEntity? Attachments2 { get; set; } = null;


        // 🔗 1 → N (ORDR → RDR1)
        public List<Orders1QueryEntity> Lines { get; set; } = [];

        // 🔗 1 → N (ORDR → @FIB_OPKG)
        public List<PickingEntity> PickingLines { get; set; } = [];
    }

    public class Orders1QueryEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string? LineStatus { get; set; }
        public string? ObjType { get; set; }
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }

        public string? U_FIB_LinStPkg { get; set; }
        public string? U_FIB_FromPkg { get; set; }

        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? AcctCode { get; set; }
        public string? FormatCode { get; set; }
        public string? AcctName { get; set; }
        public string? WhsCode { get; set; }

        public string? UnitMsr { get; set; }
        public decimal? OnHand { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal Delivered { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; }
        public decimal? U_FIB_NBulto { get; set; }
        public decimal? U_FIB_PesoKg { get; set; }

        public string? Currency { get; set; }
        public decimal? PriceBefDi { get; set; }
        public decimal? DiscPrcnt { get; set; }
        public decimal Price { get; set; }
        public string? TaxCode { get; set; }
        public decimal? VatPrcnt { get; set; }
        public decimal? VatSum { get; set; }
        public string? U_tipoOpT12 { get; set; }
        public string? U_tipoOpT12Nam { get; set; }
        public decimal LineTotal { get; set; }

        // 🔗 N → 1 (PRQ1 → ChartOfAccounts)
        public ChartOfAccountsEntity ChartOfAccounts { get; set; } = null!;


        // 🔗 N → 1 (RDR1 → TipoOperacion)
        public OperationTypeEntity OperationType { get; set; } = null!;

        public int Record { get; set; } = 2;
    }
}
