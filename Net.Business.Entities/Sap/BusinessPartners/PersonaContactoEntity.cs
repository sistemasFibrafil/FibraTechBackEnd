using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("OCPR")]
    public class PersonaContactoEntity
    {
        [Key]
        public int CntctCode { get; set; }
        public string CardCode { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
