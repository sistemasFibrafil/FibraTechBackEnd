using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class TransferenciaStockEntity
    {
        public int DocEntry { get; set; } = 0;
        public string ObjType { get; set; } = "67";
        public int DocNum { get; set; } = 0;
        public string DocStatus { get; set; } = null;
        public string U_FIB_FromPkg { get; set; } = null;
        public string U_BPP_MDTD { get; set; }
        public string U_BPP_MDSD { get; set; }
        public string U_BPP_MDCD { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int? CntctCode { get; set; } = 0;
        public string Address { get; set; } = null;
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
        /// <summary>
        /// Tipo de transportista
        /// </summary>
        public string U_FIB_TIP_TRANS { get; set; }
        /// <summary>
        /// Tipo de documento del transportista
        /// </summary>
        public string U_FIB_TIPDOC_TRA { get; set; }
        /// <summary>
        /// Número de documento transportista
        /// </summary>
        public string U_BPP_MDRT { get; set; }
        /// <summary>
        /// Nombre transportista
        /// </summary>
        public string U_BPP_MDNT { get; set; }
        /// <summary>
        /// Placa del vehículo del transportista
        /// </summary>
        public string U_BPP_MDVC { get; set; }
        /// <summary>
        /// Tipo de documento del conductor
        /// </summary>
        public string U_FIB_TIPDOC_COND { get; set; }
        /// <summary>
        /// Número de documento del conductor
        /// </summary>
        public string U_FIB_NUMDOC_COD { get; set; }
        /// <summary>
        /// Nombre del conductor
        /// </summary>
        public string U_FIB_NOM_COND { get; set; }
        /// <summary>
        /// Apellido del conductor
        /// </summary>
        public string U_FIB_APE_COND { get; set; }
        /// <summary>
        /// Nombre completo del conductor
        /// </summary>
        public string U_BPP_MDFN { get; set; }
        /// <summary>
        /// Licencia de conducir del conductor
        /// </summary>
        public string U_BPP_MDFC { get; set; }

        public string U_FIB_TIP_TRAS { get; set; }
        public string U_BPP_MDMT { get; set; }
        public string U_BPP_MDTS { get; set; }

        public int? SlpCode { get; set; } = -1;
        public decimal? U_FIB_NBULTOS { get; set; } = 0;
        public decimal? U_FIB_KG { get; set; } = 0;
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public int? U_UsrCreate { get; set; } = null;
        public int? U_UsrUpdate { get; set; } = null;


        // 🔗 1 → N (OWTR → WTR1)
        public ICollection<TransferenciaStock1Entity> Lines { get; set; } = new List<TransferenciaStock1Entity>();
    }

    public class TransferenciaStock1Entity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ObjType { get; set; } = "67";
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; } = null;
        public int? BaseLine { get; set; } = null;
        public string U_FIB_FromPkg { get; set; } = null;
        public string LineStatus { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string U_tipoOpT12 { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal? U_FIB_NBulto { get; set; } = 0;
        public decimal? U_FIB_PesoKg { get; set; } = 0;


        // 🔗 N → 1 (WTR1 → TipoOperacion)
        public TipoOperacionEntity TipoOperacion { get; set; }
    }

    public class TransferenciaStockFormatoEntity
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int DocNum { get; set; }
        public string Codigo { get; set; }
        public string Version { get; set; }
        public string Vigencia { get; set; }
        public DateTime TaxDate { get; set; }
        public string SedeOrigen { get; set; }
        public string SedeDestino { get; set; }
        public string TipoTraslado { get; set; }
        public string Comments { get; set; }
    }

    public class TransferenciaStock1FormatoEntity
    {
        public int Line { get; set; }
        public int NumSolicitud { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public decimal Quantity { get; set; }
    }
}
