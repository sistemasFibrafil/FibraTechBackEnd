using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class ParametroSistemaDeleteRequestDto : BaseEntity
    {
        public int IdParametrosSistema { get; set; }

        public ParametroSistemaEntity RetornaParametroSistema()
        {
            return new ParametroSistemaEntity
            {
                IdParametrosSistema = IdParametrosSistema,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
