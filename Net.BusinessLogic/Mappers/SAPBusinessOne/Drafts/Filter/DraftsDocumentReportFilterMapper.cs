using Net.Business.DTO.SAPBusinessOne.Drafts.Filter;
using Net.Business.Entities.SAPBusinessOne.Drafts.Filter;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Drafts.Filter
{
    public class DraftsDocumentReportFilterMapper
    {
        public static DraftsDocumentReportFilterEntity ToEntity(DraftsDocumentReportFilterRequestDto dto)
        {
            return new DraftsDocumentReportFilterEntity
            {
                User = dto.User,
                Pending = dto.Pending,
                DraftDate = dto.DraftDate,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Orders = dto.Orders
            };
        }
    }
}
