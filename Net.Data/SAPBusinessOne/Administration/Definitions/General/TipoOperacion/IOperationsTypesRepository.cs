using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.OperationsTypes.Query;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.OperationsTypes.Filter;
namespace Net.Data.SAPBusinessOne
{
    public interface IOperationsTypesRepository
    {
        Task<ResultadoTransaccionResponse<OperationsTypesQueryEntity>> GetList();
        Task<ResultadoTransaccionResponse<OperationsTypesQueryEntity>> GetListByFilter(OperationsTypesFilterEntity value);
        Task<ResultadoTransaccionResponse<OperationsTypesQueryEntity>> GetByCode(string code);
    }
}
