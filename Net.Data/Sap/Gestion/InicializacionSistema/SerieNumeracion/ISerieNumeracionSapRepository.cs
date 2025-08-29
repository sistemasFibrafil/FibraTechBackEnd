using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;

namespace Net.Data.Sap
{
    public interface ISerieNumeracionSapRepository
    {
        Task<ResultadoTransaccionEntity<SerieNumeracionSapEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<SerieNumeracionSapEntity>> GetNumDocumentoByTipoAndSerie(FilterRequestEntity value);
    }
}
