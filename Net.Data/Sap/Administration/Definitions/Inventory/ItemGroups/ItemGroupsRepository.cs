using System;
using System.Data;
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
    public class ItemGroupsRepository : RepositoryBase<ItemGroupsEntity>, IItemGroupsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSap _dc;


        public ItemGroupsRepository(IConnectionSQL context, DataContextSap dc)
            : base(context)
        {
            _dc = dc;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<ItemGroupsEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ItemGroupsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _dc.ItemGroups.Where(x=>x.ItmsGrpCod != 195).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
                resultTransaccion.dataList = list;
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
