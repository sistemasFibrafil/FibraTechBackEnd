using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.CrossCotting;
using Net.Data.AppContext;
using Newtonsoft.Json;
namespace Net.Data.SAPBusinessOne.Administration
{
    public class ApprovalRequestsRepository : RepositoryBase<ApprovalRequestsEntity>, IApprovalRequestsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_APPROVAL_STATUS = DB_ESQUEMA + "APR_GetListApprovalStatus";

        public ApprovalRequestsRepository(IConnectionSQL context, IConfiguration configuration, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
            _cnxSap = Utilidades.GetCon(_configuration, "EntornoConnectionSap:Entorno");
        }


        //public async Task<ResultadoTransaccionEntity<ApprovalStatusReportQueryEntity>> GetApprovalStatusReport(ApprovalStatusReportFilterEntity value)
        //{
        //    var resultTransaccion = new ResultadoTransaccionEntity<ApprovalStatusReportQueryEntity>
        //    {
        //        NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
        //        NombreAplicacion = _aplicacionName
        //    };

        //    try
        //    {
        //        var statusDraftList = new List<string>();
        //        var statusOrderList = new List<string>();


        //        if (value.Pending) statusDraftList.Add("W");
        //        if (value.Authorized) statusDraftList.Add("Y");
        //        if (value.Rejected) statusDraftList.Add("N");
        //        if (value.Canceled) statusDraftList.Add("C");
        //        if (value.CreatedBy) statusOrderList.Add("P");
        //        if (value.CreatedByResponsibleAuthorization) statusOrderList.Add("A");


        //        var objTypeList = new List<string>();
        //        if (value.Quotations) objTypeList.Add("23");
        //        if (value.Orders) objTypeList.Add("17");


        //        var startDate = value.StartDate?.Date;
        //        var endDate = value.EndDate?.Date;



        //        var list1 = await (
        //            from t01 in _db.ApprovalRequests
        //            join t02 in _db.ApprovalRequestsLines on t01.WddCode equals t02.WddCode
        //            join t03 in _db.Orders on t01.DocEntry equals t03.DocEntry
        //            join t04 in _db.Users on t01.OwnerID equals t04.USERID

        //            where
        //                (t01.IsDraft == "N")
        //                // 🔥 STATUS (opcional)
        //                && (!statusOrderList.Any() || statusOrderList.Contains(t03.WddStatus))

        //                // 🔥 AUTOR
        //                && (!value.StartAuthorOf.HasValue || t01.OwnerID >= value.StartAuthorOf)
        //                && (!value.EndAuthorOf.HasValue || t01.OwnerID <= value.EndAuthorOf)

        //                // 🔥 AUTORIZADOR
        //                && (!value.StartAuthorizerOf.HasValue || t02.UserID >= value.StartAuthorizerOf)
        //                && (!value.EndAuthorizerOf.HasValue || t02.UserID <= value.EndAuthorizerOf)

        //                // 🔥 FECHAS
        //                && (!startDate.HasValue || t01.CreateDate >= startDate)
        //                && (!endDate.HasValue || t01.CreateDate <= endDate)

        //                // 🔥 CARD CODE
        //                && (string.IsNullOrEmpty(value.StartCardCode) || t03.CardCode.CompareTo(value.StartCardCode) >= 0)
        //                && (string.IsNullOrEmpty(value.EndCardCode) || t03.CardCode.CompareTo(value.EndCardCode) <= 0)

        //                // 🔥 OBJTYPE
        //                && (!objTypeList.Any() || objTypeList.Contains(t03.ObjType))

        //            // 🔥 AGRUPACIÓN AGREGADA
        //            group new { t01, t03, t04 } by new
        //            {
        //                t03.DocEntry,
        //                t03.DocNum,
        //                t03.ObjType,
        //                t03.DocDate,
        //                t01.CreateDate,
        //                t01.CreateTime,
        //                t01.IsDraft,
        //                t03.CardCode,
        //                t03.CardName,
        //                t04.U_NAME,
        //                t03.WddStatus,
        //                t01.Remarks
        //            } into g

