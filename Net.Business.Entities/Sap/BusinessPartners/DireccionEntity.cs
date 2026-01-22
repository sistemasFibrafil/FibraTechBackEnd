using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.Business.Entities.Sap
{
    [Table("CRD1")]
    public class DireccionEntity
    {
        [Key]
        public string Address { get; set; }
        public string CardCode { get; set; }
        public string Street { get; set; }
        public int LineNum { get; set; }
        public string AdresType { get; set; }
    }
}
