using System;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace Net.Data
{
    public class LongitudAnchoRepository : RepositoryBase<LongitudAnchoEntity>, ILongitudAnchoRepository
    {
        private readonly string _aplicacionName;
        private readonly DataContextSap _dc;

        public LongitudAnchoRepository(IConnectionSQL context, DataContextSap dc)
            : base(context)
        {
            _dc = dc;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<LongitudAnchoEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<LongitudAnchoEntity>
            {
                NombreAplicacion = _aplicacionName,
                NombreMetodo = nameof(GetList)
            };

            try
            {
                var list = await _dc.LongitudAncho.ToListAsync();

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