        //            select new ApprovalStatusReportQueryEntity
        //            {
        //                Line = 1,
        //                DocEntry = g.Key.DocEntry,
        //                DocNumOrder = null,
        //                DocNumPreliminary = g.Key.DocNum,
        //                ObjType = g.Key.ObjType,
        //                DocDate = g.Key.DocDate,
        //                CreateDate = g.Key.CreateDate,
        //                CreateTime = g.Key.CreateTime,
        //                CreateTimeString = (g.Key.CreateTime.ToString("D4").Substring(0, 2) + ":" + g.Key.CreateTime.ToString("D4").Substring(2, 2)),
        //                IsDraft = g.Key.IsDraft,
        //                CardCode = g.Key.CardCode,
        //                CardName = g.Key.CardName,
        //                AuthorName = g.Key.U_NAME,
        //                DocumentStatus = g.Key.WddStatus,
        //                Remarks = g.Key.Remarks,

        //                Lines = (
        //                    from t01A in _db.ApprovalRequests
        //                    join t02A in _db.Orders on t01A.DocEntry equals t02A.DocEntry
        //                    join t03A in _db.Users on t01A.OwnerID equals t03A.USERID
        //                    join t04A in _db.ApprovalTemplates on t01A.WtmCode equals t04A.WtmCode

        //                    where t02A.DocEntry == g.Key.DocEntry
        //                       && t02A.ObjType == g.Key.ObjType

        //                    select new ApprovalStatusReportLines1QueryEntity
        //                    {
        //                        WddCode = t01A.WddCode,
        //                        DocEntry = t02A.DocEntry,
        //                        DocNumOrder = null,
        //                        DocNumPreliminary = t02A.DocNum,
        //                        ObjType = t02A.ObjType,
        //                        CreateDate = t01A.CreateDate,
        //                        CreateTime = t01A.CreateTime,
        //                        CreateTimeString = t01A.CreateTime.ToString("D4").Substring(0, 2) + ":" + t01A.CreateTime.ToString("D4").Substring(2, 2),
        //                        AuthorName = t03A.U_NAME,
        //                        ModelName = t04A.Name,
        //                        ApproverStatus = t01A.Status,

        //                        Lines = (
        //                            from t01B in _db.ApprovalRequests
        //                            join t02B in _db.ApprovalRequestsLines on t01B.WddCode equals t02B.WddCode
        //                            join t03B in _db.Users on t02B.UserID equals t03B.USERID
        //                            join t04B in _db.ApprovalStages on t01B.CurrStep equals t04B.WstCode

        //                            where t01B.WddCode == t01A.WddCode

        //                            select new ApprovalStatusReportLines2QueryEntity
        //                            {
        //                                WddCode = t01B.WddCode,
        //                                StapaName = t04B.Name,
        //                                AuthorizerName = t03B.U_NAME,
        //                                Status = t02B.Status,
        //                                UpdateDate = t02B.UpdateDate,
        //                                UpdateTime = t02B.UpdateTime,
        //                                UpdateTimeString = t02B.UpdateTime != null ? t02B.UpdateTime.Value.ToString("D4").Substring(0, 2) + ":" + t02B.UpdateTime.Value.ToString("D4").Substring(2, 2) : "",
        //                                Remarks = t02B.Remarks
        //                            }).ToList()
        //                    }).ToList()
        //            })
        //            .ToListAsync();



        //        var list2 = await (
        //            from t01 in _db.ApprovalRequests
        //            join t02 in _db.ApprovalRequestsLines on t01.WddCode equals t02.WddCode
        //            join t03 in _db.Drafts on t01.DocEntry equals t03.DocEntry
        //            join t04 in _db.Users on t01.OwnerID equals t04.USERID

        //            where
        //                (t01.IsDraft == "Y")
        //                // 🔥 STATUS (opcional)
        //                && (!statusDraftList.Any() || statusDraftList.Contains(t03.WddStatus))

        //                // 🔥 AUTOR
        //                && (!value.StartAuthorOf.HasValue || t01.OwnerID >= value.StartAuthorOf)
        //                && (!value.EndAuthorOf.HasValue || t01.OwnerID <= value.EndAuthorOf)

        //                // 🔥 AUTORIZADOR
        //                && (!value.StartAuthorizerOf.HasValue || t02.UserID >= value.StartAuthorizerOf)
        //                && (!value.EndAuthorizerOf.HasValue || t02.UserID <= value.EndAuthorizerOf)

        //                // 🔥 FECHAS
        //                && (!startDate.HasValue || t01.CreateDate >= startDate)
        //                && (!endDate.HasValue || t01.CreateDate <= endDate)

        //                // 🔥 CARD CODE
        //                && (string.IsNullOrEmpty(value.StartCardCode) || t03.CardCode.CompareTo(value.StartCardCode) >= 0)
        //                && (string.IsNullOrEmpty(value.EndCardCode) || t03.CardCode.CompareTo(value.EndCardCode) <= 0)

