using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ExchangeRatesFindRequestDto
    {
        public DateTime RateDate { get; set; }
        public string? Currency { get; set; }
        public string? SysCurrncy { get; set; }

        public ExchangeRatesFindEntity ReturnValue()
        {
            return new ExchangeRatesFindEntity
            {
                RateDate = RateDate,
                Currency = Currency,
                SysCurrncy = SysCurrncy
            };
        }
    }
}
