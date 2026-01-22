namespace Net.Business.Entities.Sap
{
    public class ItemsStockGeneralViewFilterEntity
    {
        public string WhsCode { get; set; }
        public bool ExcluirInactivo { get; set; }
        public bool ExcluirSinStock { get; set; }
    }
}
