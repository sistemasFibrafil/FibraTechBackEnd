using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne.Administration
{
    public interface IApprovalRequestsRepository
    {
        Task<ResultadoTransaccionEntity<ApprovalStatusReportQueryEntity>> GetApprovalStatusReport(ApprovalStatusReportFilterEntity value);
    }
}
