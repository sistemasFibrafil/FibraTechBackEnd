using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class UserDefinedFieldsFilterRequestDto
    {
        public string UserDefinedFields { get; set; }
        public string TableID { get; set; }
        public string AliasID { get; set; }

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
