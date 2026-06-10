using System.Threading.Tasks;
using Net.Business.Entities;
using Net.Business.Entities.SAPBusinessOne;
using Net.CrossCotting;
namespace Net.Data.SAPBusinessOne
{
    public interface IContactEmployeesRepository
    {
        Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> GetByCode(ContactEmployeesFindEntity value);
        Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> GetListByFilter(ContactEmployeesFilterEntity value);
        Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> GetById(ContactEmployeesFindEntity value);
        Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> GetByCardCode(string cardCode);
        Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> SetCreate(ContactEmployeesEntity value);
        Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> SetUpdate(ContactEmployeesEntity value);
        Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> SetDelete(int cntctCode);
    }
}
