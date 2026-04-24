using Net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Net.Business.DTO.SAPBusinessOne.Approvals.ApprovalRequests.Filter;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Approvals.ApprovalRequests.Filter;
namespace Net.Business.Services.Controllers.SAPBusinessOne.Approvals
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiExplorerSettings(GroupName = "ApiFibrafil")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class ApprovalRequestsController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public ApprovalRequestsController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }        


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListApprovalStatusReport([FromBody] ApprovalStatusReportFilterRequestDto dto)
        {
            var entity = ApprovalStatusReportFilterMapper.ToEntity(dto);
            var result = await _repository.ApprovalRequests.GetListApprovalStatusReport(entity);

            if (result.ResultadoCodigo == -1)
                return BadRequest(result);

            return Ok(result.dataList);
        }
    }
}
