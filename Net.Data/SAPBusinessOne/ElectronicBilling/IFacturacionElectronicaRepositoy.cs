using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IFacturacionElectronicaRepositoy
    {
        Task<ResultadoTransaccionEntity<FacturacionElectronicaSapEntity>> GetListGuiaElectronicaByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<FacturacionElectronicaSapEntity>> SetEnviar(FilterRequestEntity value);
    }
}
