using System;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Entities;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class OrdersEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string? ObjType { get; set; }
        public string? DocType { get; set; }
        public string? CANCELED { get; set; }
        public string? DocStatus { get; set; }
        public string? WddStatus { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public short DocTime { get; set; }
        public short UserSign { get; set; }

        public string? U_FIB_DocStPkg { get; set; }
        public string? U_FIB_IsPkg { get; set; }


        /// <summary>
        /// SOCIO DE NEGOCIO
        /// </summary>
        public string? CardCode { get; set; }
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
        /// EXPORTACION
        /// </summary>
        public string? U_TipoFlete { get; set; }
        public int? U_ValorFlete { get; set; }
        public decimal? U_FIB_TFLETE { get; set; }
        public decimal? U_FIB_IMPSEG { get; set; }
        public string? U_FIB_PUERTO { get; set; }
        public string? U_STR_FEMB { get; set; }

        /// <summary>
        /// OTROS
        /// </summary>
        public string? U_STR_TVENTA { get; set; }


        /// <summary>
        /// Anexos
        /// </summary>
        public int? AtcEntry { get; set; }


        /// <summary>
        /// SALES EMPLLOYEE
        /// </summary>
        public int SlpCode { get; set; }
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


        // 🔗 1 → N (ORDR → OCRD)
        public BusinessPartnersEntity? BusinessPartners { get; set; } = null;


        // 🔗 1 → N (ORDR → OCRN)
        public CurrencyCodesEntity? CurrencyCodes { get; set; } = null;


        // 🔗 1 → N (ORDR → OSLP)
        public SalesPersonsEntity? SalesPersons { get; set; } = null;


        // 🔗 1 → N (ORDR → OCTG)
        public PaymentTermsTypesEntity? PaymentTermsTypes { get; set; } = null;


        // 🔗 1 → N (ORDR → OCTG)
        public Attachments2Entity? Attachments2 { get; set; } = null;


        // 🔗 1 → N (ORDR → RDR1)
        public ICollection<Orders1Entity> Lines { get; set; } = [];
    }
}
