using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ISalesPersonsRepository
    {
        Task<ResultadoTransaccionEntity<SalesPersonsEntity>> GetList();
        Task<ResultadoTransaccionEntity<SalesPersonsEntity>> GetById(int id);
        Task<ResultadoTransaccionEntity<SalesPersonsEntity>> GetListByFiltro(SalesPersonsEntity value);
    }
}
