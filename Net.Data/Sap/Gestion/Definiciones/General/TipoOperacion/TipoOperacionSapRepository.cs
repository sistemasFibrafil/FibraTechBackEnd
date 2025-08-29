using System;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
namespace Net.Data.Sap
{
    public class TipoOperacionSapRepository : RepositoryBase<TipoOperacionSapEntity>, ITipoOperacionSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN  
        private readonly DataContextFil _dc;


        public TipoOperacionSapRepository(IConnectionSQL context, DataContextFil dc)
            : base(context)
        {
            _dc = dc;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<TipoOperacionSapEntity>> GetListByFiltro(TipoOperacionSapEntity value)
        {
            var response = new List<TipoOperacionSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<TipoOperacionSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var filter = value.U_descrp == null ? "" : value.U_descrp.ToUpper().Trim();

                var data = await _dc.TipoOperacion.Where(x => x.U_descrp.ToUpper().Contains(filter)).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                resultTransaccion.dataList = response;
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
