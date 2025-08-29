using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("OITB")]
    public class GrupoArticuloSapEntity
    {
        [Key]
        public Int16 ItmsGrpCod { get; set; }
        public string? ItmsGrpNam { get; set; } = null;
    }
}
