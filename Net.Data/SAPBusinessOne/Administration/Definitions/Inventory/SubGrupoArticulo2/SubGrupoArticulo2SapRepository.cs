using System;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class SubGrupoArticulo2SapRepository : RepositoryBase<SubGrupoArticulo2SapEntity>, ISubGrupoArticulo2Repository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _dc;


        public SubGrupoArticulo2SapRepository(IConnectionSQL context, DataContextSAPBusinessOne dc)
            : base(context)
        {
            _dc = dc;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<SubGrupoArticulo2SapEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SubGrupoArticulo2SapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var list = await _dc.SubGrupoArticulo2.ToListAsync();

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
