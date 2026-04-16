using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IUserDefinedFieldsRepository
    {
        Task<ResultadoTransaccionEntity<UserDefinedFieldsQueryEntity>> GetList(UserDefinedFieldsEntity value);
        Task<ResultadoTransaccionEntity<UserDefinedFieldsQueryEntity>> GetListByFilter(UserDefinedFieldsFilterEntity value);
    }
}
