using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class PersonaFindRequestDto
    {
        public int IdPersona { get; set; }
        public string Nombre { get; set; }
        public PersonaEntity RetornaPersona()
        {
            return new PersonaEntity
            {
                IdPersona = IdPersona,
                Nombre = Nombre
            };
        }
    }
}
