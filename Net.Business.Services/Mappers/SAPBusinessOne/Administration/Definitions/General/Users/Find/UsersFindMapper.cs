using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
{
    public class UsersFindMapper
    {
        public static UsersFindEntity ToEntity(UsersFindRequestDto dto)
        {
            return new UsersFindEntity
            {
                UserCode = dto.UserCode
            };
        }
    }
}
