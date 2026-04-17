using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Net.Data.SAPBusinessOne
{
    public class CountriesRepository : RepositoryBase<CountryEntity>, ICountriesRepository
    {
        private string _aplicacionName;
        private readonly DataContextSAPBusinessOne _db;

        public CountriesRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<CountryEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<CountryEntity>
            {
                NombreMetodo = "GetList",
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.Country.OrderBy(x => x.Name).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                resultTransaccion.dataList = data;
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
