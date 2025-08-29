using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public class ParametroSistemaRepository : RepositoryBase<ParametroSistemaEntity>, IParametroSistemaRepository
    {
        const string DB_ESQUEMA = "";
        const string SP_GET_ID = DB_ESQUEMA + "SEG_GetParametroSistemaPorId";
        const string SP_INSERT = DB_ESQUEMA + "SEG_SetParametroSistemaInsert";
        const string SP_DELETE = DB_ESQUEMA + "SEG_SetParametroSistemaDelete";
        const string SP_UPDATE = DB_ESQUEMA + "SEG_SetParametroSistemaUpdate";

        public ParametroSistemaRepository(IConnectionSQL context)
            : base(context)
        {
        }
        public Task<ParametroSistemaEntity> GetById(ParametroSistemaEntity entidad)
        {
            return Task.Run(() =>
            {
                var data = FindById(entidad, SP_GET_ID);
                data.SendEmailPasswordOrigen = data.SendEmailPassword;
                data.SendEmailFinanzaPasswordOrigen = data.SendEmailFinanzaPassword;
                return data;
            });
        }
        public async Task<int> Create(ParametroSistemaEntity entidad)
        {
            return await Task.Run(() =>
            {
                entidad.SendEmailPassword = entidad.SendEmailPasswordOrigen;
                entidad.SendEmailPasswordOrigen = null;

                entidad.SendEmailFinanzaPassword = entidad.SendEmailFinanzaPasswordOrigen;
                entidad.SendEmailFinanzaPasswordOrigen = null;
                var id = Create(entidad, SP_INSERT);
                return id;
            });
        }
        public Task Update(ParametroSistemaEntity entidad)
        {
            return Task.Run(() =>
            {
                entidad.SendEmailPassword = entidad.SendEmailPasswordOrigen;
                entidad.SendEmailPasswordOrigen = null;

                entidad.SendEmailFinanzaPassword = entidad.SendEmailFinanzaPasswordOrigen;
                entidad.SendEmailFinanzaPasswordOrigen = null;
                Update(entidad, SP_UPDATE);
            });
        }
        public Task Delete(ParametroSistemaEntity entidad)
        {
            return Task.Run(() => Delete(entidad, SP_DELETE));
        }
    }
}
