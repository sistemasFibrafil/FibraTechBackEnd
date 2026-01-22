using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IPersonaContactoSapRepository
    {
        Task<ResultadoTransaccionEntity<PersonaContactoEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<PersonaContactoEntity>> GetById(FilterRequestEntity value);
    }
}
