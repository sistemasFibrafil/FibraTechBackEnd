using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ISubGrupoArticuloRepository
    {
        Task<ResultadoTransaccionEntity<SubGrupoArticuloEntity>> GetList();
        Task<ResultadoTransaccionEntity<SubGrupoArticuloEntity>> GetListByFiltro(SubGrupoArticuloEntity value);
    }
}
