using System.Linq;
using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
{
    public class StockTransfersCreateMapper
    {
        public static StockTransfersCreateEntity ToEntity(StockTransfersCreateRequestDto dto)
        {
            return new StockTransfersCreateEntity
            {
                U_BPP_MDTD = dto.U_BPP_MDTD,
                U_BPP_MDSD = dto.U_BPP_MDSD,
                U_BPP_MDCD = dto.U_BPP_MDCD,

                DocDate = dto.DocDate,
                TaxDate = dto.TaxDate,

                CardCode = dto.CardCode,
                CardName = dto.CardName,
                CntctCode = dto.CntctCode,
                Address = dto.Address,

                Filler = dto.Filler,
                ToWhsCode = dto.ToWhsCode,

                U_FIB_TIP_TRANS = dto.U_FIB_TIP_TRANS,
                U_FIB_COD_TRA = dto.U_FIB_COD_TRA,
                U_FIB_TIPDOC_TRA = dto.U_FIB_TIPDOC_TRA,
                U_BPP_MDRT = dto.U_BPP_MDRT,
                U_BPP_MDNT = dto.U_BPP_MDNT,
                U_BPP_MDVC = dto.U_BPP_MDVC,

                U_FIB_TIPDOC_COND = dto.U_FIB_TIPDOC_COND,
                U_FIB_NUMDOC_COD = dto.U_FIB_NUMDOC_COD,
                U_FIB_NOM_COND = dto.U_FIB_NOM_COND,
                U_FIB_APE_COND = dto.U_FIB_APE_COND,
                U_BPP_MDFN = dto.U_BPP_MDFN,
                U_BPP_MDFC = dto.U_BPP_MDFC,

                U_FIB_TIP_TRAS = dto.U_FIB_TIP_TRAS,
                U_BPP_MDMT = dto.U_BPP_MDMT,
                U_BPP_MDTS = dto.U_BPP_MDTS,

                SlpCode = dto.SlpCode,

                U_FIB_NBULTOS = dto.U_FIB_NBULTOS,
                U_FIB_KG = dto.U_FIB_KG,

                JrnlMemo = dto.JrnlMemo,
                Comments = dto.Comments,

                U_UsrCreate = dto.U_UsrCreate,

                Lines = [.. dto.Lines.Select(l => new StockTransfers1CreateEntity
                {
                    BaseEntry = l.BaseEntry,
                    BaseLine = l.BaseLine,
                    BaseType = l.BaseType,
                    ItemCode = l.ItemCode,
                    Dscription = l.Dscription,
                    FromWhsCod = l.FromWhsCod,
                    WhsCode = l.WhsCode,
                    UnitMsr = l.UnitMsr,
                    Quantity = l.Quantity,

                    U_FIB_NBulto = l.U_FIB_NBulto,
                    U_FIB_PesoKg = l.U_FIB_PesoKg,
                    U_FIB_FromPkg = l.U_FIB_FromPkg,
                    U_tipoOpT12 = l.U_tipoOpT12,
                })],
                PickingLines = [.. dto.PickingLines.Select(l => new StockTransferPickingUpdateEntity
                {
                    DocEntry = l.DocEntry,
                    U_BaseEntry = l.U_BaseEntry,
                    U_BaseLine = l.U_BaseLine,
                    U_Status = l.U_Status,
                    U_UsrUpdate = l.U_UsrUpdate
                })]
            };
        }
    }
}
