using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TakeInventorySparePartsFindRequestDto
    {
        public string U_WhsCode { get; set; }
        public int U_UsrCreate { get; set; }
        public DateTime U_CreateDate { get; set; }

        public TakeInventorySparePartsFindEntity ReturnValue()
        {
            return new TakeInventorySparePartsFindEntity
            {
                U_WhsCode = U_WhsCode,
                U_CreateDate = U_CreateDate,
                U_UsrCreate = U_UsrCreate,
            };
        }
    }
}
