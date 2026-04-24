using System;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class DeliveryNotesQueryEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string? ObjType { get; set; }
        public string? DocType { get; set; }
        public string? Canceled { get; set; }
        public string? DocStatus { get; set; }

        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string? DocTime { get; set; }
        public string? UserName { get; set; }

        public string? U_FIB_FromPkg { get; set; }

        public string? U_BPP_MDTD { get; set; }
        public string? U_BPP_MDSD { get; set; }
        public string? U_BPP_MDCD { get; set; }


        /// <summary>
        /// SOCIO DE NEGOCIO
        /// </summary>
        public string? CardCode { get; set; }
        public string? LicTradNum { get; set; }
        public string? CardName { get; set; }
        public short? GroupCode { get; set; }
        public string? GroupName { get; set; }
        public string? Phone1 { get; set; }
        public int? CntctCode { get; set; }
        public string? NumAtCard { get; set; }
        public string? DocCur { get; set; }
        public List<CurrencyCodesEntity> CurrencyList { get; set; } = new List<CurrencyCodesEntity>();
        public decimal DocRate { get; set; }


        /// <summary>
        /// LOGISTICA
        /// </summary>
        public string? PayToCode { get; set; }
        public List<AddressesEntity> PayAddressList { get; set; } = new List<AddressesEntity>();
        public string? Address { get; set; }
        public string? ShipToCode { get; set; }
        public List<AddressesEntity> ShipAddressList { get; set; } = new List<AddressesEntity>();
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
        public List<AddressesEntity> AgencyAddressList { get; set; } = new List<AddressesEntity>();
        public string? U_BPP_MDDT { get; set; }

        /// <summary>
        /// TRANSPORTISTA
        /// </summary>
        public string? U_FIB_TIP_TRANS { get; set; }
        public string? U_FIB_COD_TRA { get; set; }
        public string? U_FIB_TIPDOC_TRA { get; set; }
        public string? U_FIB_RUC_TRANS2 { get; set; }
        public string? U_FIB_TRANS2 { get; set; }
        public string? U_BPP_MDVC { get; set; }
        public string? U_FIB_TIPDOC_COND { get; set; }
        public string? U_FIB_NUMDOC_COD { get; set; }
        public string? U_FIB_NOM_COND { get; set; }
        public string? U_FIB_APE_COND { get; set; }
        public string? U_BPP_MDFN { get; set; }
        public string? U_BPP_MDFC { get; set; }


        /// <summary>
        /// EXPORTACION
        /// </summary>
        public string? U_RUCDestInter { get; set; }
        public string? U_DestGuiaInter { get; set; }
        public string? U_DireccDestInter { get; set; }
        public string? U_STR_NCONTENEDOR { get; set; }
        public string? U_STR_NPRESCINTO { get; set; }
        public string? U_FIB_NPRESCINTO2 { get; set; }
        public string? U_FIB_NPRESCINTO3 { get; set; }
        public string? U_FIB_NPRESCINTO4 { get; set; }


        /// <summary>
        /// OTROS
        /// </summary>
        public string? U_STR_TVENTA { get; set; }
        public string? U_BPP_MDMT { get; set; }
        public string? U_BPP_MDOM { get; set; }


        /// <summary>
        /// SALES EMPLOYEE
        /// </summary>
        public int? SlpCode { get; set; } = -1;
        public string? SlpName { get; set; }
        public decimal U_FIB_NBULTOS { get; set; } = 0;
        public decimal U_FIB_KG { get; set; } = 0;
        public string? U_OrdenCompra { get; set; }
        public string? U_NroOrden { get; set; }
        public string? Comments { get; set; }


        /// <summary>
        /// TOTALES
        /// </summary>
        public decimal SubTotal { get; set; } = 0;
        public decimal DiscPrcnt { get; set; } = 0;
        public decimal DiscSum { get; set; } = 0;
        public decimal VatSum { get; set; } = 0;
        public decimal DocTotal { get; set; } = 0;
        public decimal DocTotalSy { get; set; } = 0;


        // 🔗 1 → N (ODLN → DLN1)
        public List<DeliveryNotes1QueryEntity> Lines { get; set; } = new List<DeliveryNotes1QueryEntity>();

        // 🔗 1 → N (ODLN → @FIB_OPKG)
        public List<PickingEntity> PickingLines { get; set; } = new List<PickingEntity>();
    }

    public class DeliveryNotes1QueryEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string? LineStatus { get; set; }
        public string? ObjType { get; set; }
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; } = null;
        public int? BaseLine { get; set; } = null;

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
        public decimal? U_FIB_NBulto { get; set; }
        public decimal? U_FIB_PesoKg { get; set; }

        public string? Currency { get; set; }
        public decimal PriceBefDi { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal Price { get; set; }
        public string? TaxCode { get; set; }
        public decimal VatPrcnt { get; set; }
        public decimal VatSum { get; set; }
        public string? U_tipoOpT12 { get; set; }
        public string? U_tipoOpT12Nam { get; set; }
        public decimal LineTotal { get; set; }


        // 🔗 N → 1 (DLN1 → OACT)
        public ChartOfAccountsEntity ChartOfAccounts { get; set; } = null!;


        // 🔗 N → 1 (DLN1 → TipoOperacion)
        public OperationTypeEntity OperationType { get; set; } = null!;
    }
}
