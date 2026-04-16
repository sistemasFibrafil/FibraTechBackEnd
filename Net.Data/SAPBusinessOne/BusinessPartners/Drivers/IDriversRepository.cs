using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IDriversRepository
    {
        Task<ResultadoTransaccionEntity<DriversEntity>> GetListByFilter(DriversFilterEntity value);
        Task<ResultadoTransaccionEntity<DriversEntity>> SetCreate(DriversCreateEntity value);
    }
}
