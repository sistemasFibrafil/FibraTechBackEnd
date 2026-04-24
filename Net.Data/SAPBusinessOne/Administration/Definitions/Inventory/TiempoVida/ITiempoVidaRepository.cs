using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ITiempoVidaRepository
    {
        Task<ResultadoTransaccionResponse<TiempoVidaEntity>> GetList();
        Task<ResultadoTransaccionResponse<TiempoVidaEntity>> GetListByFiltro(TiempoVidaEntity value);
    }
}
