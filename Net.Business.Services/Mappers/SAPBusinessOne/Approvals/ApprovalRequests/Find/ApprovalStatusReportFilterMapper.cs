using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
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
