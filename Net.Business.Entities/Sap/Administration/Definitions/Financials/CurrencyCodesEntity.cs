using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.Business.Entities.Sap
{
    [Table("OCRN")]
    public class CurrencyCodesEntity
    {
        [Key]
        public string CurrCode { get; set; }
        public string CurrName { get; set; }
    }
}
