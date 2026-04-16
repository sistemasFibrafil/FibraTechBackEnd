using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    /// <summary>
    /// Toma - inventario - repuestos
    /// </summary>
    public class TakeInventorySparePartsEntity
    {
        public int DocEntry { get; set; }
        public string? U_ItemCode { get; set; }
        public string? U_Dscription { get; set; }
        public string? U_WhsCode { get; set; }
        public string? U_UnitMsr { get; set; }
        public decimal U_OnHandPhy { get; set; }
        public decimal U_OnHandSys { get; set; }
        public decimal U_Difference { get; set; }
        public string? U_IsDelete { get; set; }
        public int U_UsrCreate { get; set; }
        public int? U_UsrDelete { get; set; }
        public DateTime U_CreateDate { get; set; }
        public DateTime? U_DeleteDate { get; set; }
        public short U_CreateTime { get; set; }
        public short? U_DeleteTime { get; set; }
    }
}
