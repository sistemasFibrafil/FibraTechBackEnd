using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IEmployeesInfoRepository
    {
        Task<ResultadoTransaccionResponse<EmployeesInfoQueryEntity>> GetList();
        Task<ResultadoTransaccionResponse<EmployeesInfoQueryEntity>> GetById(EmployeesInfoEntity value);
    }
}
