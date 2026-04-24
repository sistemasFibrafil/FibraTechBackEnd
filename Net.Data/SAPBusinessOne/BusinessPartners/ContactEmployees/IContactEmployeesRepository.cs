using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IContactEmployeesRepository
    {
        Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> GetListByFilter(ContactEmployeesFilterEntity value);
        Task<ResultadoTransaccionResponse<ContactEmployeesQueryEntity>> GetById(ContactEmployeesFindEntity value);
    }
}
