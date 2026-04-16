using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class CargaSaldoInicialRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Item { get; set; }

        public CargaSaldoInicialFilterEntity ReturnValue()
        {
            return new CargaSaldoInicialFilterEntity()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                Item = Item,
            };
        }
    }
}
