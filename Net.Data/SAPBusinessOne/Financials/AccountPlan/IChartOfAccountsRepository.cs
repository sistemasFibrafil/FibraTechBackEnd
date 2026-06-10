using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IChartOfAccountsRepository
    {
        Task<ResultadoTransaccionResponse<ChartOfAccountsQueryEntity>> GetListByFilter(ChartOfAccountsFilterEntity value);
        Task<ResultadoTransaccionResponse<ChartOfAccountsQueryEntity>> GetByFormatCode(string formatCode);
    }
}
