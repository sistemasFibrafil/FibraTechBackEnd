using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.Business.Entities.Profil
{
    [Table("OPROFIB")]
    public class PesajeEntity
    {
        [Key]
        public int RECORDKEY { get; set; }
        public string ItemNo { get; set; }
        public ICollection<Pesaje1Entity> Pesaje1 { get; set; }
    }
}
