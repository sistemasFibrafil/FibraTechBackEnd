using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class SolicitudTrasladoQueryEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ObjType { get; set; }
        public string DocStatus { get; set; } = null;
        public string U_FIB_DocStPkg { get; set; } = null;
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string U_FIB_IsPkg { get; set; } = null;
        public string CardCode { get; set; } = null;
        public string CardName { get; set; } = null;
        public int? CntctCode { get; set; } = 0;
        public string Address { get; set; } = null;
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
        public string U_FIB_TIP_TRAS { get; set; } = null;
        public string U_BPP_MDMT { get; set; } = null;
        public string U_BPP_MDTS { get; set; } = null;
        public int? SlpCode { get; set; } = -1;
        public decimal U_FIB_NBULTOS { get; set; } = 0;
        public decimal U_FIB_KG { get; set; } = 0;
        public string JrnlMemo { get; set; } = null;
        public string Comments { get; set; } = null;
        public List<SolicitudTraslado1QueryEntity> Lines { get; set; } = new List<SolicitudTraslado1QueryEntity>();
        public List<PickingEntity> PickingLines { get; set; } = new List<PickingEntity>();
    }

    public class SolicitudTraslado1QueryEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string LineStatus { get; set; }
        public string ObjType { get; set; }
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; } = null;
        public int? BaseLine { get; set; } = null;
        public string U_FIB_LinStPkg { get; set; } = null;
        public string U_FIB_FromPkg { get; set; } = null;
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string U_tipoOpT12 { get; set; } = null;
        public string U_tipoOpT12Nam { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; } = null;
        public decimal OpenQty { get; set; }
        public decimal? U_FIB_NBulto { get; set; } = null;
        public decimal? U_FIB_PesoKg { get; set; } = null;
        public int Record { get; set; } = 2;
    }
}
