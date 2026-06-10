using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Ubigeo.Filter;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Ubigeo.Entities;
namespace Net.Data.SAPBusinessOne.BusinessPartners.Ubigeo
{
    public interface IUbigeoRepository
    {
        Task<ResultadoTransaccionResponse<UbigeoEntity>> GetListByFilter(UbigeoFilterEntity value);
    }
}
