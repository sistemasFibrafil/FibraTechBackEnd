using Net.Business.Entities.Web;
using System.ComponentModel.DataAnnotations;
namespace Net.Business.DTO.Web
{
    public class UsuarioDatosRequestDto
    {
        [Required(ErrorMessage = "Debe Ingresar el Usuario")]
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public UsuarioDatosEntity UsuarioDatos()
        {
            return new UsuarioDatosEntity
            {
                Usuario = Usuario,
                Clave = Clave
            };
        }
    }

    public class DtoUsuarioTokenRequest
    {
        public string Usuario { get; set; }
        public string Token { get; set; }
        public UsuarioTokenEntity UsuarioDatos()
        {
            return new UsuarioTokenEntity
            {
                Usuario = Usuario,
                Token = Token
            };
        }
    }
}
