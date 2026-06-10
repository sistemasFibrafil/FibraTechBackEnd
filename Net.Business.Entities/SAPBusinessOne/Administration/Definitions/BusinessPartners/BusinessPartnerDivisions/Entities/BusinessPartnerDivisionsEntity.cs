using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerDivisions.Entities
{
    [Table("@FIB_DIVISION")]
    public class BusinessPartnerDivisionsEntity
    {
        [Key]
        [Column("Code")]
        public string Code { get; set; } = string.Empty;
        [Column("Name")]
        public string? Name { get; set; }
    }
}
