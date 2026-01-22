using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class LogisticUserFindRequestDto
    {
        public int IdUsuario { get; set; }

        public LogisticUserEntity ReturnValue()
        {
            return new LogisticUserEntity
            {
                IdUsuario = this.IdUsuario
            };
        }
    }
}
