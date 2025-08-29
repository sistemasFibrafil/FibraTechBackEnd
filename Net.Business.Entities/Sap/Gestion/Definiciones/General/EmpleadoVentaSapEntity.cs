using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("OSLP")]
    public class EmpleadoVentaSapEntity
    {
        [Key]
        public int SlpCode { get; set; }
        public string SlpName { get; set; }
        public string Active { get; set; }
    }
}
