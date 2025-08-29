using System;
using Net.Business.Entities.Sap;

namespace Net.Business.DTO
{
    public class MovimientoStockSapByFechaSedeFindDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string TypeMovement { get; set; }
        public string Customer { get; set; }
        public string Item { get; set; }

        public MovimientoStockSapByFechaSedeFindEntity ReturnValue()
        {
            return new MovimientoStockSapByFechaSedeFindEntity
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Location = this.Location,
                TypeMovement = this.TypeMovement,
                Customer = this.Customer,
                Item = this.Item,
            };
        }
    }
}
