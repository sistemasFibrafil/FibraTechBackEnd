using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.Business.Entities.SAPBusinessOne
{
    [Table("OPLN")]
    public class PriceListEntity
    {
        [Key]
        [Column("ListNum")]
        public short PriceListNo { get; set; }
        [Column("ListName")]
        public string PriceListName { get; set; }
    }
}
