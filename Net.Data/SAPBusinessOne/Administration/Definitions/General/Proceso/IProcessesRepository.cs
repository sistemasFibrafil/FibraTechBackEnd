using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IProcessesRepository
    {
        Task<ResultadoTransaccionEntity<ProcessesEntity>> GetList();
        Task<ResultadoTransaccionEntity<ProcessesEntity>> GetListByFiltro(ProcessesEntity value);
    }
}
