using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Create;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create
{
    public class InventoryTransferRequestCreateMapper
    {
        public static InventoryTransferRequestCreateEntity ToEntity(InventoryTransferRequestCreateRequestDto dto)
        {
            return new InventoryTransferRequestCreateEntity
            {
                DocDate = dto.DocDate,
                DocDueDate = dto.DocDueDate,
                TaxDate = dto.TaxDate,

                U_FIB_IsPkg = dto.U_FIB_IsPkg,
                U_FIB_DocStPkg = dto.U_FIB_DocStPkg,

                CardCode = dto.CardCode,
                CardName = dto.CardName,
                CntctCode = dto.CntctCode,
                Address = dto.Address,

                Filler = dto.Filler,
                ToWhsCode = dto.ToWhsCode,

                U_FIB_TIP_TRAS = dto.U_FIB_TIP_TRAS,
                U_BPP_MDMT = dto.U_BPP_MDMT,
                U_BPP_MDTS = dto.U_BPP_MDTS,

                SlpCode = dto.SlpCode,
                JrnlMemo = dto.JrnlMemo,
                Comments = dto.Comments,

                U_UsrCreate = dto.U_UsrCreate,

                Lines = [.. dto.Lines.Select(l => new InventoryTransferRequest1CreateEntity
                {
                    ItemCode = l.ItemCode,
                    Dscription = l.Dscription,

                    FromWhsCod = l.FromWhsCod,
                    WhsCode = l.WhsCode,

                    UnitMsr = l.UnitMsr,
                    Quantity = l.Quantity,

                    U_FIB_LinStPkg = l.U_FIB_LinStPkg,
                    U_FIB_OpQtyPkg = l.U_FIB_OpQtyPkg,
                    U_tipoOpT12 = l.U_tipoOpT12,
                })],
                PickingLines = [.. dto.PickingLines.Select(l => new InventoryTransferRequestPickingCreateEntity
                {
                    U_ItemCode = l.U_ItemCode,
                    U_Dscription = l.U_Dscription,
                    U_CodeBar = l.U_CodeBar,

                    U_FromWhsCod = l.U_FromWhsCod,
                    U_WhsCode = l.U_WhsCode,

                    U_UnitMsr = l.U_UnitMsr,
                    U_Quantity = l.U_Quantity,
                    U_WeightKg = l.U_WeightKg,
                    U_Status = l.U_Status
                })]
            };
        }
    }
}
