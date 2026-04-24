using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Driver.Filter;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Driver.Create;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Driver.Entities;
namespace Net.Data.SAPBusinessOne
{
    public interface IDriversRepository
    {
        Task<ResultadoTransaccionResponse<DriversEntity>> GetListByFilter(DriversFilterEntity value);
        Task<ResultadoTransaccionResponse<DriversEntity>> SetCreate(DriversCreateEntity value);
    }
}
