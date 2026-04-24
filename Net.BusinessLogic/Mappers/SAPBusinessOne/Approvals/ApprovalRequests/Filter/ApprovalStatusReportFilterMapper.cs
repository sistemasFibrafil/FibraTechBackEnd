using Net.Business.DTO.SAPBusinessOne.Approvals.ApprovalRequests.Filter;
using Net.Business.Entities.SAPBusinessOne.Approvals.ApprovalRequests.Filter;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Approvals.ApprovalRequests.Filter
{
    public class ApprovalStatusReportFilterMapper
    {
        public static ApprovalStatusReportFilterEntity ToEntity(ApprovalStatusReportFilterRequestDto dto)
        {
            return new ApprovalStatusReportFilterEntity
            {
                StatusOrder = dto.StatusOrder,
                StatusDraf = dto.StatusDraf,
                ObjType = dto.ObjType,

                StartAuthorOf = dto.StartAuthorOf,
                EndAuthorOf = dto.EndAuthorOf,
                StartAuthorizerOf = dto.StartAuthorizerOf,
                EndAuthorizerOf = dto.EndAuthorizerOf,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                StartCardCode = dto.StartCardCode,
                EndCardCode = dto.EndCardCode
            };
        }
    }
}
