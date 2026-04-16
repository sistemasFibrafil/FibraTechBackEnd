using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TakeInventorySparePartsDeleteRequestDto
    {
        public int DocEntry { get; set; }
        public string U_WhsCode { get; set; }
        public string U_IsDelete { get; set; }
        public int U_UsrDelete { get; set; }
        public DateTime U_DeleteDate { get; set; }
        public DateTime U_DeleteTime { get; set; }

        public TakeInventorySparePartsDeleteEntity ReturnValue()
        {
            return new TakeInventorySparePartsDeleteEntity
            {
                DocEntry = DocEntry,
                U_WhsCode = U_WhsCode,
                U_IsDelete = U_IsDelete,
                U_UsrDelete = U_UsrDelete,
                U_DeleteDate = U_DeleteDate,
                U_DeleteTime = U_DeleteTime
            };
        }
    }
}
