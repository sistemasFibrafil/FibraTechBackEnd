using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IUserDefinedFieldsRepository
    {
        Task<ResultadoTransaccionEntity<UserDefinedFieldsQueryEntity>> GetList(UserDefinedFieldsEntity value);
        Task<ResultadoTransaccionEntity<UserDefinedFieldsQueryEntity>> GetListByFilter(UserDefinedFieldsFilterEntity value);
    }
}
