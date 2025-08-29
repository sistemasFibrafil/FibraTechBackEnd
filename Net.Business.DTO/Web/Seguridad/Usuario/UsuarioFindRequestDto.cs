using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class UsuarioFindRequestDto
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }

        public UsuarioEntity UsuarioFind()
        {
            return new UsuarioEntity
            {
                IdUsuario = IdUsuario,
                Usuario = Usuario,
            };
        }
    }
}
