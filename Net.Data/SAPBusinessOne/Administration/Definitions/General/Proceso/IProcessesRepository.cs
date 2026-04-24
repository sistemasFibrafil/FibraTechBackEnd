using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IProcessesRepository
    {
        Task<ResultadoTransaccionResponse<ProcessesEntity>> GetList();
        Task<ResultadoTransaccionResponse<ProcessesEntity>> GetListByFiltro(ProcessesEntity value);
    }
}
