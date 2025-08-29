using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
namespace Net.Data.Web
{
    public interface ILecturaRepository
    {
        Task<ResultadoTransaccionEntity<LecturaEntity>> GetListByFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<LecturaEntity>> GetListByBaseTypeAndBaseEntry(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<LecturaEntity>> GetListByBaseTypeBaseEntryBaseLineFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<LecturaEntity>> GetListByTargetTypeTrgetEntryTrgetLineFiltro(FilterRequestEntity value);
        Task<ResultadoTransaccionEntity<LecturaEntity>> SetCreate(LecturaEntity value);
        Task<ResultadoTransaccionEntity<LecturaEntity>> SetDeleteMassive(LecturaEntity value);
        Task<ResultadoTransaccionEntity<LecturaEntity>> SetDelete(LecturaEntity value);
        Task<ResultadoTransaccionEntity<LecturaCopyToTransferenciaEntity>> GetLecturaCopyToTransferencia(LecturaCopyToTransferenciaFindEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetPackingListPdfByTargetTypeTrgetEntry(FilterRequestEntity value);
    }
}
