using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Update;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Update;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Update
{
    public class InventoryTransferRequestUpdateMapper
    {
        public static InventoryTransferRequestUpdateEntity ToEntity(InventoryTransferRequestUpdateRequestDto dto)
        {
            return new InventoryTransferRequestUpdateEntity
            {
                DocEntry = dto.DocEntry,

                DocDate = dto.DocDate,
                DocDueDate = dto.DocDueDate,
                TaxDate = dto.TaxDate,

                U_FIB_IsPkg = dto.U_FIB_IsPkg,

                CardCode = dto.CardCode,

                Filler = dto.Filler,
                ToWhsCode = dto.ToWhsCode,

                U_FIB_TIP_TRAS = dto.U_FIB_TIP_TRAS,
                U_BPP_MDMT = dto.U_BPP_MDMT,
                U_BPP_MDTS = dto.U_BPP_MDTS,

                SlpCode = dto.SlpCode,
                JrnlMemo = dto.JrnlMemo,
                Comments = dto.Comments,

                U_UsrUpdate = dto.U_UsrUpdate,

                Lines = [.. dto.Lines.Select(l => new InventoryTransferRequest1UpdateEntity
                {
                    DocEntry = l.DocEntry,
                    LineNum = l.LineNum,
                    LineStatus = l.LineStatus,

                    ItemCode = l.ItemCode,
                    Dscription = l.Dscription,

                    FromWhsCod = l.FromWhsCod,
                    WhsCode = l.WhsCode,

                    UnitMsr = l.UnitMsr,
                    Quantity = l.Quantity,

                    U_FIB_LinStPkg = l.U_FIB_LinStPkg,
                    U_FIB_OpQtyPkg = l.U_FIB_OpQtyPkg,
                    U_tipoOpT12 = l.U_tipoOpT12,

                    Record = l.Record
                })]
            };
        }
    }
}
