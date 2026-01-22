using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class UsersFindRequestDto
    {
        public string USER_CODE { get; set; }

        public UsersEntity ReturnValue()
        {
            return new UsersEntity
            {
                USER_CODE = USER_CODE
            };
        }
    }
}
