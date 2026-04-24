using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class DocumentNumberingSeriesSunatRepository : RepositoryBase<DocumentNumberingSeriesSunatEntity>, IDocumentNumberingSeriesSunatRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;

        public DocumentNumberingSeriesSunatRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionResponse<DocumentNumberingSeriesSunatQueryEntity>> GetListSerieDocumento(DocumentNumberingSeriesSunatFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DocumentNumberingSeriesSunatQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query =
                from num in _db.DocumentNumberingSeriesSunat.AsNoTracking()
                join line in _db.DocumentSeriesConfiguration1 on new { num.U_BPP_NDTD, num.U_BPP_NDSD } equals new { U_BPP_NDTD = line.U_Type, U_BPP_NDSD = line.U_Series }
                join head in _db.DocumentSeriesConfiguration on line.Code equals head.Code
                where num.U_BPP_NDTD == value.U_BPP_NDTD
                select new
                {
                    num,
                    line,
                    head
                };

                if (value.IdUsuario != 0)
                {
                    query = query.Where(x => x.head.U_IdUser == value.IdUsuario);
                }

                if (!string.IsNullOrWhiteSpace(value.U_SalesInvoices))
                {
                    query = query.Where(x => x.line.U_SalesInvoices == value.U_SalesInvoices);
                }

                if (!string.IsNullOrWhiteSpace(value.U_Delivery))
                {
                    query = query.Where(x => x.line.U_Delivery == value.U_Delivery);
                }

                if (!string.IsNullOrWhiteSpace(value.U_Transfer))
                {
                    query = query.Where(x => x.line.U_Transfer == value.U_Transfer);
                }

                if (!string.IsNullOrWhiteSpace(value.U_BPP_NDCD))
                {
                    var ndcd = value.U_BPP_NDCD.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(x.num.U_BPP_NDCD, $"%{ndcd}%")
                    );
                }

                var list = await query
                .Select(x => new DocumentNumberingSeriesSunatQueryEntity
                {
                    U_BPP_NDTD = x.num.U_BPP_NDTD,
                    U_BPP_NDSD = x.num.U_BPP_NDSD,
                    U_BPP_NDCD = x.num.U_BPP_NDCD,

                    U_Default = x.line.U_Default
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
        public async Task<ResultadoTransaccionResponse<DocumentNumberingSeriesSunatQueryEntity>> GetNumeroDocumentoByTipoSerie(DocumentNumberingSeriesSunatEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DocumentNumberingSeriesSunatQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.DocumentNumberingSeriesSunat
                .Select(x => new DocumentNumberingSeriesSunatQueryEntity
                {
                    U_BPP_NDTD = x.U_BPP_NDTD,
                    U_BPP_NDSD = x.U_BPP_NDSD,
                    U_BPP_NDCD = x.U_BPP_NDCD
                })
                .Where(n => n.U_BPP_NDTD == value.U_BPP_NDTD && n.U_BPP_NDSD == value.U_BPP_NDSD).FirstOrDefaultAsync();

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
