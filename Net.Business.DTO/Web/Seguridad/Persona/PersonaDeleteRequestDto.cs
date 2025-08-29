using Net.Business.Entities;
using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class PersonaDeleteRequestDto : BaseEntity
    {
        public int IdPersona { get; set; }
        public PersonaEntity RetornaPersona()
        {
            return new PersonaEntity
            {
                IdPersona = IdPersona,
                RegUsuario = RegUsuario,
                RegEstacion = RegEstacion
            };
        }
    }
}
