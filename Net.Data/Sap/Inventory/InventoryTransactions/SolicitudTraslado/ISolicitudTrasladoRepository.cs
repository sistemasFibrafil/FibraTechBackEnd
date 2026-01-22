using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface ISolicitudTrasladoRepository
    {
        Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> GetListOpen();
        Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> GetListByFilter(SolicitudTrasladoFilterEntity value);
        Task<ResultadoTransaccionEntity<SolicitudTrasladoQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<SolicitudTrasladoQueryEntity>> GetToTransferenciaByDocEntry(int docEntry);
        Task<ResultadoTransaccionEntity<PickingQueryEntity>> GetListNotPicking();
        Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> SetCreate(SolicitudTrasladoCreateEntity value);
        Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> SetUpdate(SolicitudTrasladoUpdateEntity value);
        Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> SetClose(SolicitudTrasladoCloseEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetFormatoPdfByDocEntry(int id);
    }
}
