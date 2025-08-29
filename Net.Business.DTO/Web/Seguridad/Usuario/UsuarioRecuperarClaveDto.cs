using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class UsuarioRecuperarClaveDto
    {
        public string Sociedad { get; set; }
        public string Usuario { get; set; }

        public UsuarioRecuperarPasswordEntity RetornaUsuario()
        {
            return new UsuarioRecuperarPasswordEntity
            {
                Sociedad = Sociedad,
                Usuario = Usuario
            };
        }
    }
}
