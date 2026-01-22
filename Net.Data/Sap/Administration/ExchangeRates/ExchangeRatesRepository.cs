using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace Net.Data.Sap
{
    public class ExchangeRatesRepository : RepositoryBase<ExchangeRatesEntity>, IExchangeRatesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSap _dc;

        public ExchangeRatesRepository(IConnectionSQL context, DataContextSap dc)
            : base(context)
        {
            _dc = dc;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<ExchangeRatesQueryEntity>> GetByDocDateAndCurrency(ExchangeRatesFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ExchangeRatesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                //var rate = await _dc.ExchangeRates
                //.Where(n => n.RateDate == value.RateDate && n.Currency == value.Currency)
                //.Select(n => n.Rate)
                //.FirstOrDefaultAsync();

                //var sysRate = await _dc.ExchangeRates
                //.Where(n => n.RateDate == value.RateDate && n.Currency == value.SysCurrncy)
                //.Select(n => n.Rate)
                //.FirstOrDefaultAsync();

                //var data = new ExchangeRatesQueryEntity
                //{
                //    Rate = rate == 0 ? 1 : rate,
                //    SysRate = sysRate
                //};

                var data = await _dc.ExchangeRates
                .Where(n => n.RateDate == value.RateDate &&
                           (n.Currency == value.Currency ||
                            n.Currency == value.SysCurrncy))
                .GroupBy(_ => 1)
                .Select(g => new ExchangeRatesQueryEntity
                {
                    Rate = g
                        .Where(x => x.Currency == value.Currency)
                        .Select(x => x.Rate)
                        .FirstOrDefault(),

                    SysRate = g
                        .Where(x => x.Currency == value.SysCurrncy)
                        .Select(x => x.Rate)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
    }
}
