using System;
using System.Linq;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
namespace Net.Business.DTO.Sap
{
    public class PurchaseRequestCreateRequestDto
    {
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime ReqDate { get; set; }
        public string DocType { get; set; }
        public int ReqType { get; set; }
        public string Requester { get; set; }
        public string ReqName { get; set; }
        public Int16 Branch { get; set; }
        public Int16 Department { get; set; }
        public string Notify { get; set; }
        public string Email { get; set; }
        public int OwnerCode { get; set; }
        public string Comments { get; set; }
        public int U_UsrCreate { get; set; }
        public List<PurchaseRequest1CreateRequestDto> Lines { get; set; } = new List<PurchaseRequest1CreateRequestDto>();

        public PurchaseRequestCreateEntity ReturnValue()
        {
            var lines = Lines.Select(line => new PurchaseRequest1CreateEntity
            {
                ItemCode = line.ItemCode,
                Dscription = line.Dscription,
                LineVendor = line.LineVendor,
                PqtReqDate = line.PqtReqDate,
                AcctCode = line.AcctCode,
                OcrCode = line.OcrCode,
                WhsCode = line.WhsCode,
                U_tipoOpT12 = line.U_tipoOpT12,
                U_FF_TIP_COM = line.U_FF_TIP_COM,
                UnitMsr = line.UnitMsr,
                Quantity = line.Quantity,
            }).ToList();

            return new PurchaseRequestCreateEntity()
            {
                DocDate = DocDate,
                DocDueDate = DocDueDate,
                TaxDate = TaxDate,
                ReqDate = ReqDate,
                ReqType = ReqType,
                Requester = Requester,
                ReqName = ReqName,
                Branch = Branch,
                Department = Department,
                Notify = Notify,
                Email = Email,
                DocType = DocType,
                OwnerCode = OwnerCode,
                Comments = Comments,
                U_UsrCreate = U_UsrCreate,
                Lines = lines
            };
        }
    }

    public class PurchaseRequest1CreateRequestDto
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string LineVendor { get; set; }
        public DateTime PqtReqDate { get; set; }
        public string AcctCode { get; set; }
        public string OcrCode { get; set; }
        public string WhsCode { get; set; }
        public string U_tipoOpT12 { get; set; }
        public string U_FF_TIP_COM { get; set; }
        public string UnitMsr { get; set; }
        public decimal Quantity { get; set; }
    }
}
