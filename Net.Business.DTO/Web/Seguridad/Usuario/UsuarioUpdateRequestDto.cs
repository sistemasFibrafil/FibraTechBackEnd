using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class UsuarioUpdateRequestDto : BaseEntity
    {
        public int IdUsuario { get; set; }
        public int IdPerfil { get; set; }
        public int IdUserSap { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string NroDocumento { get; set; }
        public string NroTelefono { get; set; }
        public string Clave { get; set; }
        public string Email { get; set; }
        public string Imagen { get; set; }
        public string Firma { get; set; }
        public bool? ThemeDark { get; set; }
        public string ThemeColor { get; set; }
        public string TypeMenu { get; set; }
        public bool Activo { get; set; }
        public UsuarioUpdateEntity ReturnValue()
        {
            return new UsuarioUpdateEntity
            {
                IdUsuario = IdUsuario,
                IdPerfil = IdPerfil,
                IdUserSap = IdUserSap,
                Nombre = Nombre,
                ApellidoPaterno = ApellidoPaterno,
                ApellidoMaterno = ApellidoMaterno,
                NroDocumento = NroDocumento,
                NroTelefono = NroTelefono,
                Clave = Clave,
                Email = Email,
                Imagen = Imagen,
                Firma = Firma,
                ThemeDark = ThemeDark,
                ThemeColor = ThemeColor,
                TypeMenu = TypeMenu,
                Activo = Activo,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
