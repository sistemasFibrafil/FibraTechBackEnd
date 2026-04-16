using System.Collections.Generic;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class ItemsCreateMassiveEntity
    {
        public bool IsEntrada { get; set; }
        public bool IsSalida { get; set; }
        public List<ItemsCreateMassiveLinesEntity> Lines { get; set; } = new List<ItemsCreateMassiveLinesEntity>();
    }
}
