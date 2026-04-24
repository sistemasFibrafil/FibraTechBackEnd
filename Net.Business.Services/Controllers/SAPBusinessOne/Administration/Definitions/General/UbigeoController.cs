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
    public class UbigeoController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;

        public UbigeoController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet("GetList")]
        public async Task<IActionResult> GetList(string dpto, string prov, string dist)
        {
            var result = await _repository.Ubigeo.GetList(dpto, prov, dist);
            return Ok(result.dataList);
        }

        [HttpGet("GetListProvincias")]
        public async Task<IActionResult> GetListProvincias(string dpto)
        {
            var result = await _repository.Ubigeo.GetListProvincias(dpto);
            return Ok(result.dataList);
        }

        [HttpGet("GetListDistritos")]
        public async Task<IActionResult> GetListDistritos(string dpto, string prov)
        {
            var result = await _repository.Ubigeo.GetListDistritos(dpto, prov);
            return Ok(result.dataList);
        }
    }
}
