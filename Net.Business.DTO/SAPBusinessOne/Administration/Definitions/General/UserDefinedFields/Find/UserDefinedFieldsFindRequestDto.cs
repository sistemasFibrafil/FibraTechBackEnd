using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class UserDefinedFieldsFindRequestDto
    {
        public string? TableID { get; set; }
        public string? AliasID { get; set; }

        public UserDefinedFieldsEntity ReturnValue()
        {
            return new UserDefinedFieldsEntity
            {
                TableID = TableID,
                AliasID = AliasID
            };
        }
    }
}
