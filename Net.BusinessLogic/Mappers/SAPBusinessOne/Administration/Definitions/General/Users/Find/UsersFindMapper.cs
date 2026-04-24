using Net.Business.DTO.SAPBusinessOne.Administration.Definitions.General.Users.Find;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Find;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Administration.Definitions.General.Users.Find
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
