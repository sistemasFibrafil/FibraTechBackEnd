using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.Business.Entities.SAPBusinessOne
{
    [Table("@FIB_SECTOR")]
    public class BusinessPartnerSectorsEntity
    {
        [Key]
        [Column("Code")]
        public string Codigo { get; set; }
        [Column("Name")]
        public string Nombre { get; set; }
    }
}
