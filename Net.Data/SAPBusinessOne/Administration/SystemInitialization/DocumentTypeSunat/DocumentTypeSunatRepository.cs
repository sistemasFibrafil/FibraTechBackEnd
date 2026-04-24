using System;
using System.Data;
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
    public class DocumentTypeSunatRepository : RepositoryBase<DocumentTypeSunatEntity>, IDocumentTypeSunatRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;


        public DocumentTypeSunatRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionResponse<DocumentTypeSunatEntity>> GetListByType(DocumentTypeSunatEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DocumentTypeSunatEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.TipoDocumentoSunat
                .AsNoTracking();
                

                // Buscar por Tipo de Documento de Entrega: Puede ser Y o N
              if (!string.IsNullOrWhiteSpace(value.U_FIB_ENTR))
                {
                    query = query.Where(n => n.U_FIB_ENTR == value.U_FIB_ENTR);
                }

                // Buscar por Tipo de Documento de Factura de Venta: Puede ser Y o N
                if (!string.IsNullOrWhiteSpace(value.U_FIB_FAVE))
                {
                    query = query.Where(n => n.U_FIB_FAVE == value.U_FIB_FAVE);
                }

                // Buscar por Tipo de Documento de Transferencia: Puede ser Y o N
                if (!string.IsNullOrWhiteSpace(value.U_FIB_TRAN))
                {
                    query = query.Where(n => n.U_FIB_TRAN == value.U_FIB_TRAN);
                }


                var list = await query
                .Select(x => new DocumentTypeSunatEntity
                {
                    U_BPP_TDTD = x.U_BPP_TDTD,
                    U_BPP_TDDD = x.U_BPP_TDDD,
                    U_FIB_ENDF = x.U_FIB_ENDF,
                    U_FIB_FVDF  = x.U_FIB_FVDF
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
