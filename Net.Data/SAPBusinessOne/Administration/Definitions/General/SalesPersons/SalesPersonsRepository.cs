using System;
using System.Data;
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
    public class SalesPersonsRepository : RepositoryBase<SalesPersonsEntity>, ISalesPersonsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;

        public SalesPersonsRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
        }


        public async Task<ResultadoTransaccionEntity<SalesPersonsEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SalesPersonsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.SalesPersons
                .AsNoTracking()
                .OrderBy(x => x.SlpName)
                .Select(x => new SalesPersonsEntity
                {
                    SlpCode = x.SlpCode,
                    SlpName = x.SlpName
                })
                .ToListAsync();

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
        public async Task<ResultadoTransaccionEntity<SalesPersonsEntity>> GetById(int id)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SalesPersonsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.SalesPersons
                .AsNoTracking()
                .Select(x => new SalesPersonsEntity
                {
                    SlpCode = x.SlpCode,
                    SlpName = x.SlpName
                })
                .FirstOrDefaultAsync(x => x.SlpCode == id);

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
        public async Task<ResultadoTransaccionEntity<SalesPersonsEntity>> GetListByFiltro(SalesPersonsEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SalesPersonsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                value.SlpName = value.SlpName?.ToString().Trim() ?? string.Empty;

                var data = await _db.SalesPersons.Where(x => x.SlpName.Contains(value.SlpName)).ToListAsync();

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