        //                // 🔥 OBJTYPE
        //                && (!objTypeList.Any() || objTypeList.Contains(t03.ObjType))

        //            // 🔥 AGRUPACIÓN AGREGADA
        //            group new { t01, t03, t04 } by new
        //            {
        //                t03.DocEntry,
        //                t03.DocNum,
        //                t03.ObjType,
        //                t03.DocDate,
        //                t01.CreateDate,
        //                t01.CreateTime,
        //                t01.IsDraft,
        //                t03.CardCode,
        //                t03.CardName,
        //                t04.U_NAME,
        //                t03.WddStatus,
        //                t01.Remarks
        //            } into g

        //            select new ApprovalStatusReportQueryEntity
        //            {
        //                Line = 2,
        //                DocEntry = g.Key.DocEntry,
        //                DocNumOrder = null,
        //                DocNumPreliminary = g.Key.DocNum,
        //                ObjType = g.Key.ObjType,
        //                DocDate = g.Key.DocDate,
        //                CreateDate = g.Key.CreateDate,
        //                CreateTime = g.Key.CreateTime,
        //                CreateTimeString =  (g.Key.CreateTime.ToString("D4").Substring(0, 2) + ":" + g.Key.CreateTime.ToString("D4").Substring(2, 2)),
        //                IsDraft = g.Key.IsDraft,
        //                CardCode = g.Key.CardCode,
        //                CardName = g.Key.CardName,
        //                AuthorName = g.Key.U_NAME,
        //                DocumentStatus = g.Key.WddStatus,
        //                Remarks = g.Key.Remarks,

        //                Lines = (
        //                    from t01A in _db.ApprovalRequests
        //                    join t02A in _db.Drafts on t01A.DocEntry equals t02A.DocEntry
        //                    join t03A in _db.Users on t01A.OwnerID equals t03A.USERID
        //                    join t04A in _db.ApprovalTemplates on t01A.WtmCode equals t04A.WtmCode

        //                    where t02A.DocEntry == g.Key.DocEntry
        //                       && t02A.ObjType == g.Key.ObjType

        //                    select new ApprovalStatusReportLines1QueryEntity
        //                    {
        //                        WddCode = t01A.WddCode,
        //                        DocEntry = t02A.DocEntry,
        //                        DocNumOrder = null,
        //                        DocNumPreliminary = t02A.DocNum,
        //                        ObjType = t02A.ObjType,
        //                        CreateDate = t01A.CreateDate,
        //                        CreateTime = t01A.CreateTime,
        //                         CreateTimeString = t01A.CreateTime.ToString("D4").Substring(0, 2) + ":" + t01A.CreateTime.ToString("D4").Substring(2, 2),
        //                        AuthorName = t03A.U_NAME,
        //                        ModelName = t04A.Name,
        //                        ApproverStatus = t01A.Status,

        //                        Lines = (
        //                            from t01B in _db.ApprovalRequests
        //                            join t02B in _db.ApprovalRequestsLines on t01B.WddCode equals t02B.WddCode
        //                            join t03B in _db.Users on t02B.UserID equals t03B.USERID
        //                            join t04B in _db.ApprovalStages on t01B.CurrStep equals t04B.WstCode

        //                            where t01B.WddCode == t01A.WddCode

        //                            select new ApprovalStatusReportLines2QueryEntity
        //                            {
        //                                WddCode = t01B.WddCode,
        //                                StapaName = t04B.Name,
        //                                AuthorizerName = t03B.U_NAME,
        //                                Status = t02B.Status,
        //                                UpdateDate = t02B.UpdateDate,
        //                                UpdateTime = t02B.UpdateTime,
        //                                UpdateTimeString = t02B.UpdateTime != null ? t02B.UpdateTime.Value.ToString("D4").Substring(0, 2) + ":" + t02B.UpdateTime.Value.ToString("D4").Substring(2, 2) : "",
        //                                Remarks = t02B.Remarks
        //                            }).ToList()
        //                    }).ToList()
        //            })
        //            .ToListAsync();




        //        // 🔥 ORDEN + ROW_NUMBER (Order)
        //        list2 = list2
        //            .OrderBy(x => x.CreateDate)
        //            .ThenBy(x => x.CreateTime)
        //            .ThenBy(x => x.DocEntry)
        //            .Select((x, index) =>
        //            {
        //                x.Order = index + 1;
        //                return x;
        //            })
        //            .ToList();


