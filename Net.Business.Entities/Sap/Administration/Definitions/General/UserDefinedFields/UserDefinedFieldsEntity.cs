using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class UserDefinedFieldsEntity
    {
        public string TableID { get; set; }
        public short FieldID { get; set; }
        public string AliasID { get; set; }
        public string Descr { get; set; }
        public string Dflt { get; set; }
        
        public ICollection<UserDefinedFields1Entity> Lines { get; set; } = new List<UserDefinedFields1Entity>();
    }

    public class UserDefinedFields1Entity
    {
        public string TableID { get; set; }
        public short FieldID { get; set; }
        public short IndexID { get; set; }
        public string FldValue { get; set; }
        public string Descr { get; set; }
        public UserDefinedFieldsEntity CampoDefinidoUsuario { get; set; }
    }
}
