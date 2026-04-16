using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class InvoicesCreateEntity
    {
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string? ReserveInvoice { get; set; }
        public string? DocType { get; set; }


        /// <summary>
        /// SUNAT
        /// </summary>
        public string? U_BPP_MDTD { get; set; }
        public string? U_BPP_MDSD { get; set; }
        public string? U_BPP_MDCD { get; set; }

        /// <summary>
        /// Picking
        /// </summary>
        public string? U_FIB_IsPkg { get; set; }


        /// <summary>
        /// SOCIO DE NEGOCIOS
        /// </summary>
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public int CntctCode { get; set; }
        public string? NumAtCard { get; set; }
        public string? DocCur { get; set; }
        public double DocRate { get; set; }

        
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
        public int GroupNum { get; set; }


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
        public int U_ValorFlete { get; set; }
        public double U_FIB_TFLETE { get; set; }
        public double U_FIB_IMPSEG { get; set; }
        public string? U_FIB_PUERTO { get; set; }


        /// <summary>
        /// OTROS
        /// </summary>
        public string? U_STR_TVENTA { get; set; }


        /// <summary>
        /// SALES EMPLOYEE
        /// </summary>
        public int SlpCode { get; set; }
        public string? U_NroOrden { get; set; }
        public string? U_OrdenCompra { get; set; }
        public string? Comments { get; set; }


        /// <summary>
        /// TOTALES
        /// </summary>
        public double DiscPrcnt { get; set; }
        public double DocTotal { get; set; }

        public int U_UsrCreate { get; set; }
        public List<Invoices1CreateEntity> Lines { get; set; } = new List<Invoices1CreateEntity>();
    }
}
