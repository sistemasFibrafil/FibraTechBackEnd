namespace Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create
{
    public class InventoryTransferRequestLinesCreateRequestDto
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
