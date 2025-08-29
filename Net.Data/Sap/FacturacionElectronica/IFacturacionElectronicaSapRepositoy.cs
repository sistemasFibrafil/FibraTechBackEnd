using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IFacturacionElectronicaSapRepositoy
    {
        Task<ResultadoTransaccionEntity<FacturacionElectronicaSapEntity>> GetListGuiaElectronicaByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<FacturacionElectronicaSapEntity>> SetEnviar(FilterRequestEntity value);
    }
}
