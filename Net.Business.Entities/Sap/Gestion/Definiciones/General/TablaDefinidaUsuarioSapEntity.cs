using System;
namespace Net.Business.Entities.Sap
{
    public class TablaDefinidaUsuarioSapEntity
    {
        public Int16 IndexID { get; set; }
        public string TableID { get; set; }
        public Int16 FieldID { get; set; }
        public string FldValue { get; set; }
        public string Descr { get; set; }
        public string FullDescr { get; set; }
    }
}
