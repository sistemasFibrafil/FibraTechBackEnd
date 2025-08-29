using System;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Text.RegularExpressions;
namespace Net.Data.Sap
{
    public class TipoCambioSapRepository : RepositoryBase<TipoCambioSapEntity>, ITipoCambioSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextFil _dc;

        public TipoCambioSapRepository(IConnectionSQL context, DataContextFil dc)
            : base(context)
        {
            _dc = dc;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<TipoCambioSapEntity>> GetByFechaCode(TipoCambioSapEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TipoCambioSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var data = await _dc.TipoCambio.FindAsync(value.RateDate, value.Currency);

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
    }
}
