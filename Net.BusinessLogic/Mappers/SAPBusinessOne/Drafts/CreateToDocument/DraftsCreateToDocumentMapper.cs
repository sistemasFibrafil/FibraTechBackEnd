using Net.Business.DTO.SAPBusinessOne.Drafts.CreateToDocument;
using Net.Business.Entities.SAPBusinessOne.Drafts.CreateToDocument;
using Net.Business.Entities.SAPBusinessOne.Common.Attachments2.CreateToDocument;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Drafts.CreateToDocument
{
    public class DraftsCreateToDocumentMapper
    {
        public static DraftsCreateToDocumentEntity ToEntity(DraftsCreateToDocumentRequestDto dto)
        {
            return new DraftsCreateToDocumentEntity
            {
                DocEntry = dto.DocEntry,
                DocDate = dto.DocDate,
                DocDueDate = dto.DocDueDate,
                TaxDate = dto.TaxDate,

                DocType = dto.DocType,

                U_FIB_DocStPkg = dto.U_FIB_DocStPkg,
                U_FIB_IsPkg = dto.U_FIB_IsPkg,

                CardCode = dto.CardCode,
                CardName = dto.CardName,
                CntctCode = dto.CntctCode,
                NumAtCard = dto.NumAtCard,
                DocCur = dto.DocCur,
                DocRate = dto.DocRate,

                ShipToCode = dto.ShipToCode,
                Address2 = dto.Address2,
                PayToCode = dto.PayToCode,
                Address = dto.Address,

                GroupNum = dto.GroupNum,

                U_BPP_MDCT = dto.U_BPP_MDCT,
                U_BPP_MDRT = dto.U_BPP_MDRT,
                U_BPP_MDNT = dto.U_BPP_MDNT,
                U_FIB_CODT = dto.U_FIB_CODT,
                U_BPP_MDDT = dto.U_BPP_MDDT,

                U_TipoFlete = dto.U_TipoFlete,
                U_ValorFlete = dto.U_ValorFlete,
                U_FIB_TFLETE = dto.U_FIB_TFLETE,
                U_FIB_IMPSEG = dto.U_FIB_IMPSEG,
                U_FIB_PUERTO = dto.U_FIB_PUERTO,

                U_STR_TVENTA = dto.U_STR_TVENTA,

                // 🔥 REGLA SAP
                SlpCode = dto.SlpCode,
                U_NroOrden = dto.U_NroOrden,
                U_OrdenCompra = dto.U_OrdenCompra,
                Comments = dto.Comments,

                DiscPrcnt = dto.DiscPrcnt,
                DocTotal = dto.DocTotal,

                U_UsrCreate = dto.U_UsrCreate,

                // 🔥 FILES
                Attachments2 = dto.Attachments2 != null ? new Attachments2CreateToDocumentEntity
                {
                    AbsEntry = dto.Attachments2.AbsEntry,
                    Lines = [..dto.Attachments2.Lines.Select(l => new Attachments2LinesCreateToDocumentEntity
                    {
                        SrcPath = l.SrcPath,
                        TrgtPath = l.TrgtPath,
                        FileName = l.FileName,
                        FileExt = l.FileExt,
                        Date = l.Date
                    })]
                } : null,

                Lines = [.. dto.Lines.Select(l => new DraftsLinesCreateToDocumentEntity
                {
                    ItemCode = l.ItemCode,
                    Dscription = l.Dscription,
                    AcctCode = l.AcctCode,
                    WhsCode = l.WhsCode,

                    UnitMsr = l.UnitMsr,
                    Quantity = l.Quantity,

                    Currency = l.Currency,
                    Price = l.Price,
                    DiscPrcnt = l.DiscPrcnt,
                    PriceBefDi = l.PriceBefDi,

                    TaxCode = l.TaxCode,
                    LineTotal = l.LineTotal,

                    U_FIB_LinStPkg = l.U_FIB_LinStPkg,
                    U_FIB_OpQtyPkg = l.U_FIB_OpQtyPkg,
                    U_tipoOpT12 = l.U_tipoOpT12
                })]
            };
        }
    }
}
