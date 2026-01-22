using System;
using AutoMapper;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace Net.Data.Web
{
    public class LogisticUserRepository : RepositoryBase<LogisticUserEntity>, ILogisticUserRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly IMapper _mapper;
        private readonly DataContextSeg _db;

        public LogisticUserRepository(IConnectionSQL context, DataContextSeg db, IMapper mapper)
            : base(context)
        {
            _db = db;
            _mapper = mapper;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<LogisticUserQueryEntity>> GetById(LogisticUserEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<LogisticUserQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.Usuario
               .Where(p => p.IdUsuario == value.IdUsuario)
               .Select(p => new LogisticUserQueryEntity
               {
                   IdLogisticUser = p.LogisticUser.IdLogisticUser,
                   IdUsuario = p.IdUsuario,
                   IdLocation = p.LogisticUser != null ? p.LogisticUser.IdLocation ?? 0 : 0,
                   ApellidoPaterno = p.ApellidoPaterno,
                   ApellidoMaterno = p.ApellidoMaterno,
                   Nombre = p.Nombre,
                   // p.LogisticUser puede ser null -> LEFT JOIN
                   SuperUser = p.LogisticUser != null ? p.LogisticUser.SuperUser : false,
                   Blocked = p.LogisticUser != null ? p.LogisticUser.Blocked  : false
               })
               .FirstOrDefaultAsync();

                data.Permissions = await _db.LogisticUserPermission.Where(n => n.IdLogisticUser == data.IdLogisticUser).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<LogisticUserQueryEntity>> GetValidateByUser(LogisticUserValidatedFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<LogisticUserQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.LogisticUser
               .Where(p => p.Blocked == false && p.IdUsuario == value.IdUsuario)
               .Select(p => new LogisticUserQueryEntity
               {
                   IdLogisticUser = p.IdLogisticUser,
                   SuperUser = p.SuperUser,
                   Blocked =  p.Blocked
               })
               .FirstOrDefaultAsync();

                data.Permissions = await _db.LogisticUserPermission.Where(n => n.Blocked == false && n.ObjectType == value.ObjectType && n.IdLogisticUser == data.IdLogisticUser).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<LogisticUserEntity>> SetCreate(LogisticUserCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<LogisticUserEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            await using var trx = await _db.Database.BeginTransactionAsync();

            try
            {
                if (await _db.LogisticUser.AnyAsync(x => x.IdLogisticUser == value.IdLogisticUser))
                {
                    var entity = await _db.LogisticUser.Include(x => x.Permissions).FirstAsync(x => x.IdLogisticUser == value.IdLogisticUser);

                    var entry = _db.Entry(entity);
                    entry.CurrentValues.SetValues(value);
                    await _db.SaveChangesAsync();

                    // Actualizar permisos existentes
                    foreach (var permission in value.Permissions.Where(n => n.IdLogisticUserPermission != 0))
                    {
                        var perm = entity.Permissions.First(cp => cp.IdLogisticUserPermission == permission.IdLogisticUserPermission);

                        _db.Entry(perm).CurrentValues.SetValues(permission);
                    }

                    // Insertar nuevos permisos
                    foreach (var permission in value.Permissions.Where(n => n.IdLogisticUserPermission == 0))
                    {
                        var perm = _mapper.Map<LogisticUserPermissionEntity>(permission);
                        perm.IdLogisticUser = entity.IdLogisticUser;
                        await _db.LogisticUserPermission.AddAsync(perm);
                    }

                    await _db.SaveChangesAsync();
                }
                else
                {
                    var entity = _mapper.Map<LogisticUserEntity>(value);
                    await _db.LogisticUser.AddAsync(entity);
                    await _db.SaveChangesAsync();                    
                }

                await trx.CommitAsync();
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync();

                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
    }
}
