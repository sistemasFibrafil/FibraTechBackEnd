using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.Business.Entities.Profil
{
    [Table("OPFIBRESM")]
    public class Pesaje1Entity
    {
        [Key, Column(Order = 0)]
        public int RECORDKEY { get; set; }
        [Key, Column(Order = 1)]
        public int LineNum { get; set; }
        public string CODEBAR { get; set; }
        public decimal? PesoBob { get; set; } = null;
        public DateTime? Fchcreate { get; set; } = null;
        [ForeignKey("RECORDKEY")]
        public PesajeEntity Pesaje { get; set; }
    }
}
