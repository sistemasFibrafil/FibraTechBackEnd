using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne.Approvals.ApprovalRequests.Filter;
using Net.Business.Entities.SAPBusinessOne.Approvals.ApprovalRequests.Query;
namespace Net.Data.SAPBusinessOne.Administration
{
    public interface IApprovalRequestsRepository
    {
        Task<ResultadoTransaccionResponse<ApprovalStatusReportQueryEntity>> GetListApprovalStatusReport(ApprovalStatusReportFilterEntity value);
    }
}
