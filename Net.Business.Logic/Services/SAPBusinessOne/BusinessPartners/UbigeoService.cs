using Net.Data;
using Net.CrossCotting;
using Net.Business.Logic.Interfaces.SAPBusinessOne.BusinessPartners;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Ubigeo.Filter;
using Net.Business.Logic.Mappers.SAPBusinessOne.BusinessPartners.Ubigeo.Filter;
namespace Net.Business.Logic.Services.SAPBusinessOne.BusinessPartners
{
    public class UbigeoService
        (
            IRepositoryWrapper repository
        ) : IUbigeoService
    {
        private readonly IRepositoryWrapper _repository = repository;

        public async Task<ResultadoTransaccionResponse<object>> GetListByFilter(UbigeoFilterRequestDto dto)
        {
            var entity = UbigeoFilterMapper.ToEntity(dto);
            var result = await _repository.Ubigeo.GetListByFilter(entity);

            if (result.ResultadoCodigo == -1)
            {
                return ResponseHelper.From(result);
            }

            return new ResultadoTransaccionResponse<object>
            {
                IdRegistro = result.IdRegistro,
                ResultadoCodigo = result.ResultadoCodigo,
                ResultadoDescripcion = result.ResultadoDescripcion,
                DataList = result.DataList?.Cast<object>().ToList() ?? []
            };
        }
    }
}
