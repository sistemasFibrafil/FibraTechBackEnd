using Net.CrossCotting;
using Microsoft.AspNetCore.Http;
using Net.Business.DTO.SAPBusinessOne.Drafts.Create;
using Net.Business.DTO.SAPBusinessOne.Drafts.Update;
using Net.Business.DTO.SAPBusinessOne.Drafts.CreateToDocument;
namespace Net.BusinessLogic.Interfaces.SAPBusinessOne.Draft
{
    public interface IDraftService
    {
        Task<ResultadoTransaccionResponse<object>> SetCreate(DraftsCreateRequestDto dto, IList<IFormFile> files);
        Task<ResultadoTransaccionResponse<object>> SetSaveDraftToDocument(DraftsCreateToDocumentRequestDto dto);
        Task<ResultadoTransaccionResponse<object>> SetUpdate(DraftsUpdateRequestDto dto, IList<IFormFile> files);
    }
}
