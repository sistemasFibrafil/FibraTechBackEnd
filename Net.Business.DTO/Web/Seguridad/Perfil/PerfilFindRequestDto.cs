using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class PerfilFindRequestDto
    {
        public int IdPerfil { get; set; }
        public string DescripcionPerfil { get; set; }
        public PerilEntity RetornarPeril()
        {
            return new PerilEntity
            {
                IdPerfil = IdPerfil,
                DescripcionPerfil = DescripcionPerfil
            };
        }
    }
}
