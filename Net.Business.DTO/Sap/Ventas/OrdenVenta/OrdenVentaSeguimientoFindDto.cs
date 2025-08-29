using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO
{
    public class OrdenVentaSeguimientoFindDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BusinessPartnerGroup { get; set; }
        public string SalesEmployee { get; set; }
        public string DocumentType { get; set; }
        public string Status { get; set; }
        public string Customer { get; set; }

        public OrdenVentaSeguimientoFindEntity ReturnValue()
        {
            return new OrdenVentaSeguimientoFindEntity
            {
                StartDate = StartDate,
                EndDate = EndDate,
                BusinessPartnerGroup = BusinessPartnerGroup,
                SalesEmployee = SalesEmployee,
                DocumentType = DocumentType,
                Status = Status,
                Customer = Customer,
            };
        }
    }
}
