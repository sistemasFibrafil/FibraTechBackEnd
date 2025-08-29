using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public class AuditoriaRepository : RepositoryBase<AuditoriaEntity>, IAuditoriaRepository
    {
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "AUD_ListarAuditoriaGet";

        public AuditoriaRepository(IConnectionSQL context)
            : base(context)
        {
        }

        public Task<IEnumerable<AuditoriaEntity>> GetAll(AuditoriaFilterEntity entidad)
        {
            return Task.Run(() => context.ExecuteSqlViewAll<AuditoriaEntity>(SP_GET, entidad));
        }
    }
}
