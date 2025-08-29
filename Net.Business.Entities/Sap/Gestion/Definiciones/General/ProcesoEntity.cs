using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("@FIB_PROC")]
    public class ProcesoEntity
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
