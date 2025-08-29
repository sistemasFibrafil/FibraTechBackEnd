using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class DataBaseFinRequestDto
    {
        public string DescripcionDataBase { get; set; }

        public DataBaseEntity RetornaDataBase()
        {
            return new DataBaseEntity
            {
                DescripcionDataBase = DescripcionDataBase
            };
        }
    }
}
