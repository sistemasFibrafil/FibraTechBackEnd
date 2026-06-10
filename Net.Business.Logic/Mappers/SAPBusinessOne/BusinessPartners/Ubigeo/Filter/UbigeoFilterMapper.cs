using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Ubigeo.Filter;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Ubigeo.Filter;
namespace Net.Business.Logic.Mappers.SAPBusinessOne.BusinessPartners.Ubigeo.Filter
{
    public class UbigeoFilterMapper
    {
        public static UbigeoFilterEntity ToEntity(UbigeoFilterRequestDto dto)
        {
            return new UbigeoFilterEntity
            {
                SearchText = dto.SearchText
            };
        }
    }
}
