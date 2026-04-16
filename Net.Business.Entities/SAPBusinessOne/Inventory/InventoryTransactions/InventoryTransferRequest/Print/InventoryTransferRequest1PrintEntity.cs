namespace Net.Business.Entities.SAPBusinessOne
{
    public class InventoryTransferRequest1PrintEntity
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? FromWhsCod { get; set; }
        public string? WhsCode { get; set; }
        public decimal Quantity { get; set; }
    }
}
