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
    public class CountriesController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;

        public CountriesController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet("GetList")]
        public async Task<IActionResult> GetList()
        {
            var result = await _repository.Countries.GetList();
            return Ok(result.dataList);
        }
    }
}
