using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ISalesPersonsRepository
    {
        Task<ResultadoTransaccionEntity<SalesPersonsEntity>> GetList();
        Task<ResultadoTransaccionEntity<SalesPersonsEntity>> GetById(int id);
        Task<ResultadoTransaccionEntity<SalesPersonsEntity>> GetListByFiltro(SalesPersonsEntity value);
    }
}
