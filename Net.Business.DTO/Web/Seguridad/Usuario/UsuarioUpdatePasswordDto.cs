using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class UsuarioUpdatePasswordDto : BaseEntity
    {
        public int IdUsuario { get; set; }
        public string Clave { get; set; }

        public UsuarioUpdatePasswordEntity RetornaUsuario()
        {
            return new UsuarioUpdatePasswordEntity
            {
                IdUsuario = IdUsuario,
                Clave = Clave
            };
        }
    }
}
