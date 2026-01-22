namespace Net.Business.Entities.Sap
{
    public class WarehousesQueryEntity
    {
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public string FullDescr { get; set; }
        public string Inactive { get; set; }
        /// <summary>
        /// Stock disponible en el almacén
        /// </summary>
        public decimal OnHand { get; set; }
        /// <summary>
        /// Cantidad comprometida
        /// </summary>
        public decimal IsCommited { get; set; }
        /// <summary>
        /// Cantidad en orden de compra
        /// </summary>
        public decimal OnOrder { get; set; }
        /// <summary>
        /// Stock disponible considerando comprometido y en orden de compra
        /// </summary>
        public decimal Available { get; set; }
    }
}
