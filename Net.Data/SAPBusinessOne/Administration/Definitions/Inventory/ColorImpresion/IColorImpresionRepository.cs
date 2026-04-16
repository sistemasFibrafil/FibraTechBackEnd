using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IColorImpresionRepository
    {
        Task<ResultadoTransaccionEntity<ColorImpresionEntity>> GetList();
        Task<ResultadoTransaccionEntity<ColorImpresionEntity>> GetListByFiltro(ColorImpresionEntity value);
    }
}
