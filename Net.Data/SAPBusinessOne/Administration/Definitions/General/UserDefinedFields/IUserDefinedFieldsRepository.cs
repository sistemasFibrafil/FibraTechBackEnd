using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IUserDefinedFieldsRepository
    {
        Task<ResultadoTransaccionResponse<UserDefinedFieldsQueryEntity>> GetList(UserDefinedFieldsEntity value);
        Task<ResultadoTransaccionResponse<UserDefinedFieldsQueryEntity>> GetListByFilter(UserDefinedFieldsFilterEntity value);
    }
}
