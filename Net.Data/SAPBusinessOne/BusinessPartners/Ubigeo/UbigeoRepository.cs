using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Ubigeo.Filter;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Ubigeo.Entities;
namespace Net.Data.SAPBusinessOne.BusinessPartners.Ubigeo
{
    public class UbigeoRepository : RepositoryBase<UbigeoEntity>, IUbigeoRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;

        public UbigeoRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionResponse<UbigeoEntity>> GetListByFilter(UbigeoFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UbigeoEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.Ubigeo.AsNoTracking()
                .AsQueryable();

                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.Code, $"%{filter}%") ||
                        EF.Functions.Like(n.U_NomDepartamento, $"%{filter}%") ||
                        EF.Functions.Like(n.U_NomProvincia, $"%{filter}%") ||
                        EF.Functions.Like(n.U_NomDistrito, $"%{filter}%")
                    );
                }

                var data = await query
                .OrderBy(x => x.Code)
                .Select(x => new UbigeoEntity
                {
                    Code = x.Code,
                    U_CodDepartamento = x.U_CodDepartamento,
                    U_NomDepartamento = x.U_NomDepartamento,
                    U_NomProvincia = x.U_NomProvincia,
                    U_NomDistrito = x.U_NomDistrito
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
