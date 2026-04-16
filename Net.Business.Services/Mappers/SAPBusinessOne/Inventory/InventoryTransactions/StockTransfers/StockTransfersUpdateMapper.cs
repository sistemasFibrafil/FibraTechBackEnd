using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
{
    public class StockTransfersUpdateMapper
    {
        public static StockTransfersUpdateEntity ToEntity(StockTransfersUpdateRequestDto dto)
        {
            return new StockTransfersUpdateEntity
            {
                DocEntry = dto.DocEntry,

                U_BPP_MDTD = dto.U_BPP_MDTD,
                U_BPP_MDSD = dto.U_BPP_MDSD,
                U_BPP_MDCD = dto.U_BPP_MDCD,

                TaxDate = dto.TaxDate,

                CardCode = dto.CardCode,

                U_FIB_TIP_TRANS = dto.U_FIB_TIP_TRANS,
                U_FIB_TIPDOC_TRA = dto.U_FIB_TIPDOC_TRA,
                U_BPP_MDRT = dto.U_BPP_MDRT,
                U_BPP_MDNT = dto.U_BPP_MDNT,
                U_BPP_MDVC = dto.U_BPP_MDVC,

                U_FIB_TIPDOC_COND = dto.U_FIB_TIPDOC_COND,
                U_FIB_COD_TRA = dto.U_FIB_COD_TRA,
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

                U_UsrUpdate = dto.U_UsrUpdate
            };
        }
    }
}
