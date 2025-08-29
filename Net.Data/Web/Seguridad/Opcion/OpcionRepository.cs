using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public class OpcionRepository : RepositoryBase<OpcionEntity>, IOpcionRepository
    {
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "SEG_GetOpcionAll";

        const string SP_GET_ID = DB_ESQUEMA + "SEG_GetOpcionPorId";
        const string SP_INSERT = DB_ESQUEMA + "SEG_SetOpcionInsert";
        const string SP_DELETE = DB_ESQUEMA + "SEG_SetOpcionDelete";
        const string SP_UPDATE = DB_ESQUEMA + "SEG_SetOpcionUpdate";

        public OpcionRepository(IConnectionSQL context)
            : base(context)
        {
        }
        public Task<IEnumerable<OpcionEntity>> GetAll(OpcionEntity entidad)
        {
            return Task.Run(() => FindAll(entidad, SP_GET));
        }
        public Task<OpcionEntity> GetById(OpcionEntity entidad)
        {
            return Task.Run(() => FindById(entidad, SP_GET_ID));
        }
        public async Task<int> Create(OpcionEntity entidad)
        {
            return await Task.Run(() => Create(entidad, SP_INSERT));
        }
        public Task Update(OpcionEntity entidad)
        {
            return Task.Run(() => Update(entidad, SP_UPDATE));
        }
        public Task Delete(OpcionEntity entidad)
        {
            return Task.Run(() => Delete(entidad, SP_DELETE));
        }
    }
}