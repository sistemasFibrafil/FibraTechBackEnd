using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class TakeInventoryFinishedProductsFindRequestDto
    {
        public string WhsCode { get; set; }
        public int UsrCreate { get; set; }
        public DateTime CreateDate { get; set; }

        public TakeInventoryFinishedProductsFindEntity ReturnValue()
        {
            return new TakeInventoryFinishedProductsFindEntity
            {
                WhsCode = WhsCode,
                CreateDate = CreateDate,
                UsrCreate = UsrCreate,
            };
        }
    }
}
