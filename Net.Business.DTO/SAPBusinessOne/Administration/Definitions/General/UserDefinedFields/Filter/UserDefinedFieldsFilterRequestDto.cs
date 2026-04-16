using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class UserDefinedFieldsFilterRequestDto
    {
        public string? UserDefinedFields { get; set; }
        public string? TableID { get; set; }
        public string? AliasID { get; set; }

        public UserDefinedFieldsFilterEntity ReturnValue()
        {
            return new UserDefinedFieldsFilterEntity
            {
                UserDefinedFields = UserDefinedFields,
                TableID = TableID,
                AliasID = AliasID
            };
        }
    }
}
