using Net.CrossCotting;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IFacturacionElectronicaRepositoy
    {
        Task<ResultadoTransaccionResponse<FacturacionElectronicaSapEntity>> GetListGuiaElectronicaByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionResponse<FacturacionElectronicaSapEntity>> SetEnviar(FilterRequestEntity value);
    }
}
