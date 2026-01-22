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
    public class TipoDocumentoSunatRepository : RepositoryBase<TipoDocumentoSunatEntity>, ITipoDocumentoSunatRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSap _db;


        public TipoDocumentoSunatRepository(IConnectionSQL context, DataContextSap db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<TipoDocumentoSunatEntity>> GetListByTipo(TipoDocumentoSunatEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TipoDocumentoSunatEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.TipoDocumentoSunat
                .AsNoTracking();
                

                // Filtrar por Tipo de Documento de Transferencia: Puede ser Y o N
                if (!string.IsNullOrWhiteSpace(value.U_FIB_TDTD))
                {
                    query = query.Where(n => n.U_FIB_TDTD == value.U_FIB_TDTD);
                }


                var list = await query
                .Select(x => new TipoDocumentoSunatEntity
                {
                    U_BPP_TDTD = x.U_BPP_TDTD,
                    U_BPP_TDDD = x.U_BPP_TDDD,
                })
                .OrderBy(x => x.U_BPP_TDTD)
                .ThenBy(x => x.U_BPP_TDDD)
                .ToListAsync();

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
