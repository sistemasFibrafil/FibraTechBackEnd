using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class TiempoVidaRepository : RepositoryBase<TiempoVidaEntity>, ITiempoVidaRepository
    {
        // PARAMETROS DE COXIÓN  
        private readonly DataContextSAPBusinessOne _db;

        public TiempoVidaRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
        }

        public async Task<ResultadoTransaccionResponse<TiempoVidaEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionResponse<TiempoVidaEntity>();

            try
            {
                var data = await _db.TiempoVida.ToListAsync();

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

        public async Task<ResultadoTransaccionResponse<TiempoVidaEntity>> GetListByFiltro(TiempoVidaEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<TiempoVidaEntity>();

            try
            {
                var data = await _db.TiempoVida.Where(x => x.Name.ToUpper().Contains(value.Name == null ? "" : value.Name)).ToListAsync();

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
