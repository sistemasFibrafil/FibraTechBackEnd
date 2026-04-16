using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IDepartmentsRepository
    {
        Task<ResultadoTransaccionEntity<DepartmentsEntity>> GetList();
    }
}
