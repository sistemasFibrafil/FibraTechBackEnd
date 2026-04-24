using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class ContactEmployeesRepository : RepositoryBase<ContactEmployeesEntity>, IContactEmployeesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;

        public ContactEmployeesRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> GetListByFilter(ContactEmployeesFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<ContactEmployeesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.ContactEmployees
                .AsNoTracking()
                .Where(n => n.CardCode == value.CardCode);


                // FILTRO DE TEXTO
                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.Name, $"%{filter}%") ||
                        EF.Functions.Like(n.FirstName, $"%{filter}%") ||
                        EF.Functions.Like(n.MiddleName, $"%{filter}%") ||
                        EF.Functions.Like(n.LastName, $"%{filter}%")
                    );
                }


                var list = await query
                .Select(n => new ContactEmployeesQueryEntity
                {
                    CntctCode = n.CntctCode,
                    CardCode = n.CardCode,
                    Name = n.Name,
                    FullName = (n.FirstName + " " + n.MiddleName + " " + n.LastName) ?? n.Name ?? string.Empty
                })
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
                resultTransaccion.dataList = list;

            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> GetById(ContactEmployeesFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<ContactEmployeesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.ContactEmployees
                .AsNoTracking()
                .Where(n => n.CardCode == value.CardCode && n.CntctCode == value.CntctCode)
                .Select(n => new ContactEmployeesQueryEntity
                {
                    CntctCode = n.CntctCode,
                    CardCode = n.CardCode,
                    Name = n.Name,
                    FullName = (n.FirstName + " " + n.MiddleName + " " + n.LastName) ?? n.Name ?? string.Empty
                })
                .FirstOrDefaultAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;

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
