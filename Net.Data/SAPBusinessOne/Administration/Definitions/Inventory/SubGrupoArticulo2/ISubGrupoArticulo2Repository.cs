using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ISubGrupoArticulo2Repository
    {
        Task<ResultadoTransaccionEntity<SubGrupoArticulo2SapEntity>> GetList();
    }
}
