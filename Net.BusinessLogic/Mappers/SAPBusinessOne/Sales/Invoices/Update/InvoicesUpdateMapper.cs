using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Update;
using Net.Business.Entities.SAPBusinessOne.Sales.Invoices.Update;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Sales.Invoices.Update
{
    public class InvoicesUpdateMapper
    {
        public static InvoicesUpdateEntity ToEntity(InvoicesUpdateRequestDto dto)
        {
            return new InvoicesUpdateEntity
            {
                DocEntry = dto.DocEntry,
                DocDueDate = dto.DocDueDate,
                ReserveInvoice = dto.ReserveInvoice,
                DocType = dto.DocType,

                U_BPP_MDTD = dto.U_BPP_MDTD,
                U_BPP_MDSD = dto.U_BPP_MDSD,
                U_BPP_MDCD = dto.U_BPP_MDCD,

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

                U_STR_TVENTA = dto.U_STR_TVENTA,

                // 🔥 REGLA SAP
                SlpCode = dto.SlpCode,
                U_NroOrden = dto.U_NroOrden,
                U_OrdenCompra = dto.U_OrdenCompra,
                Comments = dto.Comments,

                U_UsrUpdate = dto.U_UsrUpdate
            };
        }
    }
}
