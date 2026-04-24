using Net.Business.DTO.SAPBusinessOne.Sales.Orders.Close;
using Net.Business.Entities.SAPBusinessOne.Sales.Orders.Close;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.Orders.Close
{
    public class OrdersCloseMapper
    {
        public static OrdersCloseEntity ToEntity(OrdersCloseRequestDto dto)
        {
            return new OrdersCloseEntity
            {
                DocEntry = dto.DocEntry,
                U_UsrClose = dto.U_UsrClose
            };
        }
    }
}
