using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    /// <summary>
    /// Tipo de cambio
    /// </summary>
    public class ExchangeRatesEntity
    {
        public DateTime RateDate { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
    }
}
