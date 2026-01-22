using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
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
                DocEntry = this.DocEntry,
                U_WhsCode = this.U_WhsCode,
                U_IsDelete = this.U_IsDelete,
                U_UsrDelete = this.U_UsrDelete,
                U_DeleteDate = this.U_DeleteDate,
                U_DeleteTime = this.U_DeleteTime
            };
        }
    }
}
