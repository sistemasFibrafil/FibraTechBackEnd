using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ISalesPersonsRepository
    {
        Task<ResultadoTransaccionResponse<SalesPersonsEntity>> GetList();
        Task<ResultadoTransaccionResponse<SalesPersonsEntity>> GetById(int id);
        Task<ResultadoTransaccionResponse<SalesPersonsEntity>> GetListByFiltro(SalesPersonsEntity value);
    }
}
