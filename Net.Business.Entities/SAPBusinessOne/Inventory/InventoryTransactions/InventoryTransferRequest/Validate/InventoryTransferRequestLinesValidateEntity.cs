namespace Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Validate
{
    public class InventoryTransferRequestLinesValidateEntity
    {
        public string? ItemCode { get; set; }

        public string? FromWhsCod { get; set; }
        public string? WhsCode { get; set; }

        public string? U_tipoOpT12 { get; set; }

        public string? UnitMsr { get; set; }
        public decimal Quantity { get; set; }

    }
}
