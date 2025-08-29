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
    public class GrupoSocioNegocioSapRepository : RepositoryBase<GrupoSocioNegocioSapEntity>, IGrupoSocioNegocioSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST = DB_ESQUEMA + "GES_GetListGrupoSocioNegocioByGroupType";


        public GrupoSocioNegocioSapRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }


        public async Task<ResultadoTransaccionEntity<GrupoSocioNegocioSapEntity>> GetListByGroupType(FilterRequestEntity value)
        {
            var response = new List<GrupoSocioNegocioSapEntity>();
            var resultadoTran = new ResultadoTransaccionEntity<GrupoSocioNegocioSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTran.NombreMetodo = _metodoName;
            resultadoTran.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@GroupType", value.Cod1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<GrupoSocioNegocioSapEntity>)context.ConvertTo<GrupoSocioNegocioSapEntity>(reader);
                        }
                    }

                    resultadoTran.IdRegistro = 0;
                    resultadoTran.ResultadoCodigo = 0;
                    resultadoTran.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultadoTran.dataList = response;
                }
            }
            catch (Exception ex)
            {
                resultadoTran.IdRegistro = -1;
                resultadoTran.ResultadoCodigo = -1;
                resultadoTran.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultadoTran;
        }
    }
}
