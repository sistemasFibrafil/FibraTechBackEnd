using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;

namespace Net.Data.SAPBusinessOne
{
    public interface IDivisionRepository
    {
        Task<ResultadoTransaccionEntity<DivisionEntity>> GetList();
    }
}
