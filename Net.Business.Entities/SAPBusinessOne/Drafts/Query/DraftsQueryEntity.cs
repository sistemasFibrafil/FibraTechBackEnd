using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Drafts.Query
{
    public class DraftsQueryEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string? ObjType { get; set; }
        public string? DocType { get; set; }
        public string? DocStatus { get; set; }
        public string? WddStatus { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? DocTime { get; set; }


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


        // 🔗 1 → N (ODRF → RDR1)
        public List<DraftsLinesQueryEntity> Lines { get; set; } = new List<DraftsLinesQueryEntity>();
    }
}
