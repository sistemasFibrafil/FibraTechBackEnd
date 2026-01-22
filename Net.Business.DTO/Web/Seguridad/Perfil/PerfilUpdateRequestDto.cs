using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class PerfilUpdateRequestDto : BaseEntity
    {
        public int IdPerfil { get; set; }
        public string DescripcionPerfil { get; set; }
        public bool Activo { get; set; }
        public PerilEntity RetornarPeril()
        {
            return new PerilEntity
            {
                IdPerfil = IdPerfil,
                DescripcionPerfil = DescripcionPerfil,
                Activo = Activo,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
