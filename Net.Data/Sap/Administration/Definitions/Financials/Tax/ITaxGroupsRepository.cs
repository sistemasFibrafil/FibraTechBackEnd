using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ITaxGroupsRepository
    {
        Task<ResultadoTransaccionEntity<TaxGroupsEntity>> GetListByFilter(string filter);
        Task<ResultadoTransaccionEntity<TaxGroupsEntity>> GetBySplCode(int slpCode);
    }
}
