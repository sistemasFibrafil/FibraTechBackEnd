using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class UsuarioDeleteRequestDto
    {
        public int IdUsuario { get; set; }
        public UsuarioEntity ReturnValue()
        {
            return new UsuarioEntity
            {
                IdUsuario = IdUsuario,
                Eliminado = true
            };
        }
    }
}
