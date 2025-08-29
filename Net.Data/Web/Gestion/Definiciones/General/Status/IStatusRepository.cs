using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface IStatusRepository
    {
        Task<ResultadoTransaccionEntity<StatusEntity>> GetList();
    }
}
