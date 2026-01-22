using System.Collections.Generic;
namespace Net.Business.Entities.Sap
{
    public class ItemsCreateMassiveEntity
    {
        public bool IsEntrada { get; set; }
        public bool IsSalida { get; set; }
        public List<ItemsEntity> Line { get; set; } = new List<ItemsEntity>();
    }
}
