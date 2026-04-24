using System;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class LongitudAnchoRepository : RepositoryBase<LongitudAnchoEntity>, ILongitudAnchoRepository
    {
        private readonly string _aplicacionName;
        private readonly DataContextSAPBusinessOne _dc;

        public LongitudAnchoRepository(IConnectionSQL context, DataContextSAPBusinessOne dc)
            : base(context)
        {
            _dc = dc;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionResponse<LongitudAnchoEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionResponse<LongitudAnchoEntity>
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
