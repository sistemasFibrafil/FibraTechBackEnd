using Net.Connection;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public interface IUsuarioRepository : IRepositoryBase<UsuarioEntity>
    {
        Task<IEnumerable<UsuarioEntity>> GetAll(UsuarioEntity entidad);
        Task<UsuarioEntity> GetById(UsuarioEntity entidad);
        Task<ResultadoTransaccionEntity<UsuarioAutenticarEntity>> Autenticar(UsuarioAutenticarEntity entidad);
        Task<ResultadoTransaccionEntity<UsuarioDatosEntity>> ObtienePermisosPorUsuario(UsuarioDatosEntity entidad);
        UsuarioEntity VerificarLogin(UsuarioEntity entidad);
        Task RecuperarPassword(UsuarioRecuperarPasswordEntity entidad);
        Task<ResultadoTransaccionEntity<UsuarioTokenEntity>> ValidarToken(UsuarioTokenEntity value);
        Task UpdatePassword(UsuarioEntity entidad);
    }
}
