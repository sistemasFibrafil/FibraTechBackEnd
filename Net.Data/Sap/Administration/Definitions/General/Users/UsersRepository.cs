using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace Net.Data.Sap
{
    public class UsersRepository : RepositoryBase<UsersEntity>, IUsersRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSap _db;

        public UsersRepository(IConnectionSQL context, DataContextSap db)
            : base(context)
        {
            _db = db;
        }


        public async Task<ResultadoTransaccionEntity<UsersEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UsersEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.Users
                .Where(x => x.GROUPS != 99) // Exclude user elimiated
                .Select(x => new UsersEntity
                {
                    USERID = x.USERID,
                    USER_CODE = x.USER_CODE,
                    U_NAME = x.U_NAME,
                })
                .ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                resultTransaccion.dataList = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<UsersEntity>> GetByCode(UsersEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UsersEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.Users
                .Where(n => n.USER_CODE == value.USER_CODE) // Exclude user elimiated
                .Select(n => new UsersEntity
                {
                    USERID = n.USERID,
                    USER_CODE = n.USER_CODE,
                    U_NAME = n.U_NAME,
                    Department = n.Department,
                    Branch = n.Branch,
                    E_Mail = n.E_Mail,
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
