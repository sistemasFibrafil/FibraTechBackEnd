using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class OpcionPorPerfilFindRequestDto
    {
        public int IdMenu { get; set; }
        public int IdPerfil { get; set; }

        public OpcionxPerfilEntity RetornarOpcionxPerfil()
        {
            return new OpcionxPerfilEntity
            {
                IdMenu = IdMenu,
                IdPerfil = IdPerfil
            };
        }
    }
}
