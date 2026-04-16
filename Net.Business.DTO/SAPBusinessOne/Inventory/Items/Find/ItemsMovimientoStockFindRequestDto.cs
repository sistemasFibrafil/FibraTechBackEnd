using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ItemsMovimientoStockFindRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
        public string? TypeMovement { get; set; }
        public string? Customer { get; set; }
        public string? Item { get; set; }

        public ArticuloMovimientoStockFindEntity ReturnValue()
        {
            return new ArticuloMovimientoStockFindEntity
            {
                StartDate = StartDate,
                EndDate = EndDate,
                Location = Location,
                TypeMovement = TypeMovement,
                Customer = Customer,
                Item = Item,
            };
        }
    }
}
