using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ISubGrupoArticulo2SapRepository
    {
        Task<ResultadoTransaccionEntity<SubGrupoArticulo2SapEntity>> GetList();
    }
}
