using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class ParametroConexionDeleteRequestDto : BaseEntity
    {
        public int IdParametroConexion { get; set; }
        public ParametroConexionEntity RetornarParametroConexion()
        {
            return new ParametroConexionEntity
            {
                IdParametroConexion = IdParametroConexion,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
