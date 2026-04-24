namespace Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Find
{
    public class PickingFindRequestDto
    {
        public string? U_Status { get; set; }
        public int U_BaseEntry { get; set; }
        public int U_BaseType { get; set; }
        public int U_BaseLine { get; set; }
        public string? U_CodeBar { get; set; }
    }
}
