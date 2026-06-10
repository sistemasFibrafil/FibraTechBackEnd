using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Departments;
namespace Net.Data.SAPBusinessOne.Administration.Definitions.General.Departments
{
    public interface IDepartmentsRepository
    {
        Task<ResultadoTransaccionResponse<DepartmentsEntity>> GetList();
    }
}
