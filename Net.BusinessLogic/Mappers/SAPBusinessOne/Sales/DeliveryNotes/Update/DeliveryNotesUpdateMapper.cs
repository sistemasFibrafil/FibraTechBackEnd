using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Update;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Update;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.DeliveryNotes.Update
{
    public class DeliveryNotesUpdateMapper
    {
        public static DeliveryNotesUpdateEntity ToEntity(DeliveryNotesUpdateRequestDto dto)
        {
            return new DeliveryNotesUpdateEntity
            {
                DocEntry = dto.DocEntry,
                DocDueDate = dto.DocDueDate,
                DocType = dto.DocType,

                U_BPP_MDTD = dto.U_BPP_MDTD,
                U_BPP_MDSD = dto.U_BPP_MDSD,
                U_BPP_MDCD = dto.U_BPP_MDCD,

                CardCode = dto.CardCode,
                CntctCode = dto.CntctCode,
                NumAtCard = dto.NumAtCard,

                PayToCode = dto.PayToCode,
                Address = dto.Address,

                GroupNum = dto.GroupNum,

                U_BPP_MDCT = dto.U_BPP_MDCT,
                U_BPP_MDRT = dto.U_BPP_MDRT,
                U_BPP_MDNT = dto.U_BPP_MDNT,
                U_FIB_CODT = dto.U_FIB_CODT,
                U_BPP_MDDT = dto.U_BPP_MDDT,

                U_FIB_TIP_TRANS = dto.U_FIB_TIP_TRANS,
                U_FIB_COD_TRA = dto.U_FIB_COD_TRA,
                U_FIB_TIPDOC_TRA = dto.U_FIB_TIPDOC_TRA,
                U_FIB_RUC_TRANS2 = dto.U_FIB_RUC_TRANS2,
                U_FIB_TRANS2 = dto.U_FIB_TRANS2,
                U_BPP_MDVC = dto.U_BPP_MDVC,
                U_FIB_TIPDOC_COND = dto.U_FIB_TIPDOC_COND,
                U_FIB_NUMDOC_COD = dto.U_FIB_NUMDOC_COD,
                U_FIB_NOM_COND = dto.U_FIB_NOM_COND,
                U_FIB_APE_COND = dto.U_FIB_APE_COND,
                U_BPP_MDFN = dto.U_BPP_MDFN,
                U_BPP_MDFC = dto.U_BPP_MDFC,

                U_RUCDestInter = dto.U_RUCDestInter,
                U_DestGuiaInter = dto.U_DestGuiaInter,
                U_DireccDestInter = dto.U_DireccDestInter,
                U_STR_NCONTENEDOR = dto.U_STR_NCONTENEDOR,
                U_STR_NPRESCINTO = dto.U_STR_NPRESCINTO,
                U_FIB_NPRESCINTO2 = dto.U_FIB_NPRESCINTO2,
                U_FIB_NPRESCINTO3 = dto.U_FIB_NPRESCINTO3,
                U_FIB_NPRESCINTO4 = dto.U_FIB_NPRESCINTO4,

                U_STR_TVENTA = dto.U_STR_TVENTA,
                U_BPP_MDMT = dto.U_BPP_MDMT,
                U_BPP_MDOM = dto.U_BPP_MDOM,

                SlpCode = dto.SlpCode,
                U_FIB_NBULTOS = dto.U_FIB_NBULTOS,
                U_FIB_KG = dto.U_FIB_KG,
                U_NroOrden = dto.U_NroOrden,
                U_OrdenCompra = dto.U_OrdenCompra,
                Comments = dto.Comments,

                U_UsrUpdate = dto.U_UsrUpdate
            };
        }
    }
}
