using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class InventoryTransferRequestCreateEntity
    {
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }

        public string U_FIB_DocStPkg { get; set; } = string.Empty;
        public string U_FIB_IsPkg { get; set; } = string.Empty;

        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public int CntctCode { get; set; }
        public string? Address { get; set; }

        public string? Filler { get; set; } = string.Empty;
        public string? ToWhsCode { get; set; } = string.Empty;

        public string? U_FIB_TIP_TRAS { get; set; }
        public string? U_BPP_MDMT { get; set; }
        public string? U_BPP_MDTS { get; set; }

        public int SlpCode { get; set; }
        public string? JrnlMemo { get; set; }
        public string? Comments { get; set; }

        public int U_UsrCreate { get; set; }

        public List<InventoryTransferRequest1CreateEntity> Lines { get; set; } = new List<InventoryTransferRequest1CreateEntity>();
        public List<InventoryTransferRequestPickingCreateEntity> PickingLines { get; set; } = new List<InventoryTransferRequestPickingCreateEntity>();
    }

    public class InventoryTransferRequest1CreateEntity
    {
        public string ItemCode { get; set; } = string.Empty;
        public string? Dscription { get; set; }

        public string FromWhsCod { get; set; } = string.Empty;
        public string WhsCode { get; set; } = string.Empty;

        public string? UnitMsr { get; set; }
        public double Quantity { get; set; }

        public string U_FIB_LinStPkg { get; set; } = string.Empty;
        public double U_FIB_OpQtyPkg { get; set; }
        public string? U_tipoOpT12 { get; set; }
    }
}
