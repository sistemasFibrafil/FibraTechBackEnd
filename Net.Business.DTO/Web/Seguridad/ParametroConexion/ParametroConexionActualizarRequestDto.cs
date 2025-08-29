using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class ParametroConexionActualizarRequestDto : BaseEntity
    {
        public int IdParametroConexion { get; set; }
        public string AplicacionServidor { get; set; }
        public string AplicacionBaseDatos { get; set; }
        public string AplicacionPasswordOriginal { get; set; }
        public string AplicacionUsuario { get; set; }
        public string SapServidor { get; set; }
        public string SapBaseDatos { get; set; }
        public string SapUsuario { get; set; }
        public string SapPasswordOriginal { get; set; }
        public ParametroConexionEntity RetornarParametroConexion()
        {
            return new ParametroConexionEntity
            {
                IdParametroConexion = IdParametroConexion,
                AplicacionServidor = AplicacionServidor,
                AplicacionBaseDatos = AplicacionBaseDatos,
                AplicacionUsuario = AplicacionUsuario,
                AplicacionPasswordOriginal = AplicacionPasswordOriginal,
                SapServidor = SapServidor,
                SapBaseDatos = SapBaseDatos,
                SapUsuario = SapUsuario,
                SapPasswordOriginal = SapPasswordOriginal,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
