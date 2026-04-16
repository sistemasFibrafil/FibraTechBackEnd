using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class TakeInventorySparePartsUpdateRequestDto
    {
        public int DocEntry { get; set; }
        public string U_WhsCode { get; set; }
        public decimal U_OnHandPhy { get; set; }
        public decimal U_Difference { get; set; }
        public int U_UsrUpdate { get; set; }
        public DateTime U_UpdateDate { get; set; }
        public DateTime U_UpdateTime { get; set; }

        public TakeInventorySparePartsUpdateEntity ReturnValue()
        {
            return new TakeInventorySparePartsUpdateEntity
            {
                DocEntry = DocEntry,
                U_WhsCode = U_WhsCode,
                U_OnHandPhy = U_OnHandPhy,
                U_Difference = U_Difference,
                U_UsrUpdate = U_UsrUpdate,
                U_UpdateDate = U_UpdateDate,
                U_UpdateTime = U_UpdateTime
            };
        }
    }
}
