using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
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
