using System;
using Net.Business.Entities.Sap;

namespace Net.Business.DTO.Sap
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
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Item = this.Item,
            };
        }
    }
}
