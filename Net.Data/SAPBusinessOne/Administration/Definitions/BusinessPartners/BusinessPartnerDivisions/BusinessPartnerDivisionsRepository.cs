using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerDivisions.Entities;
namespace Net.Data.SAPBusinessOne
{
    public class BusinessPartnerDivisionsRepository : RepositoryBase<BusinessPartnerDivisionsEntity>, IBusinessPartnerDivisionsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");
        private readonly DataContextSAPBusinessOne _db;

        public BusinessPartnerDivisionsRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionResponse<BusinessPartnerDivisionsEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnerDivisionsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.BusinessPartnerDivisions
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .Select(n => new BusinessPartnerDivisionsEntity
                {
                    Code = n.Code,
                    Name = n.Name,
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
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
    }
}
