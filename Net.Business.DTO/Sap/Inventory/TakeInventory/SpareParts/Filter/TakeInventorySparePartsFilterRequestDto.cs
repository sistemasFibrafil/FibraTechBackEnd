using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class TakeInventorySparePartsFilterRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Usuario { get; set; }
        public string WhsCode { get; set; }
        public string Item { get; set; }
        public TakeInventorySparePartsFilterEntity ReturnValue()
        {
            return new TakeInventorySparePartsFilterEntity
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Usuario = this.Usuario,
                WhsCode = this.WhsCode,
                Item = this.Item
            };
        }
    }
}
