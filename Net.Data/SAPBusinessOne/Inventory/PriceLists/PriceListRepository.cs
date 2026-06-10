using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net.Business.Entities;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;

namespace Net.Data.SAPBusinessOne
{
    public class PriceListRepository : RepositoryBase<PriceListEntity>, IPriceListRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");
        private readonly DataContextSAPBusinessOne _db;

        public PriceListRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionResponse<PriceListEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PriceListEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.PriceList
                .AsNoTracking()
                .OrderBy(x => x.PriceListNo)
                .ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
                resultTransaccion.DataList = list;
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
