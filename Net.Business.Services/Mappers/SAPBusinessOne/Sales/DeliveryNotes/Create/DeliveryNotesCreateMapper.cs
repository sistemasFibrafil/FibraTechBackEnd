using System.Linq;
using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
{
    public class DeliveryNotesCreateMapper
    {
        public static DeliveryNotesCreateEntity ToEntity(DeliveryNotesCreateRequestDto dto)
        {
            return new DeliveryNotesCreateEntity
            {
                DocType = dto.DocType,
                U_BPP_MDTD = dto.U_BPP_MDTD,
                U_BPP_MDSD = dto.U_BPP_MDSD,
                U_BPP_MDCD = dto.U_BPP_MDCD,

                DocDate = dto.DocDate,
                DocDueDate = dto.DocDueDate,
                TaxDate = dto.TaxDate,

                CardCode = dto.CardCode,
                CardName = dto.CardName,
                CntctCode = dto.CntctCode,
                NumAtCard = dto.NumAtCard,
                DocCur = dto.DocCur,
                DocRate = dto.DocRate,

                PayToCode = dto.PayToCode,
                Address = dto.Address,
                ShipToCode = dto.ShipToCode,
                Address2 = dto.Address2,

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

                DiscPrcnt = dto.DiscPrcnt,
                DocTotal = dto.DocTotal,

                U_UsrCreate = dto.U_UsrCreate,

                Lines = [.. dto.Lines.Select(l => new DeliveryNotes1CreateEntity
                {
                    BaseEntry = l.BaseEntry,
                    BaseLine = l.BaseLine,
                    BaseType = l.BaseType,

                    ItemCode = l.ItemCode,
                    Dscription = l.Dscription,
                    AcctCode = l.AcctCode,
                    WhsCode = l.WhsCode,

                    UnitMsr = l.UnitMsr,
                    Quantity = l.Quantity,

                    Currency = l.Currency,
                    PriceBefDi = l.PriceBefDi,
                    DiscPrcnt = l.DiscPrcnt,

                    Price = l.Price,
                    TaxCode = l.TaxCode,
                    LineTotal = l.LineTotal,

                    U_FIB_PesoKg = l.U_FIB_PesoKg,
                    U_FIB_NBulto = l.U_FIB_NBulto,
                    U_FIB_FromPkg = l.U_FIB_FromPkg,
                    U_tipoOpT12 = l.U_tipoOpT12
                })],
                PickingLines = [.. dto.PickingLines.Select(l => new DeliveryNotesPickingUpdateEntity
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
