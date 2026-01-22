using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class SolicitudTrasladoUpdateEntity
    {
        public int DocEntry { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string U_FIB_IsPkg { get; set; }
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
        public string U_FIB_TIP_TRAS { get; set; }
        public string U_BPP_MDMT { get; set; }
        public string U_BPP_MDTS { get; set; }
        public int? SlpCode { get; set; } = -1;
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public int? U_UsrUpdate { get; set; } = null;

        public List<SolicitudTraslado1UpdateEntity> Lines { get; set; } = new List<SolicitudTraslado1UpdateEntity>();
    }

    public class SolicitudTraslado1UpdateEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string LineStatus { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string U_tipoOpT12 { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQty { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; } = null;
        public int Record { get; set; } = 2;
    }
}
