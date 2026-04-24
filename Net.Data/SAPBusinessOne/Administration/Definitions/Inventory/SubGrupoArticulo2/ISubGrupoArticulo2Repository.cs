using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ISubGrupoArticulo2Repository
    {
        Task<ResultadoTransaccionResponse<SubGrupoArticulo2SapEntity>> GetList();
    }
}
