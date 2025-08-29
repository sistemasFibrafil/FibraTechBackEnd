using System;
using System.Data;
using Net.Connection;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
namespace Net.Data.Sap
{
    public class AlmacenSapRepository : RepositoryBase<AlmacenSapEntity>, IAlmacenSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_ALMACEN_PRODUCCION = DB_ESQUEMA + "GES_GetListAlmacenProduccion";
        const string SP_GET_ALMACEN_BY_CODE = DB_ESQUEMA + "GES_GetAlmacenByCode";
        const string SP_GET_LIST_ALMACEN_BY_FILTRO = DB_ESQUEMA + "GES_GetListAlmacenByFiltro";
        const string SP_GET_LIST_ALMACEN_BY_ESTADO = DB_ESQUEMA + "GES_GetListAlmacenByEstado";
        const string SP_GET_LIST_ALMACEN_BY_WHSCODE_ITEMCODE = DB_ESQUEMA + "GES_GetListAlmacenByWhsCodeAndItemCode";
        const string SP_GET_ALAMCEN_EXISTE = DB_ESQUEMA + "GES_GetAlmaceExisteByCodeSede";


        public AlmacenSapRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }


        public async Task<ResultadoTransaccionEntity<AlmacenSapEntity>> GetListAlmacenProduccion()
        {
            var response = new List<AlmacenSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<AlmacenSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_ALMACEN_PRODUCCION, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<AlmacenSapEntity>)context.ConvertTo<AlmacenSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<AlmacenSapEntity>> GetByCode(FilterRequestEntity value)
        {
            var response = new AlmacenSapEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<AlmacenSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_ALMACEN_BY_CODE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@WhsCode", value.Cod1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<AlmacenSapEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                    resultTransaccion.data = response;
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
        public async Task<ResultadoTransaccionEntity<AlmacenSapEntity>> GetListByFiltro(FilterRequestEntity value)
        {
            var response = new List<AlmacenSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<AlmacenSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_ALMACEN_BY_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Demandante", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@ItemCode", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@Inactive", value.Cod3));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<AlmacenSapEntity>)context.ConvertTo<AlmacenSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<AlmacenSapEntity>> GetListByEstado(FilterRequestEntity value)
        {
            var response = new List<AlmacenSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<AlmacenSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_ALMACEN_BY_ESTADO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Inactive", value.Cod1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<AlmacenSapEntity>)context.ConvertTo<AlmacenSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<ArticuloAlmacenSapEntity>> GetListByWhsCodeAndItemCode(FilterRequestEntity value)
        {
            var response = new List<ArticuloAlmacenSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloAlmacenSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_ALMACEN_BY_WHSCODE_ITEMCODE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@WhsCode", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@ItemCode", value.Cod2));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<ArticuloAlmacenSapEntity>)context.ConvertTo<ArticuloAlmacenSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<AlmacenSapEntity>> GetExisteByCodeAndSede(FilterRequestEntity value)
        {
            var response = new AlmacenSapEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<AlmacenSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_ALAMCEN_EXISTE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@WhsCode", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@CodSede", value.Id1));

                        var existe = await cmd.ExecuteScalarAsync();

                        if (!(bool)existe)
                        {
                            resultTransaccion.IdRegistro = -1;
                            resultTransaccion.ResultadoCodigo = -1;
                            resultTransaccion.ResultadoDescripcion = string.Format("El almacén <b> {0} </b> no corresponde a su Locación. Seleccione un almacén válido.", value.Cod1);
                            return resultTransaccion;
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "El almacén existe.";
                    resultTransaccion.data = response;
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
