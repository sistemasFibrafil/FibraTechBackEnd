using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ITaxGroupsRepository
    {
        Task<ResultadoTransaccionEntity<TaxGroupsEntity>> GetListByFilter(string filter);
        Task<ResultadoTransaccionEntity<TaxGroupsEntity>> GetByCardCode(TaxGroupsFindEntity value);
    }
}
