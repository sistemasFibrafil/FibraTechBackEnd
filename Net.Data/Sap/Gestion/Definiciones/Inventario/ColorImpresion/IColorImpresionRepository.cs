using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IColorImpresionRepository
    {
        Task<ResultadoTransaccionEntity<ColorImpresionEntity>> GetList();
        Task<ResultadoTransaccionEntity<ColorImpresionEntity>> GetListByFiltro(ColorImpresionEntity value);
    }
}