        //        resultTransaccion.IdRegistro = 0;
        //        resultTransaccion.ResultadoCodigo = 0;
        //        resultTransaccion.ResultadoDescripcion = $"Registros Totales {list2.Count}";
        //        resultTransaccion.dataList = list2;
        //    }
        //    catch (Exception ex)
        //    {
        //        resultTransaccion.IdRegistro = -1;
        //        resultTransaccion.ResultadoCodigo = -1;
        //        resultTransaccion.ResultadoDescripcion = ex.Message;
        //    }

        //    return resultTransaccion;
        //}


        //public async Task<ResultadoTransaccionEntity<ApprovalStatusReportQueryEntity>> GetApprovalStatusReport(ApprovalStatusReportFilterEntity value)
        //{
        //    var resultTransaccion = new ResultadoTransaccionEntity<ApprovalStatusReportQueryEntity>();

        //    try
        //    {
        //        var statusDraftList = new List<string>();
        //        var statusOrderList = new List<string>();

        //        if (value.Pending) statusDraftList.Add("W");
        //        if (value.Authorized) statusDraftList.Add("Y");
        //        if (value.Rejected) statusDraftList.Add("N");
        //        if (value.Canceled) statusDraftList.Add("C");

        //        if (value.CreatedBy) statusOrderList.Add("P");
        //        if (value.CreatedByResponsibleAuthorization) statusOrderList.Add("A");

        //        var objTypeList = new List<string>();
        //        if (value.Quotations) objTypeList.Add("23");
        //        if (value.Orders) objTypeList.Add("17");

        //        var startDate = value.StartDate?.Date;
        //        var endDate = value.EndDate?.Date;

        //        var list1 = new List<ApprovalStatusReportQueryEntity>();
        //        var list2 = new List<ApprovalStatusReportQueryEntity>();

        //        // =========================
        //        // 🔥 LIST1 (ORDERS)
        //        // =========================
        //        if (statusOrderList.Any() && objTypeList.Any())
        //        {
        //            list1 = await (
        //                from t01 in _db.ApprovalRequests
        //                join t03 in _db.Orders on t01.DocEntry equals t03.DocEntry
        //                join t04 in _db.Users on t01.OwnerID equals t04.USERID

        //                where
        //                    t01.IsDraft == "N"
        //                    && statusOrderList.Contains(t03.WddStatus)
        //                    && objTypeList.Contains(t03.ObjType)

        //                    // 🔥 REEMPLAZO DEL JOIN POR EXISTS
        //                    && _db.ApprovalRequestsLines.Any(t02 =>
        //                        t02.WddCode == t01.WddCode
        //                        && (!value.StartAuthorizerOf.HasValue || t02.UserID >= value.StartAuthorizerOf)
        //                        && (!value.EndAuthorizerOf.HasValue || t02.UserID <= value.EndAuthorizerOf)
        //                    )

        //                    && (!value.StartAuthorOf.HasValue || t01.OwnerID >= value.StartAuthorOf)
        //                    && (!value.EndAuthorOf.HasValue || t01.OwnerID <= value.EndAuthorOf)

        //                    && (!startDate.HasValue || t01.CreateDate >= startDate)
        //                    && (!endDate.HasValue || t01.CreateDate <= endDate)

        //                    && (string.IsNullOrEmpty(value.StartCardCode) || t03.CardCode.CompareTo(value.StartCardCode) >= 0)
        //                    && (string.IsNullOrEmpty(value.EndCardCode) || t03.CardCode.CompareTo(value.EndCardCode) <= 0)

        //                select new
        //                {
        //                    t01,
        //                    t03,
        //                    t04
        //                }

        //            )
        //            .GroupBy(x => new
        //            {
        //                x.t03.DocEntry,
        //                x.t03.DocNum,
        //                x.t03.ObjType,
        //                x.t03.DocDate,
        //                x.t01.CreateDate,
        //                x.t01.CreateTime,
        //                x.t03.CardCode,
        //                x.t03.CardName,
        //                x.t04.U_NAME,
        //                x.t03.WddStatus,
        //                x.t01.Remarks
        //            })
        //            .Select(g => new ApprovalStatusReportQueryEntity
        //            {
        //                Line = 1,
        //                DocEntry = g.Key.DocEntry,
        //                DocNumPreliminary = g.Key.DocNum,
        //                ObjType = g.Key.ObjType,
        //                DocDate = g.Key.DocDate,
        //                CreateDate = g.Key.CreateDate,
        //                CreateTime = g.Key.CreateTime,
        //                CreateTimeString = g.Key.CreateTime.ToString("D4").Substring(0, 2) + ":" + g.Key.CreateTime.ToString("D4").Substring(2, 2),
        //                CardCode = g.Key.CardCode,
        //                CardName = g.Key.CardName,
        //                AuthorName = g.Key.U_NAME,
        //                DocumentStatus = g.Key.WddStatus,
        //                Remarks = g.Key.Remarks,

