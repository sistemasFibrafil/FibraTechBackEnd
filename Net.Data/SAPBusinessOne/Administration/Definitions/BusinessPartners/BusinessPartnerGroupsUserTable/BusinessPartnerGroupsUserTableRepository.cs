using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerGroupsUserTable;
namespace Net.Data.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerGroupsUserTable
{
    public class BusinessPartnerGroupsUserTableRepository : RepositoryBase<BusinessPartnerGroupsUserTableEntity>, IBusinessPartnerGroupsUserTableRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;


        public BusinessPartnerGroupsUserTableRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }



        public async Task<ResultadoTransaccionResponse<BusinessPartnerGroupsUserTableEntity>> GetByCode(string code)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnerGroupsUserTableEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.BusinessPartnerGroupsUserTable
                .Where(n => n.Code == code)
                .Select(n => new BusinessPartnerGroupsUserTableEntity
                {
                    Code = n.Code,
                    Name = n.Name,
                    U_Prefix = n.U_Prefix
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.Data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
    }
}
