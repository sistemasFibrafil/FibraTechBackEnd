using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ICargaSaldoInicialRepository
    {
        Task<ResultadoTransaccionEntity<CargaSaldoInicialEntity>> GetListByFilter(CargaSaldoInicialFilterEntity value);
    }
}
