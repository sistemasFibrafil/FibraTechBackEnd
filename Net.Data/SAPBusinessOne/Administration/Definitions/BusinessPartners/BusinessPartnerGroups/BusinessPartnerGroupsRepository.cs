using System;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerGroups;
namespace Net.Data.SAPBusinessOne
{
    public class BusinessPartnerGroupsRepository : RepositoryBase<BusinessPartnerGroupsEntity>, IBusinessPartnerGroupsRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;


        public BusinessPartnerGroupsRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }



        public async Task<ResultadoTransaccionResponse<BusinessPartnerGroupsEntity>> GetListByGroupType(BusinessPartnerGroupsEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnerGroupsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.BusinessPartnerGroups
                .Where(n => n.GroupType == value.GroupType)
                .OrderBy(n => n.GroupName)
                .Select(n => new BusinessPartnerGroupsEntity
                {
                    GroupCode = n.GroupCode,
                    GroupName = n.GroupName,
                })
                .ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
                resultTransaccion.DataList = list;
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
