using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.SAPBusinessOne
{
    [Table("@FIB_OTLA")]
    public class TipoLaminadoEntity
    {
        [Key]
        public string Code { get; set; }
        public string? Name { get; set; } = null;
    }
}
