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
    public class StatusRepository : RepositoryBase<StatusEntity>, IStatusRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST = DB_ESQUEMA + "GES_GetListStatus";



        public StatusRepository(IConnectionSQL context)
            : base(context)
        {
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<StatusEntity>> GetList()
        {
            var response = new List<StatusEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<StatusEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<StatusEntity>)context.ConvertTo<StatusEntity>(reader);
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
