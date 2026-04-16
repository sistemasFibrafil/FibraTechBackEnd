namespace Net.Business.Entities.SAPBusinessOne
{
    public class DeliveryNotesPickingUpdateEntity
    {
        public int DocEntry { get; set; }
        public int U_BaseEntry { get; set; }
        public int U_BaseLine { get; set; }
        public string? U_Status { get; set; }
        public int U_UsrUpdate { get; set; }
    }
}
