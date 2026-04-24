using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ITaxGroupsRepository
    {
        Task<ResultadoTransaccionResponse<TaxGroupsEntity>> GetListByFilter(string filter);
        Task<ResultadoTransaccionResponse<TaxGroupsEntity>> GetByCardCode(TaxGroupsFindEntity value);
    }
}
