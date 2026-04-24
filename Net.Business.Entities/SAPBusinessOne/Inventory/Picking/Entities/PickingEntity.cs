using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities
{
    public class PickingEntity
    {
        public int DocEntry { get; set; }
        public string? U_Status { get; set; }
        public string? U_IsReleased { get; set; }
        public int? U_BaseEntry { get; set; }
        public int? U_BaseNum { get; set; }
        public int? U_BaseType { get; set; }
        public int? U_BaseLine { get; set; }
        public int? U_TrgetEntry { get; set; }
        public int? U_TargetType { get; set; }
        public int? U_TrgetLine { get; set; }
        public DateTime? U_DocDate { get; set; }
        public DateTime? U_TaxDate { get; set; }
        public DateTime? U_DocDueDate { get; set; }
        public string? U_CardCode { get; set; }
        public string? U_CardName { get; set; }
        public string? U_ItemCode { get; set; }
        public string? U_Dscription { get; set; }
        public string? U_CodeBar { get; set; }
        public string? U_FromWhsCod { get; set; }
        public string? U_WhsCode { get; set; }
        public string? U_UnitMsr { get; set; }
        public decimal? U_Quantity { get; set; }
        /// <summary>
        /// Número de bulto
        /// </summary>
        public decimal? U_NumBulk { get; set; }
        /// <summary>
        /// Peso en kg
        /// </summary>
        public decimal? U_WeightKg { get; set; }
        public int? U_UsrCreate { get; set; }
        public int? U_UsrUpdate { get; set; }
        public int? U_UsrRelease { get; set; }
    }

    public class PickingCopyToFindEntity
    {
        public int U_BaseEntry { get; set; }
        public int U_BaseType { get; set; }
        public List<PickingCopyTo1FindEntity> Lines { get; set; } = new List<PickingCopyTo1FindEntity>();
    }

    public class PickingCopyTo1FindEntity
    {
        public int U_BaseEntry { get; set; }
        public int U_BaseType { get; set; }
        public int U_BaseLine { get; set; }
        public string? U_FIB_IsPkg { get; set; }
    }
}
