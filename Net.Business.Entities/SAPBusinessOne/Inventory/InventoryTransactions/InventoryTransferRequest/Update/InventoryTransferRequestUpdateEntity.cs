using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class InventoryTransferRequestUpdateEntity
    {
        public int DocEntry { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }

        public string? U_FIB_IsPkg { get; set; }

        public string? CardCode { get; set; }

        public string? Filler { get; set; }
        public string? ToWhsCode { get; set; }
        
        public string? U_FIB_TIP_TRAS { get; set; }
        public string? U_BPP_MDMT { get; set; }
        public string? U_BPP_MDTS { get; set; }

        public int SlpCode { get; set; }
        public string? JrnlMemo { get; set; }
        public string? Comments { get; set; }

        public int U_UsrUpdate { get; set; }

        public List<InventoryTransferRequest1UpdateEntity> Lines { get; set; } = new List<InventoryTransferRequest1UpdateEntity>();
    }

    public class InventoryTransferRequest1UpdateEntity
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string? LineStatus { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string? Dscription { get; set; }
        public string? FromWhsCod { get; set; }
        public string? WhsCode { get; set; }
        public string? UnitMsr { get; set; }
        public double Quantity { get; set; }

        public string U_FIB_LinStPkg { get; set; } = string.Empty;
        public double U_FIB_OpQtyPkg { get; set; }
        public string? U_tipoOpT12 { get; set; }

        public int Record { get; set; }
    }
}
