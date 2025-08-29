using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("@OK1_T12")]
    public class TipoOperacionSapEntity
    {
        [Key]
        public string Code { get; set; }
        public string U_descrp { get; set; }
    }
}
