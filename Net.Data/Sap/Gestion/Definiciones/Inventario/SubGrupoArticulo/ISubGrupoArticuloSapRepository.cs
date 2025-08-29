using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ISubGrupoArticuloSapRepository
    {
        Task<ResultadoTransaccionEntity<SubGrupoArticuloSapEntity>> GetList();
        Task<ResultadoTransaccionEntity<SubGrupoArticuloSapEntity>> GetListByFiltro(SubGrupoArticuloSapEntity value);
    }
}
