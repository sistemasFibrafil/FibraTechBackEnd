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
    public class SectorSocioNegocioSapRepository : RepositoryBase<SectorSocioNegocioSapEntity>, ISectorSocioNegocioSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST = DB_ESQUEMA + "GES_GetListSectorSocioNegocio";


        public SectorSocioNegocioSapRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }


        public async Task<ResultadoTransaccionEntity<SectorSocioNegocioSapEntity>> GetList()
        {
            var response = new List<SectorSocioNegocioSapEntity>();
            var resultadoTran = new ResultadoTransaccionEntity<SectorSocioNegocioSapEntity>();

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

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<SectorSocioNegocioSapEntity>)context.ConvertTo<SectorSocioNegocioSapEntity>(reader);
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
