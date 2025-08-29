using Net.Business.Entities;
using System.Threading.Tasks;
namespace Net.Data
{
    public interface ILongitudAnchoRepository
    {
        Task<ResultadoTransaccionEntity<LongitudAnchoEntity>> GetList();
    }
}
