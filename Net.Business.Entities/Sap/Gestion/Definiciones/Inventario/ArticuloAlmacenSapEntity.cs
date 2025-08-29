namespace Net.Business.Entities.Sap
{
    public class ArticuloAlmacenSapEntity
    {
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public decimal OnHand { get; set; }
        public decimal IsCommited { get; set; }
        public decimal OnOrder { get; set; }
        public decimal Available { get; set; }
    }
}
