using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class UserDefinedFieldsEntity
    {
        public string TableID { get; set; } = string.Empty;
        public short FieldID { get; set; }
        public string? AliasID { get; set; }
        public string? Descr { get; set; }
        public string? Dflt { get; set; }        

        public ICollection<UserDefinedFields1Entity> Lines { get; set; } = new List<UserDefinedFields1Entity>();
    }
}
