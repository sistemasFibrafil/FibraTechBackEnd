using System;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using Microsoft.EntityFrameworkCore;
namespace Net.Data.Sap
{
    public class EmpleadoVentaSapRepository : RepositoryBase<EmpleadoVentaSapEntity>, IEmpleadoVentaSapRepository
    {
        // PARAMETROS DE COXIÓN
        private readonly DataContextFil _db;

        public EmpleadoVentaSapRepository(IConnectionSQL context, DataContextFil db)
            : base(context)
        {
            _db = db;
        }


        public async Task<ResultadoTransaccionEntity<EmpleadoVentaSapEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<EmpleadoVentaSapEntity>();

            try
            {
                var data = await _db.EmpleadoVenta.Where(x=>x.Active == "Y").ToListAsync();

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
        public async Task<ResultadoTransaccionEntity<EmpleadoVentaSapEntity>> GetById(int id)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<EmpleadoVentaSapEntity>();

            try
            {
                var data = await _db.EmpleadoVenta.FindAsync(id);

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
        public async Task<ResultadoTransaccionEntity<EmpleadoVentaSapEntity>> GetListByFiltro(EmpleadoVentaSapEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<EmpleadoVentaSapEntity>();

            try
            {
                var data = await _db.EmpleadoVenta.Where(x => x.SlpName.Contains(value.SlpName == null? "" : value.SlpName)).ToListAsync();

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
