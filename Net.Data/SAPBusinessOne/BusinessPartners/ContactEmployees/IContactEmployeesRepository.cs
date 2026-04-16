using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IContactEmployeesRepository
    {
        Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> GetListByFilter(ContactEmployeesFilterEntity value);
        Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> GetById(ContactEmployeesFindEntity value);
    }
}
