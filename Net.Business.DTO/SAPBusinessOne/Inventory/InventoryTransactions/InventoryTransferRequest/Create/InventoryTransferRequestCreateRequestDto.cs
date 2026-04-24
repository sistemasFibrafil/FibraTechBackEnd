using System;
using System.Collections.Generic;
using Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Create;
namespace Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create
{
    public class InventoryTransferRequestCreateRequestDto
    {
        public string? ObjType { get; set; }

        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }

        public string? U_FIB_IsPkg { get; set; }
        public string? U_FIB_DocStPkg { get; set; }

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
        public List<InventoryTransferRequestLinesCreateRequestDto> Lines { get; set; } = [];
        public List<InventoryTransferRequestPickingCreateRequestDto> PickingLines { get; set; } = [];
    }
}
