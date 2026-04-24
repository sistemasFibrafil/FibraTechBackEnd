using System;
using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Entities
{
    public class InventoryTransferRequestEntity
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
        public int? CntctCode { get; set; }
        public string? Address { get; set; }
        public string? Filler { get; set; }
        public string? ToWhsCode { get; set; }
        public string? U_FIB_TIP_TRAS { get; set; }
        public string? U_BPP_MDMT { get; set; }
        public string? U_BPP_MDTS { get; set; }
        public int? SlpCode { get; set; }
        public decimal? U_FIB_NBULTOS { get; set; }
        public decimal? U_FIB_KG { get; set; }
        public string? JrnlMemo { get; set; }
        public string? Comments { get; set; }


        // 🔗 1 → N (OWTQ → WTQ1)
        public ICollection<InventoryTransferRequest1Entity> Lines { get; set; } = new List<InventoryTransferRequest1Entity>();
    }
}
