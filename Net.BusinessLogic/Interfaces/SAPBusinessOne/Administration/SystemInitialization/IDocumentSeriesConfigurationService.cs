using Net.CrossCotting;
using Net.Business.DTO.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create;
namespace Net.BusinessLogic.Interfaces.SAPBusinessOne.Administration.SystemInitialization
{
    public interface IDocumentSeriesConfigurationService
    {
        Task<ResultadoTransaccionResponse<object>> SetCreate(DocumentSeriesConfigurationCreateRequestDto dto);
    }
}
