using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
{
    public class DocumentSeriesConfigurationFindMapper
    {
        public static DocumentSeriesConfigurationFindEntity ToEntity(DocumentSeriesConfigurationFindRequestDto dto)
        {
            return new DocumentSeriesConfigurationFindEntity
            {
                IdUsuario = dto.IdUsuario
            };
        }
    }
}
