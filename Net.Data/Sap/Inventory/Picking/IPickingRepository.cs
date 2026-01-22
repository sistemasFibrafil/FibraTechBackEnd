using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IPickingRepository
    {
        Task<ResultadoTransaccionEntity<PickingQueryEntity>> GetListByFilter(PickingFilterEntity value);
        Task<ResultadoTransaccionEntity<PickingEntity>> GetListByBaseEntry(PickingEntity value);
        Task<ResultadoTransaccionEntity<PickingQueryEntity>> GetListByBaseEntryBaseType(PickingEntity value);
        Task<ResultadoTransaccionEntity<PickingEntity>> GetListByTarget(PickingEntity value);
        Task<ResultadoTransaccionEntity<SolicitudTrasladoQueryEntity>> GetToCopy(PickingCopyToFindEntity value);
        Task<ResultadoTransaccionEntity<PickingQueryEntity>> SetCreate(PickingEntity value);
        Task<ResultadoTransaccionEntity<PickingEntity>> SetRelease(PickingEntity value);
        Task<ResultadoTransaccionEntity<PickingEntity>> SetDelete(PickingEntity value);
        Task<ResultadoTransaccionEntity<PickingEntity>> SetDeleteMassive(PickingEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetPickingPrint(PickingEntity value);
    }
}
