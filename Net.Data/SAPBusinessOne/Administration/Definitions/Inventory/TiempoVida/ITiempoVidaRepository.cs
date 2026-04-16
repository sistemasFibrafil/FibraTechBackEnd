using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ITiempoVidaRepository
    {
        Task<ResultadoTransaccionEntity<TiempoVidaEntity>> GetList();
        Task<ResultadoTransaccionEntity<TiempoVidaEntity>> GetListByFiltro(TiempoVidaEntity value);
    }
}
