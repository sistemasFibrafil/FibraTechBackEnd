using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IEmployeesInfoRepository
    {
        Task<ResultadoTransaccionEntity<EmployeesInfoQueryEntity>> GetList();
        Task<ResultadoTransaccionEntity<EmployeesInfoQueryEntity>> GetById(EmployeesInfoEntity value);
    }
}
