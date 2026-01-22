using System;
using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class SolicitudTrasladoCreateEntity
    {
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string U_FIB_DocStPkg { get; set; } = null;
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
        public int? U_UsrCreate { get; set; } = null;

        public List<SolicitudTraslado1CreateEntity> Lines { get; set; } = new List<SolicitudTraslado1CreateEntity>();
    }

    public class SolicitudTraslado1CreateEntity
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string FromWhsCod { get; set; }
        public string WhsCode { get; set; }
        public string U_tipoOpT12 { get; set; } = null;
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; } = null;
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
    }
}
