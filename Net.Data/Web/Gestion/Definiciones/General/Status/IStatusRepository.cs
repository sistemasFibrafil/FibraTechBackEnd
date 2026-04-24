using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface IStatusRepository
    {
        Task<ResultadoTransaccionResponse<StatusEntity>> GetList();
    }
}
