using System;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Create;
namespace Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create
{
    public class InventoryTransferRequestCreateEntity
    {
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }

        public string? U_FIB_DocStPkg { get; set; }
        public string? U_FIB_IsPkg { get; set; }

        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public int CntctCode { get; set; }
        public string? Address { get; set; }

        public string? Filler { get; set; }
        public string? ToWhsCode { get; set; }

        public string? U_FIB_TIP_TRAS { get; set; }
        public string? U_BPP_MDMT { get; set; }
        public string? U_BPP_MDTS { get; set; }

        public int SlpCode { get; set; }
        public string? JrnlMemo { get; set; }
        public string? Comments { get; set; }

        public int U_UsrCreate { get; set; }

        public List<InventoryTransferRequest1CreateEntity> Lines { get; set; } = [];
        public List<InventoryTransferRequestPickingCreateEntity> PickingLines { get; set; } = [];
    }

    public class InventoryTransferRequest1CreateEntity
    {
        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }

        public string? FromWhsCod { get; set; }
        public string? WhsCode { get; set; }
        public string? UnitMsr { get; set; }
        public double Quantity { get; set; }

        public string? U_FIB_LinStPkg { get; set; }
        public double U_FIB_OpQtyPkg { get; set; }
        public string? U_tipoOpT12 { get; set; }
    }
}
