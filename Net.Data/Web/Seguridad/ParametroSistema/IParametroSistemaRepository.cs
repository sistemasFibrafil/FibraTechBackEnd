using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface IParametroSistemaRepository : IRepositoryBase<ParametroSistemaEntity>
    {
        Task<ParametroSistemaEntity> GetById(ParametroSistemaEntity entidad);
        Task<int> Create(ParametroSistemaEntity entidad);
        Task Update(ParametroSistemaEntity entidad);
        Task Delete(ParametroSistemaEntity entidad);
    }
}
