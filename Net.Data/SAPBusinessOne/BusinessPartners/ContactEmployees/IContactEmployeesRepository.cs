using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IContactEmployeesRepository
    {
        Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> GetByCode(ContactEmployeesFindEntity value);
        Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> GetListByFilter(ContactEmployeesFilterEntity value);
        Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> GetById(ContactEmployeesFindEntity value);
        Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> GetByCardCode(string cardCode);
        Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> SetCreate(ContactEmployeesEntity value);
        Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> SetUpdate(ContactEmployeesEntity value);
        Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> SetDelete(int cntctCode);
    }
}
