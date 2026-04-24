using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IColorImpresionRepository
    {
        Task<ResultadoTransaccionResponse<ColorImpresionEntity>> GetList();
        Task<ResultadoTransaccionResponse<ColorImpresionEntity>> GetListByFiltro(ColorImpresionEntity value);
    }
}
