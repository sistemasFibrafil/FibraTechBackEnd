using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public class ParametroConexionRepository : RepositoryBase<ParametroConexionEntity>, IParametroConexionRepository
    {
        const string DB_ESQUEMA = "";
        const string SP_GET_ID = DB_ESQUEMA + "SEG_GetParametroConexionPorId";
        const string SP_INSERT = DB_ESQUEMA + "SEG_SetParametroConexionInsert";
        const string SP_DELETE = DB_ESQUEMA + "SEG_SetParametroConexionDelete";
        const string SP_UPDATE = DB_ESQUEMA + "SEG_SetParametroConexionUpdate";
        public ParametroConexionRepository(IConnectionSQL context)
            : base(context)
        {
        }
        public Task<ParametroConexionEntity> GetById(ParametroConexionEntity entidad)
        {
            return Task.Run(() =>
            {

                var data = FindById(entidad, SP_GET_ID);
                //data.AplicacionPassword = null;
                data.AplicacionPasswordOriginal = data.AplicacionPassword;
                //data.SapPassword = null;
                data.SapPasswordOriginal = data.SapPassword;
                return data;
            });
        }
        public async Task<int> Create(ParametroConexionEntity entidad)
        {
            return await Task.Run(() =>
            {
                entidad.AplicacionPassword = entidad.AplicacionPasswordOriginal;
                entidad.AplicacionPasswordOriginal = null;
                entidad.SapPassword = entidad.SapPasswordOriginal;
                entidad.SapPasswordOriginal = null;
                var id = Create(entidad, SP_INSERT);
                return id;
            });
        }
        public Task Update(ParametroConexionEntity entidad)
        {
            return Task.Run(() =>
            {

                entidad.AplicacionPassword = entidad.AplicacionPasswordOriginal;
                entidad.SapPassword = entidad.SapPasswordOriginal;


                entidad.AplicacionPasswordOriginal = null;

                entidad.SapPasswordOriginal = null;

                Update(entidad, SP_UPDATE);
            });
        }
        public Task Delete(ParametroConexionEntity entidad)
        {
            return Task.Run(() => Delete(entidad, SP_DELETE));
        }
    }
}
