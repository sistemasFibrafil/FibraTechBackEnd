using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO
{
    public class OrdersSeguimientoFindRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BusinessPartnerGroup { get; set; }
        public string SalesEmployee { get; set; }
        public string DocumentType { get; set; }
        public string Status { get; set; }
        public string Customer { get; set; }
        public string Item { get; set; }

        public OrdersSeguimientoFindEntity ReturnValue()
        {
            return new OrdersSeguimientoFindEntity
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                BusinessPartnerGroup = this.BusinessPartnerGroup,
                SalesEmployee = this.SalesEmployee,
                DocumentType = this.DocumentType,
                Status = this.Status,
                Customer = this.Customer,
                Item = this.Item,
            };
        }
    }
}
