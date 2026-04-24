using Net.Business.DTO.SAPBusinessOne.Administration.Definitions.General.Users.Filter;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Filter;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Administration.Definitions.General.Users.Filter
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
