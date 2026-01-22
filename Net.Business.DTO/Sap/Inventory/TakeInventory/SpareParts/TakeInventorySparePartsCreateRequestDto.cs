using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class TakeInventorySparePartsCreateRequestDto
    {
        public string U_WhsCode { get; set; }
        public string U_CodeBar { get; set; }
        public int U_UsrCreate { get; set; }
        public DateTime U_CreateDate { get; set; }
        public DateTime U_CreateTime { get; set; }

        public TakeInventorySparePartsCreateEntity ReturnValue()
        {
            return new TakeInventorySparePartsCreateEntity
            {
                U_WhsCode = this.U_WhsCode,
                U_CodeBar = this.U_CodeBar,
                U_UsrCreate = this.U_UsrCreate,
                U_CreateDate = this.U_CreateDate,
                U_CreateTime = this.U_CreateTime
            };
        }
    }
}
