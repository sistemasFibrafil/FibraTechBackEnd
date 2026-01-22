using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IDireccionRepository
    {
        Task<ResultadoTransaccionEntity<DireccionEntity>> GetListByCode(DireccionEntity value);
        Task<ResultadoTransaccionEntity<DireccionEntity>> GetByCode(DireccionEntity value);
    }
}
