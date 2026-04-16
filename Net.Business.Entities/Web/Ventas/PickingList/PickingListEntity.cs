using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Business.Entities.Web
{
    public class PickingListEntity
    {

        
    }

    public class PickingItem1Entity
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardName { get; set; }
        public string Contenedor { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }

    public class PickingItem2Entity
    {
        public int Id { get; set; }
        public string CodeBar1 { get; set; }
        public string CodeBar2 { get; set; }
        public string CodeBar3 { get; set; }
        public string CodeBar4 { get; set; }
        public int TotalItem { get; set; }
        public decimal PesoTotal { get; set; }
    }
}
