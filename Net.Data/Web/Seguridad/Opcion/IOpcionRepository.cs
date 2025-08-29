using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public interface IOpcionRepository : IRepositoryBase<OpcionEntity>
    {
        Task<IEnumerable<OpcionEntity>> GetAll(OpcionEntity entidad);
        Task<OpcionEntity> GetById(OpcionEntity entidad);
        Task<int> Create(OpcionEntity entidad);
        Task Update(OpcionEntity entidad);
        Task Delete(OpcionEntity entidad);
    }
}
