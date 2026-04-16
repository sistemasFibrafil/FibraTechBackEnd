using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OrdersCloseRequestDto
    {
        public int DocEntry { get; set; }
        public int U_UsrClose { get; set; }


        public OrdersCloseEntity ReturnValue()
        {
            return new OrdersCloseEntity()
            {
                DocEntry = DocEntry,
                U_UsrClose = U_UsrClose,
            };
        }
    }
}
