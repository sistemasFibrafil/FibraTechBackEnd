using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class OrdersFilterRequestDto
    {
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public string DocStatus { get; set; } = null;
        public string SearchText { get; set; } = null;

        public OrdersFilterEntity ReturnValue()
        {
            return new OrdersFilterEntity
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                DocStatus = this.DocStatus,
                SearchText = this.SearchText
            };
        }
    }
}
