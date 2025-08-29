using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public class OpcionxPerfilRepository : RepositoryBase<OpcionxPerfilEntity>, IOpcionxPerfilRepository
    {
        const string DB_ESQUEMA = "";
        const string SP_GET_SELECCIONADO = DB_ESQUEMA + "SEG_GetOpcionxPerfilAllSeleccionado";
        const string SP_GET_POR_SELECCIONAR = DB_ESQUEMA + "SEG_GetOpcionxPerfilAllPorSeleccionar";
        const string SP_INSERT = DB_ESQUEMA + "SEG_SetOpcionxPerfilInsert";
        const string SP_DELETE = DB_ESQUEMA + "SEG_SetOpcionxPerfilDelete";

        public OpcionxPerfilRepository(IConnectionSQL context)
            : base(context)
        {
        }
        public Task<IEnumerable<OpcionxPerfilEntity>> GetAllSeleccionado(OpcionxPerfilEntity entidad)
        {
            return Task.Run(() => FindAll(entidad, SP_GET_SELECCIONADO));
        }
        public Task<IEnumerable<OpcionxPerfilEntity>> GetAllPorSeleccionar(OpcionxPerfilEntity entidad)
        {
            return Task.Run(() => FindAll(entidad, SP_GET_POR_SELECCIONAR));
        }
        public async Task<int> Create(OpcionxPerfilEntity entidad)
        {
            return await Task.Run(() => Create(entidad, SP_INSERT));
        }
        public Task Delete(OpcionxPerfilEntity entidad)
        {
            return Task.Run(() => Delete(entidad, SP_DELETE));
        }
    }
}