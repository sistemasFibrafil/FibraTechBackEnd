using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class ProcesoRepository : RepositoryBase<ProcessesEntity>, IProcessesRepository
    {
        // PARAMETROS DE COXIÓN  
        private readonly DataContextSAPBusinessOne _db;

        public ProcesoRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
        }


        public async Task<ResultadoTransaccionEntity<ProcessesEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ProcessesEntity>();

            try
            {
                var data = await _db.Processes.ToListAsync();

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

        public async Task<ResultadoTransaccionEntity<ProcessesEntity>> GetListByFiltro(ProcessesEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ProcessesEntity>();

            try
            {
                var data = await _db.Processes.Where(x => x.Name.ToUpper().Contains(value.Name == null ? "" : value.Name)).ToListAsync();

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
