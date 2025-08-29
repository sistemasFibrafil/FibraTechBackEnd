using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IEmpleadoVentaSapRepository
    {
        Task<ResultadoTransaccionEntity<EmpleadoVentaSapEntity>> GetList();
        Task<ResultadoTransaccionEntity<EmpleadoVentaSapEntity>> GetById(int id);
        Task<ResultadoTransaccionEntity<EmpleadoVentaSapEntity>> GetListByFiltro(EmpleadoVentaSapEntity value);
    }
}
