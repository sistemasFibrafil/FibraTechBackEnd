using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class ExchangeRatesFindEntity
    {
        public DateTime RateDate { get; set; }
        public string? Currency { get; set; }
        public string? SysCurrncy { get; set; }
    }
}