        //                // 🔥 SUBCONSULTAS INTACTAS
        //                Lines = (
        //                    from t01A in _db.ApprovalRequests
        //                    join t02A in _db.Orders on t01A.DocEntry equals t02A.DocEntry
        //                    join t03A in _db.Users on t01A.OwnerID equals t03A.USERID
        //                    join t04A in _db.ApprovalTemplates on t01A.WtmCode equals t04A.WtmCode
        //                    where t02A.DocEntry == g.Key.DocEntry && t02A.ObjType == g.Key.ObjType
        //                    select new ApprovalStatusReportLines1QueryEntity
        //                    {
        //                        WddCode = t01A.WddCode,
        //                        DocEntry = t02A.DocEntry,
        //                        DocNumPreliminary = t02A.DocNum,
        //                        ObjType = t02A.ObjType,
        //                        CreateDate = t01A.CreateDate,
        //                        CreateTime = t01A.CreateTime,
        //                        CreateTimeString = t01A.CreateTime.ToString("D4").Substring(0, 2) + ":" + t01A.CreateTime.ToString("D4").Substring(2, 2),
        //                        AuthorName = t03A.U_NAME,
        //                        ModelName = t04A.Name,
        //                        ApproverStatus = t01A.Status,

        //                        Lines = (
        //                            from t01B in _db.ApprovalRequests
        //                            join t02B in _db.ApprovalRequestsLines on t01B.WddCode equals t02B.WddCode
        //                            join t03B in _db.Users on t02B.UserID equals t03B.USERID
        //                            join t04B in _db.ApprovalStages on t01B.CurrStep equals t04B.WstCode
        //                            where t01B.WddCode == t01A.WddCode
        //                            select new ApprovalStatusReportLines2QueryEntity
        //                            {
        //                                WddCode = t01B.WddCode,
        //                                StapaName = t04B.Name,
        //                                AuthorizerName = t03B.U_NAME,
        //                                Status = t02B.Status,
        //                                UpdateDate = t02B.UpdateDate,
        //                                UpdateTime = t02B.UpdateTime,
        //                                UpdateTimeString = t02B.UpdateTime != null ? t02B.UpdateTime.Value.ToString("D4").Substring(0, 2) + ":" + t02B.UpdateTime.Value.ToString("D4").Substring(2, 2) : "",
        //                                Remarks = t02B.Remarks
        //                            }).ToList()
        //                    }).ToList()
        //            })
        //            .ToListAsync();
        //        }

        //        // =========================
        //        // 🔥 LIST2 (DRAFTS)
        //        // =========================
        //        if (statusDraftList.Any() && objTypeList.Any())
        //        {
        //            list2 = await (
        //                from t01 in _db.ApprovalRequests
        //                join t03 in _db.Drafts on t01.DocEntry equals t03.DocEntry
        //                join t04 in _db.Users on t01.OwnerID equals t04.USERID

        //                where
        //                    t01.IsDraft == "Y"
        //                    && statusDraftList.Contains(t03.WddStatus)
        //                    && objTypeList.Contains(t03.ObjType)

        //                    // 🔥 EXISTS
        //                    && _db.ApprovalRequestsLines.Any(t02 =>
        //                        t02.WddCode == t01.WddCode
        //                        && (!value.StartAuthorizerOf.HasValue || t02.UserID >= value.StartAuthorizerOf)
        //                        && (!value.EndAuthorizerOf.HasValue || t02.UserID <= value.EndAuthorizerOf)
        //                    )

        //                select new
        //                {
        //                    t01,
        //                    t03,
        //                    t04
        //                }

