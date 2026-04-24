namespace Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Cancel
{
    public class DeliveryNotesCancelRequestDto
    {
        public int DocEntry { get; set; }
        public int U_UsrCreate { get; set; }
        public int U_UsrCancel { get; set; }
    }
}
