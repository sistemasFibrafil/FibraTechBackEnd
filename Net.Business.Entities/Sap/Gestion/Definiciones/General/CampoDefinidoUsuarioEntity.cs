using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.Sap
{
    [Table("CUFD")]
    public class CampoDefinidoUsuarioEntity
    {
        public string TableID { get; set; }
        public short FieldID { get; set; }
        public string AliasID { get; set; }
        public string FldValue { get; set; }
        public string Descr { get; set; }
        public ICollection<CampoDefinidoUsuario1Entity> Detalles { get; set; }
    }

    [Table("UFD1")]
    public class CampoDefinidoUsuario1Entity
    {
        public string TableID { get; set; }
        public short FieldID { get; set; }
        public short IndexID { get; set; }
        public string FldValue { get; set; }
        public string Descr { get; set; }
        public CampoDefinidoUsuarioEntity CampoDefinidoUsuario { get; set; }
    }
}
