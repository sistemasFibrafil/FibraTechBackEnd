using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities
{
    [Table("OLGT")]
    public class LongitudAnchoEntity
    {
        [Key]
        public Int16 UnitCode { get; set; }
        public string UnitDisply { get; set; }
        public string UnitName { get; set; }
        public decimal SizeInMM { get; set; }
    }
}
