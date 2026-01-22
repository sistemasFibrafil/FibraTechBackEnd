using Net.Connection;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface IUsuarioRepository : IRepositoryBase<UsuarioEntity>
    {
        Task<ResultadoTransaccionEntity<UsuarioQueryEntity>> GetList();
        Task<ResultadoTransaccionEntity<UsuarioQueryEntity>> GetListByFilter(UsuarioFilterEntity value);
        Task<ResultadoTransaccionEntity<UsuarioEntity>> GetById(UsuarioEntity value);
        Task<ResultadoTransaccionEntity<UsuarioEntity>> Create(UsuarioCreateEntity value);
        Task<ResultadoTransaccionEntity<UsuarioEntity>> Update(UsuarioUpdateEntity value);
        Task<ResultadoTransaccionEntity<UsuarioEntity>> Delete(UsuarioEntity value);
        Task<ResultadoTransaccionEntity<UsuarioAutenticarEntity>> Autenticar(UsuarioAutenticarEntity entidad);
        Task<ResultadoTransaccionEntity<UsuarioDatosEntity>> ObtienePermisosPorUsuario(UsuarioDatosEntity entidad);
        
        Task RecuperarPassword(UsuarioRecuperarPasswordEntity entidad);
        Task<ResultadoTransaccionEntity<UsuarioTokenEntity>> ValidarToken(UsuarioTokenEntity value);
        Task<ResultadoTransaccionEntity<UsuarioEntity>> UpdatePassword(UsuarioUpdatePasswordEntity value);
    }
}
