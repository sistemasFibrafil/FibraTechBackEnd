using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class MenuActualizarRequestDto : BaseEntity
    {
        public int IdMenu { get; set; }
        public string DescripcionTitulo { get; set; }
        public string Icono { get; set; }
        public string Url { get; set; }
        public int NroNivel { get; set; }
        public bool FlgActivo { get; set; }
        public int IdMenuPadre { get; set; }
        public string NombreFormulario { get; set; }
        public MenuEntity RetornarMenu()
        {
            return new MenuEntity
            {
                IdMenu = IdMenu,
                DescripcionTitulo = DescripcionTitulo,
                Icono = Icono,
                Url = Url,
                NroNivel = NroNivel,
                FlgActivo = FlgActivo,
                IdMenuPadre = IdMenuPadre,
                NombreFormulario = NombreFormulario,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
