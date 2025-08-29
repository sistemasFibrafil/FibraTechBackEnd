using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public class PerfilRepository : RepositoryBase<PerilEntity>, IPerfilRepository
    {
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "SEG_GetPerfilAll";
        const string SP_GET_ID = DB_ESQUEMA + "SEG_GetPerfilPorId";
        const string SP_INSERT = DB_ESQUEMA + "SEG_SetPerfilInsert";
        const string SP_DELETE = DB_ESQUEMA + "SEG_SetPerfilDelete";
        const string SP_UPDATE = DB_ESQUEMA + "SEG_SetPerfilUpdate";

        public PerfilRepository(IConnectionSQL context)
            : base(context)
        {
        }
        public Task<IEnumerable<PerilEntity>> GetAll(PerilEntity entidad)
        {
            return Task.Run(() => FindAll(entidad, SP_GET));
        }
        public Task<PerilEntity> GetById(PerilEntity entidad)
        {
            return Task.Run(() => FindById(entidad, SP_GET_ID));
        }
        public async Task<int> Create(PerilEntity entidad)
        {

            return await Task.Run(() => Create(entidad, SP_INSERT));
        }
        public Task Update(PerilEntity entidad)
        {
            return Task.Run(() => Update(entidad, SP_UPDATE));
        }
        public Task Delete(PerilEntity entidad)
        {
            return Task.Run(() => Delete(entidad, SP_DELETE));
        }
    }
}