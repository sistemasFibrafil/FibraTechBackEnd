using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.SAPBusinessOne
{
    [Table("OLCT")]
    public class LocationEntity
    {
        [Key]
        public int Code { get; set; }
        public string Location { get; set; }
    }
}
