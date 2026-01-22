using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IEmployeesInfoRepository
    {
        Task<ResultadoTransaccionEntity<EmployeesInfoQueryEntity>> GetList();
        Task<ResultadoTransaccionEntity<EmployeesInfoQueryEntity>> GetById(EmployeesInfoEntity value);
    }
}
