using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
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
                DocEntry = this.DocEntry,
                U_WhsCode = this.U_WhsCode,
                U_OnHandPhy = this.U_OnHandPhy,
                U_Difference = this.U_Difference,
                U_UsrUpdate = this.U_UsrUpdate,
                U_UpdateDate = this.U_UpdateDate,
                U_UpdateTime = this.U_UpdateTime
            };
        }
    }
}
