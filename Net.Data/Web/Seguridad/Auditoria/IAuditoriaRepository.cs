using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public interface IAuditoriaRepository : IRepositoryBase<AuditoriaEntity>
    {
        Task<IEnumerable<AuditoriaEntity>> GetAll(AuditoriaFilterEntity entidad);
    }
}
