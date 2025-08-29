using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("ORTT")]
    public class TipoCambioSapEntity
    {
        public decimal RateDate { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
    }
}
