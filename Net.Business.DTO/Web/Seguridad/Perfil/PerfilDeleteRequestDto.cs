using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class PerfilDeleteRequestDto : BaseEntity
    {
        public int IdPerfil { get; set; }
        public PerilEntity RetornarPeril()
        {
            return new PerilEntity
            {
                IdPerfil = IdPerfil,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
