using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
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
                U_WhsCode = U_WhsCode,
                U_CodeBar = U_CodeBar,
                U_UsrCreate = U_UsrCreate,
                U_CreateDate = U_CreateDate,
                U_CreateTime = U_CreateTime
            };
        }
    }
}
