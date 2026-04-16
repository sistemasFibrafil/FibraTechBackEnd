using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IChartOfAccountsRepository
    {
        Task<ResultadoTransaccionEntity<ChartOfAccountsQueryEntity>> GetListByFilter(ChartOfAccountsFilterEntity value);
    }
}
