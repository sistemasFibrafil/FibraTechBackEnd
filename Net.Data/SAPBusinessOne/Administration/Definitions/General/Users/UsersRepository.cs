using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Find;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Query;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Filter;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Entities;
namespace Net.Data.SAPBusinessOne
{
    public class UsersRepository : RepositoryBase<UsersEntity>, IUsersRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;

        public UsersRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
        }


        public async Task<ResultadoTransaccionResponse<UsersQueryEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.Users
                .Where(x => x.GROUPS != 99) // Exclude user elimiated
                .Select(x => new UsersQueryEntity
                {
                    UserId = x.USERID,
                    UserCode = x.USER_CODE,
                    UserName = x.U_NAME
                })
                .ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
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

        public async Task<ResultadoTransaccionResponse<UsersQueryEntity>> GetListByFilter(UsersFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.Users
                .AsNoTracking();


                // FILTRO DE TEXTO
                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.USER_CODE, $"%{filter}%") ||
                        EF.Functions.Like(n.U_NAME, $"%{filter}%")
                    );
                }


                var list = await query
                .Select(n => new UsersQueryEntity
                {
                    UserId = n.USERID,
                    UserCode = n.USER_CODE,
                    UserName = n.U_NAME,
                })
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
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

        public async Task<ResultadoTransaccionResponse<UsersQueryEntity>> GetByCode(UsersFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.Users
                .Where(n => n.USER_CODE == value.UserCode) // Exclude user elimiated
                .Select(n => new UsersQueryEntity
                {
                    UserId = n.USERID,
                    UserCode = n.USER_CODE,
                    UserName = n.U_NAME,
                    Department = n.Department,
                    Branch = n.Branch,
                    Email = n.E_Mail,
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
