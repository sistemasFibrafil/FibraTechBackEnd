using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class OpcionFindRequestDto
    {
        public int IdMenu { get; set; }

        public OpcionEntity RetornarOpcion()
        {
            return new OpcionEntity
            {
                IdMenu = IdMenu
            };
        }
    }
}
