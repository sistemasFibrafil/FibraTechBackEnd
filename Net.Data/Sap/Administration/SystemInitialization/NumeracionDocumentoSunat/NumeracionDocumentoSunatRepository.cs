using System;
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
    public class NumeracionDocumentoSunatRepository : RepositoryBase<NumeracionDocumentoSunatEntity>, INumeracionDocumentoSunatRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSap _db;

        public NumeracionDocumentoSunatRepository(IConnectionSQL context, DataContextSap db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<NumeracionDocumentoSunatEntity>> GetListSerieDocumento(NumeracionDocumentoSunatEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<NumeracionDocumentoSunatEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.NumeracionDocumentoSunat
                .AsNoTracking()
                .Where(x => x.U_BPP_NDTD == value.U_BPP_NDTD && x.U_FIB_SEDE == value.U_FIB_SEDE);


                // Filtrar por Tipo de Documento de Entrega: Puede ser Y o N
                if (!string.IsNullOrWhiteSpace(value.U_FIB_TDED))
                {
                    query = query.Where(n => n.U_FIB_TDED == value.U_FIB_TDED);
                }


                // Extraer por tipo de documento de transferencia: Puede ser Y o N
                if (!string.IsNullOrWhiteSpace(value.U_FIB_TDTD))
                {
                    query = query.Where(n => n.U_FIB_TDTD == value.U_FIB_TDTD);
                }


                // Filtrar por Número de Documento
                if (!string.IsNullOrWhiteSpace(value.U_BPP_NDCD))
                {
                    var u_BPP_NDCD = value.U_BPP_NDCD.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.U_BPP_NDCD, $"%{u_BPP_NDCD}%")
                    );
                }


                var list = await query
                .Select(x => new NumeracionDocumentoSunatEntity
                {
                    U_BPP_NDTD = x.U_BPP_NDTD,
                    U_BPP_NDSD = x.U_BPP_NDSD,
                    U_BPP_NDCD = x.U_BPP_NDCD,
                    U_FIB_SDED = x.U_FIB_SDED,
                    U_FIB_SDTD = x.U_FIB_SDTD
                })
                .OrderBy(x => x.U_BPP_NDTD)
                .ThenBy(x => x.U_BPP_NDSD)
                .ThenBy(x => x.U_BPP_NDCD)
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
        public async Task<ResultadoTransaccionEntity<NumeracionDocumentoSunatEntity>> GetNumeroDocumentoByTipoSerie(NumeracionDocumentoSunatEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<NumeracionDocumentoSunatEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.NumeracionDocumentoSunat.Where(n => n.U_BPP_NDTD == value.U_BPP_NDTD && n.U_BPP_NDSD == value.U_BPP_NDSD).FirstOrDefaultAsync();

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
