using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class UsuarioFilterRequestDto
    {
        public string Filter { get; set; }

        public UsuarioFilterEntity UsuarioFind()
        {
            return new UsuarioFilterEntity
            {
                Filter = Filter
            };
        }
    }
}
