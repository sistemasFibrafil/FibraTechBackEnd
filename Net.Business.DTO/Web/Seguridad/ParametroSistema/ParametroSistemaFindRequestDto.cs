using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class ParametroSistemaFindRequestDto
    {
        public int IdParametrosSistema { get; set; }

        public ParametroSistemaEntity RetornaParametroSistema()
        {
            return new ParametroSistemaEntity
            {
                IdParametrosSistema = IdParametrosSistema
            };
        }
    }
}
