using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class UserDefinedFields1Entity
    {
        public string TableID { get; set; } = string.Empty;
        public short FieldID { get; set; }
        public short IndexID { get; set; }
        public string? FldValue { get; set; }
        public string? Descr { get; set; }

        [NotMapped]
        public string? FullDescr => $"{FldValue} - {Descr}";
        public UserDefinedFieldsEntity UserDefinedFields { get; set; } = null!;
    }
}
