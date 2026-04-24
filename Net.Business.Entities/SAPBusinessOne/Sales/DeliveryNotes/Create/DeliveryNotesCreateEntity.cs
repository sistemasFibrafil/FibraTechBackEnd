using System;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Update;
namespace Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Create
{
    public class DeliveryNotesCreateEntity
    {
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string? DocType { get; set; }

        public string? U_BPP_MDTD { get; set; }
        public string? U_BPP_MDSD { get; set; }
        public string? U_BPP_MDCD { get; set; }


        /// <summary>
        /// SOCIO DE NEGOCIO
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


        // SALES EMPLOYEE
        public int SlpCode { get; set; }
        public double U_FIB_NBULTOS { get; set; }
        public double U_FIB_KG { get; set; }
        public string? U_NroOrden { get; set; }
        public string? U_OrdenCompra { get; set; }
        public string? Comments { get; set; }


        /// <summary>
        /// TOTALES
        /// </summary>
        public double DiscPrcnt { get; set; }
        public double DocTotal { get; set; }


        /// <summary>
        /// AUDITORIA
        /// </summary>
        public int U_UsrCreate { get; set; }

        public List<DeliveryNotes1CreateEntity> Lines { get; set; } = [];
        public List<DeliveryNotesPickingUpdateEntity> PickingLines { get; set; } = [];
    }
}
