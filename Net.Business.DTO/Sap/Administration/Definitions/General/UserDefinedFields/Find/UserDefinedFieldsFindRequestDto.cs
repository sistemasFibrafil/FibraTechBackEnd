using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class UserDefinedFieldsFindRequestDto
    {
        public string TableID { get; set; }
        public string AliasID { get; set; }

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
