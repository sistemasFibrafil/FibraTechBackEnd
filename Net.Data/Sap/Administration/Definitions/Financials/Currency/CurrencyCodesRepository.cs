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
    public class CurrencyCodesRepository : RepositoryBase<CurrencyCodesEntity>, ICurrencyCodesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSap _db;


        public CurrencyCodesRepository(IConnectionSQL context, DataContextSap db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<CurrencyCodesEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<CurrencyCodesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.CurrencyCodes.AsNoTracking().ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
                resultTransaccion.dataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<CurrencyCodesEntity>> GetListByCode(string currCode)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<CurrencyCodesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                IQueryable<CurrencyCodesEntity> query = _db.CurrencyCodes.AsNoTracking();

                if (!string.IsNullOrEmpty(currCode) && currCode != "##")
                {
                    query = query.Where(n => n.CurrCode == currCode);
                }

                var list = await query.ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
                resultTransaccion.dataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
    }
}
