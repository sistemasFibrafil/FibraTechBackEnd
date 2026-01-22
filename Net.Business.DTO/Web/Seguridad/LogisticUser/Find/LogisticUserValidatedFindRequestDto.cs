using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class LogisticUserValidatedFindRequestDto
    {
        public string ObjectType { get; set; }
        public int IdUsuario { get; set; }

        public LogisticUserValidatedFindEntity ReturnValue()
        {
            return new LogisticUserValidatedFindEntity
            {
                ObjectType = this.ObjectType,
                IdUsuario = this.IdUsuario
            };
        }
    }
}
