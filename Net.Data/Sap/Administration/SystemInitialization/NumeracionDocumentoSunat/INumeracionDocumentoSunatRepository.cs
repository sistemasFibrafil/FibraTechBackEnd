using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;

namespace Net.Data.Sap
{
    public interface INumeracionDocumentoSunatRepository
    {
        Task<ResultadoTransaccionEntity<NumeracionDocumentoSunatEntity>> GetListSerieDocumento(NumeracionDocumentoSunatEntity value);
        Task<ResultadoTransaccionEntity<NumeracionDocumentoSunatEntity>> GetNumeroDocumentoByTipoSerie(NumeracionDocumentoSunatEntity value);
    }
}
