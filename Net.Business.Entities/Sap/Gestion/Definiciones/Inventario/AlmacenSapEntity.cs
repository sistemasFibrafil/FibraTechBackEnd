namespace Net.Business.Entities.Sap
{
    public class AlmacenSapEntity
    {
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public string FullDescr { get; set; }
        public string Inactive { get; set; }
        public decimal OnHand { get; set; }
    }
}
