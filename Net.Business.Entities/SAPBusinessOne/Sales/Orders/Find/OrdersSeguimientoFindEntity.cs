using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class OrdersSeguimientoFindEntity
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? BusinessPartnerGroup { get; set; }
        public string? SalesEmployee { get; set; }
        public string? DocumentType { get; set; }
        public string? Status { get; set; }
        public string? Customer { get; set; }
        public string? Item { get; set; }
    }
}
