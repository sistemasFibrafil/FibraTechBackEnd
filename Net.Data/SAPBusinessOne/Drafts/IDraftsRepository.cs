using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Drafts.Query;
using Net.Business.Entities.SAPBusinessOne.Drafts.Filter;
using Net.Business.Entities.SAPBusinessOne.Drafts.Create;
using Net.Business.Entities.SAPBusinessOne.Drafts.Update;
using Net.Business.Entities.SAPBusinessOne.Drafts.Entities;
using Net.Business.Entities.SAPBusinessOne.Drafts.CreateToDocument;
namespace Net.Data.SAPBusinessOne
{
    public interface IDraftsRepository
    {
        Task<ResultadoTransaccionResponse<DraftsQueryEntity>> GetListDraftsDocumentReport(DraftsDocumentReportFilterEntity value);
        Task<ResultadoTransaccionResponse<DraftsQueryEntity>> GetByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<DraftsStatusQueryEntity>> GetStatusByDocEntry(int docEntry);
        Task<ResultadoTransaccionResponse<DraftsEntity>> SetCreate(DraftsCreateEntity value);
        Task<ResultadoTransaccionResponse<DraftsEntity>> SetSaveDraftToDocument(DraftsCreateToDocumentEntity value);
        Task<ResultadoTransaccionResponse<DraftsEntity>> SetUpdate(DraftsUpdateEntity value);
    }
}
