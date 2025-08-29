using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class MenuFindRequestDto
    {
        public string DescripcionTitulo { get; set; }
        public MenuEntity RetornarMenu()
        {
            return new MenuEntity
            {
                DescripcionTitulo = DescripcionTitulo
            };
        }
    }
}
