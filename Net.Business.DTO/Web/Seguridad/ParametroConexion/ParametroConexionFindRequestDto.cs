using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class ParametroConexionFindRequestDto
    {
        public int IdParametroConexion { get; set; }
        public ParametroConexionEntity RetornarParametroConexion()
        {
            return new ParametroConexionEntity
            {
                IdParametroConexion = IdParametroConexion
            };
        }
    }
}
