using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class TakeInventoryFinishedProductsCreateRequestDto
    {
        public string WhsCode { get; set; }
        public string CodeBar { get; set; }
        public int UsrCreate { get; set; } = 0;
        public DateTime CreateDate { get; set; }
        public Int16 CreateTime { get; set; }

        public TakeInventoryFinishedProductsCreateEntity ReturnValue()
        {
            return new TakeInventoryFinishedProductsCreateEntity
            {
                WhsCode = this.WhsCode,
                CodeBar = this.CodeBar,
                UsrCreate = this.UsrCreate,
                CreateDate = CreateDate,
                CreateTime = CreateTime,
            };
        }
    }
}
