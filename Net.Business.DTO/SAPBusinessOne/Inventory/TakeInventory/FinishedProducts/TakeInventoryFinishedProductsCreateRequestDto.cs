using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TakeInventoryFinishedProductsCreateRequestDto
    {
        public string WhsCode { get; set; }
        public string CodeBar { get; set; }
        public int UsrCreate { get; set; } = 0;
        public DateTime CreateDate { get; set; }
        public short CreateTime { get; set; }

        public TakeInventoryFinishedProductsCreateEntity ReturnValue()
        {
            return new TakeInventoryFinishedProductsCreateEntity
            {
                WhsCode = WhsCode,
                CodeBar = CodeBar,
                UsrCreate = UsrCreate,
                CreateDate = CreateDate,
                CreateTime = CreateTime,
            };
        }
    }
}
