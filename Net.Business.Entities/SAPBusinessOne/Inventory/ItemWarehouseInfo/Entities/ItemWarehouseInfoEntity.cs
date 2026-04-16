namespace Net.Business.Entities.SAPBusinessOne
{
    public class ItemWarehouseInfoEntity
    {
        public string ItemCode { get; set; } = string.Empty;
        public string WhsCode { get; set; } = string.Empty;
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
        /// Ubicación de bodega
        /// </summary>
        public string? U_FIB_BinLocation { get; set; }


        // 🔹 Relación inversa: cada ItemWarehouseInfo pertenece a un artículo
        public ItemsEntity? Items { get; set; } = null;


        // 🔹 Relación inversa: cada ItemWarehouseInfo pertenece a un Warehouses
        public WarehousesEntity? Warehouses { get; set; } = null;
    }
}
