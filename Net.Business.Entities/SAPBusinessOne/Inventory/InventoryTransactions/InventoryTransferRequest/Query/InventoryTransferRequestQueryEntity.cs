using System;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities;
namespace Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Query
{
    public class InventoryTransferRequestQueryEntity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string? ObjType { get; set; }
        public string? DocStatus { get; set; }
        public string? U_FIB_DocStPkg { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string? U_FIB_IsPkg { get; set; }
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public int? CntctCode { get; set; } = 0;
        public string? Address { get; set; }
        public string? Filler { get; set; }
        public string? ToWhsCode { get; set; }
        public string? U_FIB_TIP_TRAS { get; set; }
        public string? U_BPP_MDMT { get; set; }
        public string? U_BPP_MDTS { get; set; }
        public int? SlpCode { get; set; } = -1;
        public decimal U_FIB_NBULTOS { get; set; } = 0;
        public decimal U_FIB_KG { get; set; } = 0;
        public string? JrnlMemo { get; set; }
        public string? Comments { get; set; }


        // 🔗 1 → N (OWTQ → WTQ1)
        public List<InventoryTransferRequest1QueryEntity> Lines { get; set; } = new List<InventoryTransferRequest1QueryEntity>();


        // 🔗 1 → N (OWTQ → @FIB_OPKG)
        public List<PickingEntity> PickingLines { get; set; } = new List<PickingEntity>();
    }

    public class InventoryTransferRequest1QueryEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string? LineStatus { get; set; }
        public string? ObjType { get; set; }
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public string? U_FIB_LinStPkg { get; set; }
        public string? U_FIB_FromPkg { get; set; }
        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? FromWhsCod { get; set; }
        public string? WhsCode { get; set; }
        public string? U_tipoOpT12 { get; set; }
        public string? U_tipoOpT12Nam { get; set; }
        public string? UnitMsr { get; set; }
        public decimal Quantity { get; set; }
        public decimal? U_FIB_OpQtyPkg { get; set; }
        public decimal OpenQty { get; set; }
        public decimal? U_FIB_NBulto { get; set; }
        public decimal? U_FIB_PesoKg { get; set; }
        public int Record { get; set; } = 2;
    }
}
