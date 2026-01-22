using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ITiempoVidaRepository
    {
        Task<ResultadoTransaccionEntity<TiempoVidaEntity>> GetList();
        Task<ResultadoTransaccionEntity<TiempoVidaEntity>> GetListByFiltro(TiempoVidaEntity value);
    }
}
