using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class ExchangeRatesRepository : RepositoryBase<ExchangeRatesEntity>, IExchangeRatesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _dc;

        public ExchangeRatesRepository(IConnectionSQL context, DataContextSAPBusinessOne dc)
            : base(context)
        {
            _dc = dc;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionResponse<ExchangeRatesQueryEntity>> GetByDocDateAndCurrency(ExchangeRatesFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<ExchangeRatesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _dc.ExchangeRates
                .Where(n => n.RateDate == value.RateDate)
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
