using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class PerfilInsertarRequestDto : BaseEntity
    {
        public int IdPerfil { get; set; }
        public string DescripcionPerfil { get; set; }
        public string CodigoTablaVisualizacion { get; set; }
        public bool FlgActivo { get; set; }
        public PerilEntity RetornarPeril()
        {
            return new PerilEntity
            {
                IdPerfil = IdPerfil,
                DescripcionPerfil = DescripcionPerfil,
                CodigoTablaVisualizacion = CodigoTablaVisualizacion,
                FlgActivo = FlgActivo,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
