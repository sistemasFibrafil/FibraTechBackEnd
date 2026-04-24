using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.Data;
using Net.Data.SAPBusinessOne;
using System.Threading.Tasks;

namespace Net.Business.Services.Controllers.SAPBusinessOne.Administration.Definitions.General
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatesController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;

        public StatesController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet("GetList")]
        public async Task<IActionResult> GetList(string countryCode)
        {
            var result = await _repository.States.GetList(countryCode);
            return Ok(result.dataList);
        }
    }
}
