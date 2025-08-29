using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public class DataBaseRepository : RepositoryBase<DataBaseEntity>, IDataBaseRepository
    {
        const string DB_ESQUEMA = "DBO.";
        const string SP_GET = DB_ESQUEMA + "SEG_GetDataBaseAll";
        public DataBaseRepository(IConnectionSQL context)
            : base(context)
        {

        }
        public Task<IEnumerable<DataBaseEntity>> GetAll(DataBaseEntity entidad)
        {
            return Task.Run(() => FindAll(entidad, SP_GET));
        }
    }
}
