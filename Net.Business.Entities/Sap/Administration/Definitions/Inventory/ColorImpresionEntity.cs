using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("@FIB_OPCO")]
    public class ColorImpresionEntity
    {
        [Key]
        public string Code { get; set; }
        public string? Name { get; set; } = null;
    }
}
