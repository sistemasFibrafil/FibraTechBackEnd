using System;
using System.Data;
using Net.Connection;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Net.Business.Entities.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Net.Data.Web
{
    public class DocumentoLecturaRepository : RepositoryBase<DocumentoLecturaEntity>, IDocumentoLecturaRepository
    {
        private string _metodoName;
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_BY_OBJTYPE_CARDCODE = DB_ESQUEMA + "INV_GetListDocumentoPendienteByObjTypeAndCardCode";


        public DocumentoLecturaRepository(IConnectionSQL context)
            : base(context)
        {
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<DocumentoLecturaEntity>> GetListPendienteByObjTypeAndCardCode(FilterRequestEntity value)
        {
            var response = new List<DocumentoLecturaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<DocumentoLecturaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_BY_OBJTYPE_CARDCODE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@ObjType", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@CardCode", value.Cod2));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<DocumentoLecturaEntity>)context.ConvertTo<DocumentoLecturaEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultTransaccion.dataList = response;
                }
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
