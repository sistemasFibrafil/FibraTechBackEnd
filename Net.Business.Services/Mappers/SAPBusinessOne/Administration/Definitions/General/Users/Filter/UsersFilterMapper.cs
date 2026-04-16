using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
{
    public class UsersFilterMapper
    {
        public static UsersFilterEntity ToEntity(UsersFilterRequestDto dto)
        {
            return new UsersFilterEntity
            {
                SearchText = dto.SearchText
            };
        }
    }
}
