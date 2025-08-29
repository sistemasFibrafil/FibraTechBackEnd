using System;
using System.Data;
using System.Linq;
using Net.Connection;
using System.Transactions;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Net.Business.Entities.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Net.Data.Web
{
    public class SerieNumeracionRepository : RepositoryBase<SerieNumeracionEntity>, ISerieNumeracionRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_BY_FILTRO = DB_ESQUEMA + "GES_GetListSerieNumeracionByFiltro";
        const string SP_SET_CREATE = DB_ESQUEMA + "GES_SetSerieNumeracionCreate";
        const string SP_SET_UPDATE = DB_ESQUEMA + "GES_SetSerieNumeracionUpdate";
        const string SP_SET_DELETE = DB_ESQUEMA + "GES_SetSerieNumeracionDelete";


        public SerieNumeracionRepository(IConnectionSQL context)
            : base(context)
        {
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<SerieNumeracionEntity>> GetListByFiltro(FilterRequestEntity value)
        {
            var response = new List<SerieNumeracionEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<SerieNumeracionEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_BY_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@CodSede", value.Id1));
                        cmd.Parameters.Add(new SqlParameter("@CodFormulario", value.Id2));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<SerieNumeracionEntity>)context.ConvertTo<SerieNumeracionEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<SerieNumeracionEntity>> SetAction(SerieNumeracionEntity value)
        {
            var responde = new SerieNumeracionEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<SerieNumeracionEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    using (CommittableTransaction transaction = new CommittableTransaction())
                    {
                        await conn.OpenAsync();
                        conn.EnlistTransaction(transaction);

                        // SE ELIMINA
                        foreach (var linea in value.Linea.Where(x => x.Record == 3))
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_SET_DELETE, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;
                                cmd.Parameters.Add(new SqlParameter("@CodSerieNumeracion", linea.CodSerieNumeracion));

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        // SE ACTUALIZA
                        foreach (var linea in value.Linea.Where(x => x.Record == 2))
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_SET_UPDATE, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;
                                cmd.Parameters.Add(new SqlParameter("@CodSerieNumeracion", linea.CodSerieNumeracion));
                                cmd.Parameters.Add(new SqlParameter("@TipDocumento", linea.TipDocumento));
                                cmd.Parameters.Add(new SqlParameter("@SerDocumento", linea.SerDocumento));
                                cmd.Parameters.Add(new SqlParameter("@NumDocumento", linea.NumDocumento));
                                cmd.Parameters.Add(new SqlParameter("@MaxNumDocumento", linea.MaxNumDocumento));
                                cmd.Parameters.Add(new SqlParameter("@CodSede", linea.CodSede));
                                cmd.Parameters.Add(new SqlParameter("@CodFormulario", linea.CodFormulario));
                                cmd.Parameters.Add(new SqlParameter("@IdUsuarioUpdate", linea.IdUsuario));

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        // SE CREA
                        foreach (var linea in value.Linea.Where(x => x.Record == 1))
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_SET_CREATE, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;
                                cmd.Parameters.Add(new SqlParameter("@CodSerieNumeracion", linea.CodSerieNumeracion));
                                cmd.Parameters.Add(new SqlParameter("@TipDocumento", linea.TipDocumento));
                                cmd.Parameters.Add(new SqlParameter("@SerDocumento", linea.SerDocumento));
                                cmd.Parameters.Add(new SqlParameter("@NumDocumento", linea.NumDocumento));
                                cmd.Parameters.Add(new SqlParameter("@MaxNumDocumento", linea.MaxNumDocumento));
                                cmd.Parameters.Add(new SqlParameter("@CodSede", linea.CodSede));
                                cmd.Parameters.Add(new SqlParameter("@CodFormulario", linea.CodFormulario));
                                cmd.Parameters.Add(new SqlParameter("@IdUsuarioCreate", linea.IdUsuario));

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();

                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "Registro procesado con éxito ..!";
                        return resultTransaccion;
                    }
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                return resultTransaccion;
            }
        }
    }
}
