using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public interface IPerfilRepository : IRepositoryBase<PerilEntity>
    {
        Task<IEnumerable<PerilEntity>> GetAll(PerilEntity entidad);
        Task<PerilEntity> GetById(PerilEntity entidad);
        Task<int> Create(PerilEntity entidad);
        Task Update(PerilEntity entidad);
        Task Delete(PerilEntity entidad);
    }
}