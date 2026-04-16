using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class FacturaVentaSapByFilterFindDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Customer { get; set; }

        public FacturaVentaSapByFilterFindEntity ReturnValue()
        {
            return new FacturaVentaSapByFilterFindEntity
            {
                StartDate = StartDate,
                EndDate = EndDate,
                Customer = Customer
            };
        }
    }
}
