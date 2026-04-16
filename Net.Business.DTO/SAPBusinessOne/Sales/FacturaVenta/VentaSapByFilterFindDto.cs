using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class VentaSapByFilterFindDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? SalesEmployee { get; set; }
        public string? Customer { get; set; }
        public string? Item { get; set; }

        public VentaSapByFilterFindEntity ReturnValue()
        {
            return new VentaSapByFilterFindEntity
            {
                StartDate = StartDate,
                EndDate = EndDate,
                SalesEmployee = SalesEmployee,
                Customer = Customer,
                Item = Item,
            };
        }
    }
}
