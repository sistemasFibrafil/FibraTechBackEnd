using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class MenuEliminarRequestDto : BaseEntity
    {
        public int IdMenu { get; set; }
        public MenuEntity RetornarMenu()
        {
            return new MenuEntity
            {
                IdMenu = IdMenu,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
