using System.Linq;
using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public class MenuRepository : RepositoryBase<MenuEntity>, IMenuRepository
    {
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "SEG_GetMenuAll";
        const string SP_GET_ID = DB_ESQUEMA + "SEG_GetMenuPorId";
        const string SP_GET_MENU_POR_USUARIO = DB_ESQUEMA + "SEG_GetAccesoMenuxPerfil";
        const string SP_GET_OPCION_POR_USUARIO = DB_ESQUEMA + "SEG_GetAccesoOpcionesxPerfil";
        const string SP_INSERT = DB_ESQUEMA + "SEG_SetMenuInsert";
        const string SP_DELETE = DB_ESQUEMA + "SEG_SetMenuDelete";
        const string SP_UPDATE = DB_ESQUEMA + "SEG_SetMenuUpdate";

        public MenuRepository(IConnectionSQL context)
            : base(context)
        {
        }
        public Task<IEnumerable<MenuEntity>> GetAll(MenuEntity entidad)
        {
            return Task.Run(() => FindAll(entidad, SP_GET));
        }
        public Task<MenuEntity> GetById(MenuEntity entidad)
        {
            return Task.Run(() => FindById(entidad, SP_GET_ID));
        }
        public Task<IEnumerable<MenuEntity>> GetAllPorIdUsuario(int? idUsuario)
        {
            return Task.Run(() =>
            {
                IEnumerable<MenuEntity> listMenu = context.ExecuteSqlViewFindByCondition<MenuEntity>(SP_GET_MENU_POR_USUARIO, new UsuarioEntity { IdUsuario = idUsuario });

                IEnumerable<OpcionEntity> listOpcion;

                foreach (var item in listMenu)
                {
                    listOpcion = context.ExecuteSqlViewFindByCondition<OpcionEntity>(SP_GET_OPCION_POR_USUARIO, new OpcionFilterEntity { IdUsuario = idUsuario, IdMenu = item.IdMenu });
                    listMenu.FirstOrDefault(x => x.IdMenu == item.IdMenu).ListaOpciones = listOpcion;
                }

                return listMenu;
            });
        }
        public async Task<int> Create(MenuEntity entidad)
        {
            return await Task.Run(() => Create(entidad, SP_INSERT));
        }
        public Task Update(MenuEntity entidad)
        {
            return Task.Run(() => Update(entidad, SP_UPDATE));
        }
        public Task Delete(MenuEntity entidad)
        {
            return Task.Run(() => Delete(entidad, SP_DELETE));
        }
    }
}