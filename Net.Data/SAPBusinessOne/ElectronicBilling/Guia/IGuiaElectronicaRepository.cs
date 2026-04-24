using Net.CrossCotting;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IGuiaElectronicaRepository
    {
        Task<ResultadoTransaccionResponse<GuiaElectronicaSapEntity>> SetEnviar(FilterRequestEntity value);
    }
}
