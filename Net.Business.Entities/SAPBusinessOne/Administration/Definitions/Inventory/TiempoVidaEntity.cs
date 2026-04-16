using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.SAPBusinessOne
{
    [Table("@FIB_OUMO")]
    public class TiempoVidaEntity
    {
        [Key]
        public string Code { get; set; }
        public string? Name { get; set; } = null;
    }
}
