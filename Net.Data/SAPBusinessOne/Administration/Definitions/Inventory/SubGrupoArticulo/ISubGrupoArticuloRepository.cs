using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ISubGrupoArticuloRepository
    {
        Task<ResultadoTransaccionResponse<SubGrupoArticuloEntity>> GetList();
        Task<ResultadoTransaccionResponse<SubGrupoArticuloEntity>> GetListByFiltro(SubGrupoArticuloEntity value);
    }
}
