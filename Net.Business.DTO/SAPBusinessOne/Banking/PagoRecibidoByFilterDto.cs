using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class PagoRecibidoByFilterDto
    {
        public DateTime CourtDate { get; set; }
        public string BusinessPartnerGroup { get; set; }
        public string Customer { get; set; }

        public PagoRecibidoByFilterEntity ReturnValue()
        {
            return new PagoRecibidoByFilterEntity
            {
                CourtDate = CourtDate,
                BusinessPartnerGroup = BusinessPartnerGroup,
                Customer = Customer
            };
        }
    }
}
