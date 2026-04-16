using System;
using System.Data;
using Net.Connection;
using Newtonsoft.Json;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class FacturacionElectronicaRepositoy : RepositoryBase<FacturacionElectronicaSapEntity>, IFacturacionElectronicaRepositoy
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly string _cnxLocal;
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_GUIA_ELECTRONICA_BY_FILTRO = DB_ESQUEMA + "VEN_GetListGuiaElectronicaByFiltro";
        const string SP_GET_GUIA_ELECTRONICA_BY_ID = DB_ESQUEMA + "VEN_GetGuiaElectronicaById";
        const string SP_GET_LIST_GUIA_DETALLE_ELECTRONICA_BY_ID = DB_ESQUEMA + "VEN_GetListGuiaDetalleElectronicaById";
        
        const string SP_SET_UPDATE_GUIA_SAP = DB_ESQUEMA + "VEN_SetUpdateGuiaElectronica";
        const string SP_SET_UPDATE_GUIA_ERROR_SAP = DB_ESQUEMA + "VEN_SetUpdateGuiaElectronicaError";

        const string SP_SET_UPDATE_GUIA_LOCAL = DB_ESQUEMA + "INV_SetUpdateGuiaElectronica";
        const string SP_SET_UPDATE_GUIA_ERROR_LOCAL = DB_ESQUEMA + "INV_SetUpdateGuiaElectronicaError";

        const string SP_GET_LIST_GUIA_INTERNA_ELECTRONICA_BY_FILTRO = DB_ESQUEMA + "INV_GuiaInternaElectronicaByFiltro";
        const string SP_GET_GUIA_INTERNA_ELECTRONICA_BY_ID = DB_ESQUEMA + "INV_GetGuiaInternaElectronicaById";
        const string SP_GET_LIST_GUIA_DETALLE_INTERNA_ELECTRONICA_BY_ID = DB_ESQUEMA + "INV_GetListGuiaDetalleInternaElectronicaById";


        public FacturacionElectronicaRepositoy(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(_configuration, "EntornoConnectionSap:Entorno");
            _cnxLocal = Utilidades.GetCon(_configuration, "EntornoConnection:Entorno");
        }


        public async Task<ResultadoTransaccionEntity<FacturacionElectronicaSapEntity>> GetListGuiaElectronicaByFiltro(FilterRequestEntity value)
        {
            var response = new List<FacturacionElectronicaSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<FacturacionElectronicaSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_GUIA_ELECTRONICA_BY_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@ObjType", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@CodStatus", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@Filtro1", value.Text1));
                        cmd.Parameters.Add(new SqlParameter("@Filtro2", value.Text2));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<FacturacionElectronicaSapEntity>)context.ConvertTo<FacturacionElectronicaSapEntity>(reader);
                        }

                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        resultTransaccion.dataList = response;
                    }

                    conn.Close();
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
        public async Task<ResultadoTransaccionEntity<FacturacionElectronicaSapEntity>> SetEnviar(FilterRequestEntity value)
        {
            var guia = new Invoice();
            var resultTransaccion = new ResultadoTransaccionEntity<FacturacionElectronicaSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_GUIA_ELECTRONICA_BY_ID, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@ObjType", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@DocEntry", value.Id1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            guia = ((List<Invoice>)context.ConvertTo<Invoice>(reader))[0];
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_GUIA_DETALLE_ELECTRONICA_BY_ID, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@ObjType", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@DocEntry", value.Id1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            guia.items = (List<ItemsGuias>)context.ConvertTo<ItemsGuias>(reader);
                        }
                    }

                    string tsq = JsonConvert.SerializeObject(guia, Formatting.Indented);
                    var rpta = FacturacionElectronica.GetResponse(tsq);

                    var resp = JsonConvert.SerializeObject(rpta, Formatting.Indented);

                    if (rpta.errors == null)
                    {
                        await SetUpdateSapExito(conn, value, rpta);
                        await SetUpdateLocalExito(value, rpta);

                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "El comprobante se envío con éxito...!!!";
                    }
                    else
                    {
                        await SetUpdateSapError(conn, value, rpta);
                        await SetUpdateLocalError(value, rpta);

                        resultTransaccion.IdRegistro = -1;
                        resultTransaccion.ResultadoCodigo = -1;
                        resultTransaccion.ResultadoDescripcion = rpta.errors;
                    }

                    conn.Close();
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

        private async Task SetUpdateSapExito(SqlConnection conn, FilterRequestEntity value, Respuesta resp)
        {
            using (SqlCommand cmd = new SqlCommand(SP_SET_UPDATE_GUIA_SAP, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@ObjType", value.Cod1));
                cmd.Parameters.Add(new SqlParameter("@DocEntry", value.Id1));
                cmd.Parameters.Add(new SqlParameter("@AceptadaPorSunat", resp.aceptada_por_sunat));
                cmd.Parameters.Add(new SqlParameter("@SunatDescription", resp.sunat_description));
                cmd.Parameters.Add(new SqlParameter("@SunatNote", resp.sunat_note));
                cmd.Parameters.Add(new SqlParameter("@SunatResponsecode", resp.sunat_responsecode));
                cmd.Parameters.Add(new SqlParameter("@SunatSoapError", resp.sunat_soap_error));
                cmd.Parameters.Add(new SqlParameter("@CadenaParaCodigoQr", resp.cadena_para_codigo_qr));
                cmd.Parameters.Add(new SqlParameter("@CodigoHash", resp.codigo_hash));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task SetUpdateSapError(SqlConnection conn, FilterRequestEntity value, Respuesta resp)
        {
            using (SqlCommand cmd = new SqlCommand(SP_SET_UPDATE_GUIA_ERROR_SAP, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@ObjType", value.Cod1));
                cmd.Parameters.Add(new SqlParameter("@DocEntry", value.Id1));
                cmd.Parameters.Add(new SqlParameter("@Error", resp.errors));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task SetUpdateLocalExito(FilterRequestEntity value, Respuesta resp)
        {
            using (SqlConnection conn = new SqlConnection(_cnxLocal))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(SP_SET_UPDATE_GUIA_LOCAL, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@ObjType", value.Cod1));
                    cmd.Parameters.Add(new SqlParameter("@DocEntry", value.Id1));
                    cmd.Parameters.Add(new SqlParameter("@AceptadaPorSunat", resp.aceptada_por_sunat));
                    cmd.Parameters.Add(new SqlParameter("@SunatDescription", resp.sunat_description));
                    cmd.Parameters.Add(new SqlParameter("@SunatNote", resp.sunat_note));

                    await cmd.ExecuteNonQueryAsync();
                }

                conn.Close();
            }
        }

        private async Task SetUpdateLocalError(FilterRequestEntity value, Respuesta resp)
        {
            using (SqlConnection conn = new SqlConnection(_cnxLocal))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(SP_SET_UPDATE_GUIA_ERROR_LOCAL, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@ObjType", value.Cod1));
                    cmd.Parameters.Add(new SqlParameter("@DocEntry", value.Id1));
                    cmd.Parameters.Add(new SqlParameter("@Error", resp.errors));

                    await cmd.ExecuteNonQueryAsync();
                }

                conn.Close();
            }
        }
    }
}
