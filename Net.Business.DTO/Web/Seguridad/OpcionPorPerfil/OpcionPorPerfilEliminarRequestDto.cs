using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class OpcionPorPerfilEliminarRequestDto : BaseEntity
    {
        public int IdOpcionxPerfil { get; set; }

        public OpcionxPerfilEntity RetornarOpcionxPerfil()
        {
            return new OpcionxPerfilEntity
            {
                IdOpcionxPerfil = IdOpcionxPerfil,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
