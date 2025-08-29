using Net.Connection;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
namespace Net.Data.Web
{
    public interface IPersonaRepository : IRepositoryBase<PersonaEntity>
    {
        Task<IEnumerable<PersonaEntity>> GetAll(PersonaEntity entidad);
        Task<PersonaEntity> GetById(PersonaEntity entidad);
        Task<ResultadoTransaccionEntity<PersonaEntity>> Create(PersonaEntity entidad);
        Task<ResultadoTransaccionEntity<PersonaEntity>> Update(PersonaEntity entidad);
        Task<ResultadoTransaccionEntity<PersonaEntity>> Delete(PersonaEntity entidad);
    }
}
