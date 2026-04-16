using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class PurchaseRequestCloseRequestDto
    {
        public int DocEntry { get; set; }
        public int U_UsrClose { get; set; }


        public PurchaseRequestCloseEntity ReturnValue()
        {
            return new PurchaseRequestCloseEntity()
            {
                DocEntry = DocEntry,
                U_UsrClose = U_UsrClose,
            };
        }
    }
}
