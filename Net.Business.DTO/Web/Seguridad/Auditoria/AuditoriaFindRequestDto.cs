using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class AuditoriaFindRequestDto
    {
        public string IdTransaccional { get; set; }
        public string Tabla { get; set; }
        public string Campo { get; set; }

        public AuditoriaFilterEntity RetornarCampos()
        {
            return new AuditoriaFilterEntity
            {
                IdTransaccional = IdTransaccional,
                Tabla = Tabla,
                Campo = Campo
            };
        }
    }
}
