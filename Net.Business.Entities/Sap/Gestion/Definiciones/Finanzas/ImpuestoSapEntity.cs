using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("OSTC")]
    public class ImpuestoSapEntity
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public decimal Lock { get; set; }
        public decimal ValidForAR { get; set; }
    }
}
