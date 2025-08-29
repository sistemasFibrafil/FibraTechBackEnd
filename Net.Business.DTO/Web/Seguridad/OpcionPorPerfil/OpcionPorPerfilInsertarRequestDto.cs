using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class OpcionPorPerfilInsertarRequestDto : BaseEntity
    {
        public int IdOpcionxPerfil { get; set; }
        public int IdOpcion { get; set; }
        public int IdPerfil { get; set; }

        public OpcionxPerfilEntity RetornarOpcionxPerfil()
        {
            return new OpcionxPerfilEntity
            {
                IdOpcionxPerfil = IdOpcionxPerfil,
                IdOpcion = IdOpcion,
                IdPerfil = IdPerfil,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
