using Net.Business.DTO.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Find;
using Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Find;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Find
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
