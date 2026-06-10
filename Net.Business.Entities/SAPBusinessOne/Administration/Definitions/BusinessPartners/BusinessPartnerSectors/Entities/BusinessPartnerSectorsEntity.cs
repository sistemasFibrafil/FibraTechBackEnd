using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.Business.Entities.SAPBusinessOne
{
    [Table("@FIB_SECTOR")]
    public class BusinessPartnerSectorsEntity
    {
        [Key]
        [Column("Code")]
        public string Code { get; set; } = string.Empty;
        [Column("Name")]
        public string? Name { get; set; }
    }
}
