using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class UsuarioUpdatePasswordDto : BaseEntity
    {
        public int IdUsuario { get; set; }
        public string ClaveOrigen { get; set; }

        public UsuarioEntity RetornaUsuario()
        {
            return new UsuarioEntity
            {
                IdUsuario = IdUsuario,
                ClaveOrigen = ClaveOrigen,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
