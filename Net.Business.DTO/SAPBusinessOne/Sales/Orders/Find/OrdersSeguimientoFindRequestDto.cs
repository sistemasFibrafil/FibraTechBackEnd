using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OrdersSeguimientoFindRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? BusinessPartnerGroup { get; set; }
        public string? SalesEmployee { get; set; }
        public string? DocumentType { get; set; }
        public string? Status { get; set; }
        public string? Customer { get; set; }
        public string? Item { get; set; }

        public OrdersSeguimientoFindEntity ReturnValue()
        {
            return new OrdersSeguimientoFindEntity
            {
                StartDate = StartDate,
                EndDate = EndDate,
                BusinessPartnerGroup = BusinessPartnerGroup,
                SalesEmployee = SalesEmployee,
                DocumentType = DocumentType,
                Status = Status,
                Customer = Customer,
                Item = Item,
            };
        }
    }
}
