using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class InvoicesEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ObjType { get; set; } = string.Empty;
        public string DocType { get; set; } = string.Empty;
        public string CANCELED { get; set; } = string.Empty;
        public string DocStatus { get; set; } = string.Empty;
        public string InvntSttus { get; set; } = string.Empty;


        /// <summary>
        /// Factura             : "DocSubType" = '--'
        /// Factura Exportación : "DocSubType" = 'IX'
        /// </summary>
        public string DocSubType { get; set; } = string.Empty;


        /// <summary>
        /// Factura         : "isIns" = 'N'
        /// Factura Reserva : "isIns" = 'Y'
        /// </summary>
        public string isIns { get; set; } = string.Empty;

        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public short DocTime { get; set; }
        public short UserSign { get; set; }


        /// <summary>
        /// SUNAT
        /// </summary>
        public string? U_BPP_MDTD { get; set; }
        public string? U_BPP_MDSD { get; set; }
        public string? U_BPP_MDCD { get; set; }



        /// <summary>
        /// PICKING
        /// </summary>
        public string? U_FIB_DocStPkg { get; set; }
        public string? U_FIB_IsPkg { get; set; }



        /// <summary>
        /// SOCIO DE NEGOCIOS
        /// </summary>
        public string CardCode { get; set; } = string.Empty;
        public string? CardName { get; set; }
        public short GroupCode { get; set; }
        public int? CntctCode { get; set; }
        public string? NumAtCard { get; set; }
        public string? DocCur { get; set; }
        public decimal DocRate { get; set; }



        /// <summary>
        /// LOGISTICA
        /// </summary>
        public string? PayToCode { get; set; }
        public string? Address { get; set; }
        public string? ShipToCode { get; set; }
        public string? Address2 { get; set; }



        /// <summary>
        /// FINANZAS
        /// </summary>
        public short GroupNum { get; set; }



        /// <summary>
        /// AGENCIA
        /// </summary>
        public string? U_BPP_MDCT { get; set; }
        public string? U_BPP_MDRT { get; set; }
        public string? U_BPP_MDNT { get; set; }
        public string? U_FIB_CODT { get; set; }
        public string? U_BPP_MDDT { get; set; }



        /// <summary>
        /// TRANSPORTISTA
        /// </summary>        
        // DATOS QUE SE LLENAN EN LA ORDEN DE VENTA
        public string? U_TipoFlete { get; set; }
        public int? U_ValorFlete { get; set; }
        public decimal? U_FIB_TFLETE { get; set; }
        public decimal? U_FIB_IMPSEG { get; set; }
        public string? U_FIB_PUERTO { get; set; }

        // DATOS QUE SE LLENAN EN LA ENTREGA
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



        // <summary>
        /// SALES EMPLOYEE
        /// </summary>
        public int? SlpCode { get; set; }
        public decimal? U_FIB_NBULTOS { get; set; }
        public decimal? U_FIB_KG { get; set; }
        public string? U_NroOrden { get; set; }
        public string? U_OrdenCompra { get; set; }
        public string? Comments { get; set; }


        /// <summary>
        /// TOTALES
        /// </summary>
        public decimal SubTotal { get; set; }
        public decimal? DiscPrcnt { get; set; }
        public decimal DiscSum { get; set; }
        public decimal DiscSumSy { get; set; }
        public decimal VatSum { get; set; }
        public decimal VatSumSy { get; set; }
        public decimal DocTotal { get; set; }
        public decimal DocTotalSy { get; set; }
        public decimal PaidSys { get; set; }


        // 🔗 1 → N (OINV → OUSR)
        public UsersEntity? Users { get; set; } = null;


        // 🔗 1 → N (OINV → OCRD)
        public BusinessPartnersEntity? BusinessPartners { get; set; } = null;


        // 🔗 1 → N (OINV → OCRN)
        public CurrencyCodesEntity? CurrencyCodes { get; set; } = null;


        // 🔗 1 → N (OINV → OSLP)
        public SalesPersonsEntity? SalesPersons { get; set; } = null;


        // 🔗 1 → N (OINV → OCTG)
        public PaymentTermsTypesEntity? PaymentTermsTypes { get; set; } = null;


        // 🔗 1 → N (OINV → INV1)
        public List<Invoices1Entity> Lines { get; set; } = new List<Invoices1Entity>();
    }
}
