using Net.Connection;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface IUsuarioRepository : IRepositoryBase<UsuarioEntity>
    {
        Task<ResultadoTransaccionResponse<UsuarioQueryEntity>> GetList();
        Task<ResultadoTransaccionResponse<UsuarioQueryEntity>> GetListByFilter(UsuarioFilterEntity value);
        Task<ResultadoTransaccionResponse<UsuarioEntity>> GetById(UsuarioEntity value);
        Task<ResultadoTransaccionResponse<UsuarioEntity>> Create(UsuarioCreateEntity value);
        Task<ResultadoTransaccionResponse<UsuarioEntity>> Update(UsuarioUpdateEntity value);
        Task<ResultadoTransaccionResponse<UsuarioEntity>> Delete(UsuarioEntity value);
        Task<ResultadoTransaccionResponse<UsuarioAutenticarEntity>> Autenticar(UsuarioAutenticarEntity entidad);
        Task<ResultadoTransaccionResponse<UsuarioDatosEntity>> ObtienePermisosPorUsuario(UsuarioDatosEntity entidad);
        
        Task RecuperarPassword(UsuarioRecuperarPasswordEntity entidad);
        Task<ResultadoTransaccionResponse<UsuarioTokenEntity>> ValidarToken(UsuarioTokenEntity value);
        Task<ResultadoTransaccionResponse<UsuarioEntity>> UpdatePassword(UsuarioUpdatePasswordEntity value);
    }
}
