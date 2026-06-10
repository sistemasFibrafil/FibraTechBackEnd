using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.States;
namespace Net.Data.SAPBusinessOne.Administration.Definitions.BusinessPartners.States
{
    public class StatesRepository : RepositoryBase<StatesEntity>, IStatesRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;

        public StatesRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionResponse<StatesEntity>> GetListByCountryCode(string countryCode)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<StatesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.States.AsQueryable();

                if (!string.IsNullOrWhiteSpace(countryCode))
                {
                    query = query.Where(x => x.Country == countryCode);
                }

                var data = await query
                .OrderBy(x => x.Name)
                .Select(x => new StatesEntity
                {
                    Code = x.Code,
                    Name = x.Name
                })
                .ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                resultTransaccion.DataList = data;
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
