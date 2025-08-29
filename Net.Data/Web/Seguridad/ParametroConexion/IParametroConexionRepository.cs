using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface IParametroConexionRepository : IRepositoryBase<ParametroConexionEntity>
    {
        Task<ParametroConexionEntity> GetById(ParametroConexionEntity entidad);
        Task<int> Create(ParametroConexionEntity entidad);
        Task Update(ParametroConexionEntity entidad);
        Task Delete(ParametroConexionEntity entidad);
    }
}
