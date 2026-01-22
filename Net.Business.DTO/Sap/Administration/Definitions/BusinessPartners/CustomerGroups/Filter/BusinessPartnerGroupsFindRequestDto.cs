using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class BusinessPartnerGroupsFindRequestDto
    {
        public string GroupType { get; set; }

        public BusinessPartnerGroupsEntity ReturnValue()
        {
            return new BusinessPartnerGroupsEntity
            {
                GroupType = GroupType
            };
        }
    }
}
