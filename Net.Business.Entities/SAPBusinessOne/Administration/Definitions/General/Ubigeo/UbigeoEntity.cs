using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.Business.Entities.SAPBusinessOne
{
    [Table("@FIB_UBIGEO")]
    public class UbigeoEntity
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        
        [Column("U_FIB_DISTRITO")]
        public string Distrito { get; set; }
        
        [Column("U_FIB_PROVINCIA")]
        public string Provincia { get; set; }
        
        [Column("U_FIB_DEPARTAMENTO")]
        public string Departamento { get; set; }
    }
}
