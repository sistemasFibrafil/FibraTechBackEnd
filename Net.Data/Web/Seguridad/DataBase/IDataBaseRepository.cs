using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public interface IDataBaseRepository : IRepositoryBase<DataBaseEntity>
    {
        Task<IEnumerable<DataBaseEntity>> GetAll(DataBaseEntity entidad);
    }
}