        //            )
        //            .GroupBy(x => new
        //            {
        //                x.t03.DocEntry,
        //                x.t03.DocNum,
        //                x.t03.ObjType,
        //                x.t03.DocDate,
        //                x.t01.CreateDate,
        //                x.t01.CreateTime,
        //                x.t03.CardCode,
        //                x.t03.CardName,
        //                x.t04.U_NAME,
        //                x.t03.WddStatus,
        //                x.t01.Remarks
        //            })
        //            .Select(g => new ApprovalStatusReportQueryEntity
        //            {
        //                Line = 2,
        //                DocEntry = g.Key.DocEntry,
        //                DocNumPreliminary = g.Key.DocNum,
        //                ObjType = g.Key.ObjType,
        //                DocDate = g.Key.DocDate,
        //                CreateDate = g.Key.CreateDate,
        //                CreateTime = g.Key.CreateTime,
        //                CardCode = g.Key.CardCode,
        //                CardName = g.Key.CardName,
        //                AuthorName = g.Key.U_NAME,
        //                DocumentStatus = g.Key.WddStatus,
        //                Remarks = g.Key.Remarks,

        //                Lines = (
        //                    from t01A in _db.ApprovalRequests
        //                    join t02A in _db.Drafts on t01A.DocEntry equals t02A.DocEntry
        //                    join t03A in _db.Users on t01A.OwnerID equals t03A.USERID
        //                    join t04A in _db.ApprovalTemplates on t01A.WtmCode equals t04A.WtmCode
        //                    where t02A.DocEntry == g.Key.DocEntry && t02A.ObjType == g.Key.ObjType
        //                    select new ApprovalStatusReportLines1QueryEntity
        //                    {
        //                        WddCode = t01A.WddCode,
        //                        DocEntry = t02A.DocEntry,
        //                        DocNumPreliminary = t02A.DocNum,
        //                        ObjType = t02A.ObjType,

        //                        Lines = (
        //                            from t01B in _db.ApprovalRequests
        //                            join t02B in _db.ApprovalRequestsLines on t01B.WddCode equals t02B.WddCode
        //                            join t03B in _db.Users on t02B.UserID equals t03B.USERID
        //                            join t04B in _db.ApprovalStages on t01B.CurrStep equals t04B.WstCode
        //                            where t01B.WddCode == t01A.WddCode
        //                            select new ApprovalStatusReportLines2QueryEntity
        //                            {
        //                                WddCode = t01B.WddCode,
        //                                StapaName = t04B.Name
        //                            }).ToList()
        //                    }).ToList()
        //            })
        //            .ToListAsync();
        //        }

        //        var finalList = list1
        //            .Concat(list2)
        //            .OrderBy(x => x.Line)
        //            .ThenBy(x => x.CreateDate)
        //            .ThenBy(x => x.CreateTime)
        //            .ThenBy(x => x.DocEntry)
        //            .Select((x, i) =>
        //            {
        //                x.Order = i + 1;
        //                return x;
        //            })
        //            .ToList();

        //        resultTransaccion.dataList = finalList;
        //    }
        //    catch (Exception ex)
        //    {
        //        resultTransaccion.ResultadoCodigo = -1;
        //        resultTransaccion.ResultadoDescripcion = ex.Message;
        //    }

        //    return resultTransaccion;
        //}

        public async Task<ResultadoTransaccionEntity<ApprovalStatusReportQueryEntity>> GetApprovalStatusReport(ApprovalStatusReportFilterEntity value)
        {
            var response = new List<ApprovalStatusReportJsonQueryEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<ApprovalStatusReportQueryEntity>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_APPROVAL_STATUS, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@StatusOrder", value.StatusOrder));
                        cmd.Parameters.Add(new SqlParameter("@StatusDraf", value.StatusDraf));
                        cmd.Parameters.Add(new SqlParameter("@ObjType", value.ObjType));
                        cmd.Parameters.Add(new SqlParameter("@StartAuthorOf", value.StartAuthorOf));
                        cmd.Parameters.Add(new SqlParameter("@EndAuthorOf", value.EndAuthorOf));
                        cmd.Parameters.Add(new SqlParameter("@StartAuthorizerOf", value.StartAuthorizerOf));
                        cmd.Parameters.Add(new SqlParameter("@EndAuthorizerOf", value.EndAuthorizerOf));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", value.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", value.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@StartCardCode", value.StartCardCode));
                        cmd.Parameters.Add(new SqlParameter("@EndCardCode", value.EndCardCode));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<ApprovalStatusReportJsonQueryEntity>)context.ConvertTo<ApprovalStatusReportJsonQueryEntity>(reader);
                        }

                        var json = response.FirstOrDefault()?.JsonResult;

                        var data = string.IsNullOrEmpty(json)
                            ? new List<ApprovalStatusReportQueryEntity>()
                            : JsonConvert.DeserializeObject<List<ApprovalStatusReportQueryEntity>>(json);


                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        resultTransaccion.dataList = data;
                    }
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
    }
}
