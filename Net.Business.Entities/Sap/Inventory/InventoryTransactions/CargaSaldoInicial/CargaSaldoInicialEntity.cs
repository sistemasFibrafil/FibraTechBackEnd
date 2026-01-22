using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.Business.Entities.Sap
{
    [Table("@INCODEMOB_SALDOSINIC_INVENTARIO")]
    public class CargaSaldoInicialEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaSI { get; set; }
        public string ItemCode { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CostoTotal { get; set; }
        public string WhsCode { get; set; }
        public string Cuenta { get; set; }
    }
}
