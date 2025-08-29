using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public interface IMenuRepository : IRepositoryBase<MenuEntity>
    {
        Task<IEnumerable<MenuEntity>> GetAll(MenuEntity entidad);
        Task<IEnumerable<MenuEntity>> GetAllPorIdUsuario(int? idUsuario);
        Task<MenuEntity> GetById(MenuEntity entidad);
        Task<int> Create(MenuEntity entidad);
        Task Update(MenuEntity entidad);
        Task Delete(MenuEntity entidad);
    }
}
