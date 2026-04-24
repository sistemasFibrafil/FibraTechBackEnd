using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IUnidadMedidaRepository
    {
        Task<ResultadoTransaccionResponse<UnidadMedidaEntity>> GetList();
        Task<ResultadoTransaccionResponse<UnidadMedidaEntity>> GetListByFiltro(UnidadMedidaEntity value);
    }
}
