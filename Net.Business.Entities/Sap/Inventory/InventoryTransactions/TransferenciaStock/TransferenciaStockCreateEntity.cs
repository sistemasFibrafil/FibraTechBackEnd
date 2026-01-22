using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class TransferenciaStockCreateEntity
    {
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

        public string U_FIB_TIP_TRANS { get; set; }
        public string U_FIB_TIPDOC_TRA { get; set; }
        public string U_BPP_MDRT { get; set; }
        public string U_BPP_MDNT { get; set; }
        public string U_BPP_MDVC { get; set; }

        public string U_FIB_TIPDOC_COND { get; set; }
        public string U_FIB_NUMDOC_COD { get; set; }
        public string U_FIB_NOM_COND { get; set; }
        public string U_FIB_APE_COND { get; set; }
        public string U_BPP_MDFN { get; set; }
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
        public List<TransferenciaStock1CreateEntity> Lines { get; set; } = new List<TransferenciaStock1CreateEntity>();
        public List<PickingUpdateEntity> PickingLines { get; set; } = new List<PickingUpdateEntity>();
    }

    public class TransferenciaStock1CreateEntity
    {
        public int? BaseEntry { get; set; } = 0;
        public int BaseLine { get; set; }
        public int BaseType { get; set; }
        public string U_FIB_FromPkg { get; set; }
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
    }

    public class PickingUpdateEntity
    {
        public int DocEntry { get; set; }
        public int U_BaseEntry { get; set; }
        public int U_BaseLine { get; set; }
        public string U_Status { get; set; }
        public int U_UsrUpdate { get; set; }
    }
}
