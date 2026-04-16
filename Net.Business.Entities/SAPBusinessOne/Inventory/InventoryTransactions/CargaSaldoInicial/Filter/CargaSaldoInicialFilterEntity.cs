using System;

namespace Net.Business.Entities.SAPBusinessOne
{
    public class CargaSaldoInicialFilterEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Item { get; set; }
    }
}
