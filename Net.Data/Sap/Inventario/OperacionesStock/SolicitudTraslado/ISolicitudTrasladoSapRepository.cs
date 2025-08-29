using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
namespace Net.Data.Sap
{
    public interface ISolicitudTrasladoSapRepository
    {
        Task<ResultadoTransaccionEntity<MemoryStream>> GetSolicitudTrasladoPdfByDocEntry(int id);
    }
}
