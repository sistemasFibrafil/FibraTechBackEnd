namespace Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Cancel
{
    public class InvoicesCancelRequestDto
    {
        public int DocEntry { get; set; }
        public int U_UsrCreate { get; set; }
        public int U_UsrCancel { get; set; }
    }
}
