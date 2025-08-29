using Net.Connection;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public interface IOpcionxPerfilRepository : IRepositoryBase<OpcionxPerfilEntity>
    {
        Task<IEnumerable<OpcionxPerfilEntity>> GetAllSeleccionado(OpcionxPerfilEntity entidad);
        Task<IEnumerable<OpcionxPerfilEntity>> GetAllPorSeleccionar(OpcionxPerfilEntity entidad);
        Task<int> Create(OpcionxPerfilEntity entidad);
        Task Delete(OpcionxPerfilEntity entidad);
    }
}