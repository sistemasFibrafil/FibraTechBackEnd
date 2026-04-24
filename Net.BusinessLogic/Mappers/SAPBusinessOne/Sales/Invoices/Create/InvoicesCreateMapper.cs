using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Create;
using Net.Business.Entities.SAPBusinessOne.Sales.Invoices.Create;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.Invoices.Create
{
    public class InvoicesCreateMapper
    {
        public static InvoicesCreateEntity ToEntity(InvoicesCreateRequestDto dto)
        {
            return new InvoicesCreateEntity
            {
                DocDate = dto.DocDate,
                DocDueDate = dto.DocDueDate,
                TaxDate = dto.TaxDate,
                ReserveInvoice = dto.ReserveInvoice,
                DocType = dto.DocType,

                U_BPP_MDTD = dto.U_BPP_MDTD,
                U_BPP_MDSD = dto.U_BPP_MDSD,
                U_BPP_MDCD = dto.U_BPP_MDCD,

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

                Lines = [.. dto.Lines.Select(l => new Invoices1CreateEntity
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
                    Price = l.Price,
                    DiscPrcnt = l.DiscPrcnt,
                    PriceBefDi = l.PriceBefDi,

                    TaxCode = l.TaxCode,
                    LineTotal = l.LineTotal,

                    U_FIB_OpQtyPkg = l.U_FIB_OpQtyPkg,
                    U_tipoOpT12 = l.U_tipoOpT12
                })]
            };
        }
    }
}
