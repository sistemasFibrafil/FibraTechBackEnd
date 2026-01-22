using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("OCTG")]
    public class PaymentTermsTypesEntity
    {
        [Key]
        public Int16 GroupNum { get; set; }
        public string PymntGroup { get; set; }
    }
}
