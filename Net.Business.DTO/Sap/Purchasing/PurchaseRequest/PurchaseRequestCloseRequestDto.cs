using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class PurchaseRequestCloseRequestDto
    {
        public int DocEntry { get; set; }
        public int U_UsrUpdate { get; set; }


        public PurchaseRequestCloseEntity ReturnValue()
        {
            return new PurchaseRequestCloseEntity()
            {
                DocEntry = DocEntry,
                U_UsrUpdate = U_UsrUpdate,
            };
        }
    }
}
