using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface ISedeRepository
    {
        Task<ResultadoTransaccionEntity<SedeEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<SedeEntity>> SetAction(SedeEntity value);
    }
}
