using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;

namespace Net.Data.Sap
{
    public interface INumeracionDocumentoRepository
    {
        Task<ResultadoTransaccionEntity<NumeracionDocumento1Entity>> GetNumero(NumeracionDocumentoEntity value);
    }
}
