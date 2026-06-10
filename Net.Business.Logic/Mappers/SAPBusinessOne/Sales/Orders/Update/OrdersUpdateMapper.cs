using Net.Business.DTO.SAPBusinessOne.Sales.Orders.Update;
using Net.Business.Entities.SAPBusinessOne.Sales.Orders.Update;
using Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Update;
namespace Net.Business.Logic.Mappers.SAPBusinessOne.Sales.Orders.Update
{
    public class OrdersUpdateMapper
    {
        public static OrdersUpdateEntity ToEntity(OrdersUpdateRequestDto dto)
        {
            return new OrdersUpdateEntity
            {
                DocEntry = dto.DocEntry,
                DocDate = dto.DocDate,
                DocDueDate = dto.DocDueDate,
                TaxDate = dto.TaxDate,

                DocType = dto.DocType,
                DocStatus = dto.DocStatus,
                WddStatus = dto.WddStatus,

                CardCode = dto.CardCode,
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
                U_FIB_NEMBA = dto.U_FIB_NEMBA,
                U_FIB_DEMBA = dto.U_FIB_DEMBA,

                // 🔥 REGLA SAP
                SlpCode = dto.SlpCode,
                U_NroOrden = dto.U_NroOrden,
                U_OrdenCompra = dto.U_OrdenCompra,
                Comments = dto.Comments,

                DiscPrcnt = dto.DiscPrcnt,
                DocTotal = dto.DocTotal,

                U_UsrUpdate = dto.U_UsrUpdate,

                // 🔥 FILES
                Attachments2 = dto.Attachments2 != null ? new Attachments2UpdateEntity
                {
                    AbsEntry = dto.Attachments2.AbsEntry,
                    Lines = [..dto.Attachments2.Lines
                    .Where(l => l.Record != 3 && !string.IsNullOrWhiteSpace(l.SrcPath))
                    .Select(l => new Attachments2LinesUpdateEntity
                    {
                        AbsEntry = l.AbsEntry,
                        SrcPath = l.SrcPath,
                        TrgtPath = l.TrgtPath,
                        FileName = l.FileName,
                        FileExt = l.FileExt,
                        Date = l.Date,
                        Record = l.Record,
                    })]
                } : null,
                Lines = [.. dto.Lines.Select(l => new OrdersLinesUpdateEntity
                {
                    LineStatus = l.LineStatus,
                    LineNum = l.LineNum,
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

                    U_FIB_LinStPkg = l.U_FIB_LinStPkg,
                    U_FIB_OpQtyPkg = l.U_FIB_OpQtyPkg,
                    U_tipoOpT12 = l.U_tipoOpT12,
                    U_S_PartAranc1 = l.U_S_PartAranc1,

                    Record = l.Record
                })]
            };
        }
    }
}
