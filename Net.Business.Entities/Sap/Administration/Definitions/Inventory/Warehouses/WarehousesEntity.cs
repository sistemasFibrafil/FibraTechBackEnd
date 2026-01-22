using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    /// <summary>
    /// Almacenes
    /// </summary>
    public class WarehousesEntity
    {
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public string FullDescr { get; set; }
        /// <summary>
        /// Cuenta de existencia
        /// </summary>
        public string BalInvntAc { get; set; }
        public string Inactive { get; set; }
        public string U_FIB_ALMPRO { get; set; }

        // 🔹 Relación lógica con OACT
        public ChartOfAccountsEntity ChartOfAccounts { get; set; }


        // Relación N:1 (lado 1) - Un Warehouse puede ser default de muchos Items
        public ICollection<ItemsEntity> DefaultItems { get; set; }

        // 🔹 Relación 1:N — un Warehouses tiene muchos ItemWarehouseInfo
        public ICollection<ItemWarehouseInfoEntity> ItemWarehouseInfo { get; set; }

    }
}
