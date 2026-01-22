using System;
namespace Net.Business.Entities.Sap
{
    public class PickingQueryEntity
    {
        public int DocEntry { get; set; }
        public string U_Status { get; set; }
        public string U_IsReleased { get; set; }
        public int? U_BaseEntry { get; set; } = null;
        public int? U_BaseNum { get; set; } = null;
        public int? U_BaseType { get; set; } = null;
        public int? U_BaseLine { get; set; } = null;
        public int? U_TrgetEntry { get; set; } = null;
        public int? U_TrgetDocNum { get; set; } = null;
        public int? U_TargetType { get; set; } = null;
        public int? U_TrgetLine { get; set; } = null;
        public DateTime? U_TrgetDocDate { get; set; } = null;
        public string U_TrgetNumber { get; set; } = null;
        public DateTime? U_DocDate { get; set; } = null;
        public DateTime? U_TaxDate { get; set; } = null;
        public DateTime? U_DocDueDate { get; set; } = null;
        public string U_CardCode { get; set; } = null;
        public string U_CardName { get; set; } = null;
        public string U_ItemCode { get; set; } = null;
        public string U_Dscription { get; set; } = null;
        public string U_CodeBar { get; set; } = null;
        public string U_FromWhsCod { get; set; } = null;
        public string U_WhsCode { get; set; } = null;
        public string U_tipoOpT12 { get; set; }
        public string U_tipoOpT12Nam { get; set; }
        public string U_UnitMsr { get; set; } = null;
        public decimal? U_Quantity { get; set; } = null;
        /// <summary>
        /// Peso en kg
        /// </summary>
        public decimal? U_WeightKg { get; set; } = null;
        /// <summary>
        /// Número de bulto
        /// </summary>
        public decimal? U_NumBulk { get; set; } = null;
        /// <summary>
        /// Es para indentificar se es un picking o un solicitud de traslado
        /// </summary>
        public string U_FIB_IsPkg { get; set; }
        public string U_Contenedor { get; set; }
        /// <summary>
        /// Cantidad penddiente
        /// </summary>
        public decimal U_OpenQty { get; set; }
        /// <summary>
        /// Cantidad picada
        /// </summary>
        public decimal U_QtyPkg { get; set; }
        /// <summary>
        /// Cantidad pendiente por leer
        /// </summary>
        public decimal U_EngQtyPkg { get; set; }
        /// <summary>
        /// Cantidad despachada leida
        /// </summary>
        public decimal U_DisQtyPkg { get; set; }
        public decimal U_FIB_OpQtyPkg { get; set; }
        public int? U_UsrCreate { get; set; }
        public int? U_UsrUpdate { get; set; }
    }
}
