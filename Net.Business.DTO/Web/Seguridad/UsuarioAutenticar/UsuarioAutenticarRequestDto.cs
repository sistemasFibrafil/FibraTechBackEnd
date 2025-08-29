using Net.Business.Entities.Web;
using System.ComponentModel.DataAnnotations;
namespace Net.Business.DTO.Web
{
    public class UsuarioAutenticarRequestDto
    {
        [Required(ErrorMessage = "Debe Ingresar el Usuario")]
        public string Usuario { get; set; }
        [Required(ErrorMessage = "Debe Ingresar la clave")]
        public string Clave { get; set; }
        public UsuarioAutenticarEntity UsuarioAutenticar()
        {
            return new UsuarioAutenticarEntity
            {
                Usuario = Usuario,
                Clave = Clave
            };
        }
    }
}
