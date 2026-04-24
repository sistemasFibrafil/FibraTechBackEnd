namespace Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Create
{
    public class InventoryTransferRequestPickingCreateRequestDto
    {
        public string U_ItemCode { get; set; } = string.Empty;
        public string? U_Dscription { get; set; }
        public string U_CodeBar { get; set; } = string.Empty;

        public string U_FromWhsCod { get; set; } = string.Empty;
        public string U_WhsCode { get; set; } = string.Empty;

        public string? U_UnitMsr { get; set; }
        public decimal U_Quantity { get; set; }
        public decimal U_WeightKg { get; set; }
        public string? U_Status { get; set; }
    }
}
