using Net.Connection;
using Net.CrossCotting;
using System.Threading.Tasks;
using System.Collections.Generic;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface IPerfilRepository : IRepositoryBase<PerilEntity>
    {
        Task<ResultadoTransaccionResponse<PerilEntity>> GetList();
        Task<IEnumerable<PerilEntity>> GetAll(PerilEntity entidad);
        Task<PerilEntity> GetById(PerilEntity entidad);
        Task<int> Create(PerilEntity entidad);
        Task Update(PerilEntity entidad);
        Task Delete(PerilEntity entidad);
    }
}