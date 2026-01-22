using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using Microsoft.EntityFrameworkCore;
namespace Net.Data.Sap
{
    public class ColorImpresionRepository : RepositoryBase<ColorImpresionEntity>, IColorImpresionRepository
    {
        // PARAMETROS DE COXIÓN  
        private readonly DataContextSap _db;

        public ColorImpresionRepository(IConnectionSQL context, DataContextSap db)
            : base(context)
        {
            _db = db;
        }

        public async Task<ResultadoTransaccionEntity<ColorImpresionEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ColorImpresionEntity>();

            try
            {
                var data = await _db.ColorImpresion.ToListAsync();

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

        public async Task<ResultadoTransaccionEntity<ColorImpresionEntity>> GetListByFiltro(ColorImpresionEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ColorImpresionEntity>();

            try
            {
                var data = await _db.ColorImpresion.Where(x => x.Name.ToUpper().Contains(value.Name == null ? "" : value.Name)).ToListAsync();

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
