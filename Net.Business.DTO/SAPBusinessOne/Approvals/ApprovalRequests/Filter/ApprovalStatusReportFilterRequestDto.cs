using System;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ApprovalStatusReportFilterRequestDto
    {
        public string? StatusOrder { get; set; }
        public string? StatusDraf { get; set; }
        public string? ObjType { get; set; }

        public int? StartAuthorOf { get; set; }
        public int? EndAuthorOf { get; set; }
        public int? StartAuthorizerOf { get; set; }
        public int? EndAuthorizerOf { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? StartCardCode { get; set; }
        public string? EndCardCode { get; set; }
    }
}
