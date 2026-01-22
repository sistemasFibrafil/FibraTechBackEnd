using System;
namespace Net.Business.Entities.Sap
{
    public class TakeInventoryFinishedProductsCreateEntity
    {
        public string WhsCode { get; set; }
        public string CodeBar { get; set; }
        public int UsrCreate { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16 CreateTime { get; set; }
    }
}
