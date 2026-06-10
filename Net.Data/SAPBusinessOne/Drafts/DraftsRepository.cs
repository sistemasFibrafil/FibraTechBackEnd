using System;
using System.IO;
using SAPbobsCOM;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Drafts.Query;
using Net.Business.Entities.SAPBusinessOne.Drafts.Filter;
using Net.Business.Entities.SAPBusinessOne.Drafts.Create;
using Net.Business.Entities.SAPBusinessOne.Drafts.Update;
using Net.Business.Entities.SAPBusinessOne.Drafts.Entities;
using Net.Business.Entities.SAPBusinessOne.Drafts.CreateToDocument;
using Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Query;
namespace Net.Data.SAPBusinessOne
{
    public class DraftsRepository : RepositoryBase<DraftsEntity>, IDraftsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;
        private static readonly string[] collection = ["O", "C"];

        public DraftsRepository(IConnectionSQL context, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }


        #region <<< CONSULTAS >>>

        public async Task<ResultadoTransaccionResponse<DraftsQueryEntity>> GetListDraftsDocumentReport(DraftsDocumentReportFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DraftsQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var objTypeList = new List<string>();
                var docStatusList = new List<string>();

                var query = _db.Drafts
                .AsNoTracking();


                switch (value.DraftDate)
                {
                    case "01": // CreateDate
                        if (value.StartDate != null)
                            query = query.Where(x => x.CreateDate >= value.StartDate);

                        if (value.EndDate != null)
                            query = query.Where(x => x.CreateDate <= value.EndDate);
                        break;

                    case "02": // UpdateDate
                        if (value.StartDate != null)
                            query = query.Where(x => x.UpdateDate >= value.StartDate);

                        if (value.EndDate != null)
                            query = query.Where(x => x.UpdateDate <= value.EndDate);
                        break;

                    case "03": // DocDate
                        if (value.StartDate != null)
                            query = query.Where(x => x.DocDate >= value.StartDate);

                        if (value.EndDate != null)
                            query = query.Where(x => x.DocDate <= value.EndDate);
                        break;

                    case "04": // TaxDate
                        if (value.StartDate != null)
                            query = query.Where(x => x.TaxDate >= value.StartDate);

                        if (value.EndDate != null)
                            query = query.Where(x => x.TaxDate <= value.EndDate);
                        break;
                }


                if (!string.IsNullOrWhiteSpace(value.User))
                {
                    var userIds = value.User
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => short.TryParse(x.Trim(), out var id) ? (short?)id : null)
                        .Where(x => x.HasValue)
                        .Select(x => x.Value)
                        .ToArray();

                    if (userIds.Length > 0)
                        query = query.Where(x => userIds.Contains(x.UserSign));
                }


                docStatusList.AddRange(value.Pending ? ["O"] : collection);

                query = query.Where(x => docStatusList.Contains(x.DocStatus));


                if(value.Orders) objTypeList.Add("17");

                if (objTypeList.Count > 0)
                    query = query.Where(x => objTypeList.Contains(x.ObjType));


                var list = await query
                .Select(n => new DraftsQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    DocType = n.DocType,
                    DocStatus = n.DocStatus,
                    CreateDate = n.CreateDate,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    UpdateDate = n.UpdateDate,

                    CardCode = n.CardCode,
                    CardName = n.CardName,
                    GroupCode = n.BusinessPartners.GroupCode,
                    GroupName = n.BusinessPartners.BusinessPartnerGroups.GroupName,
                    DocCur = n.DocCur,

                    SlpName = n.SalesPersons != null ? n.SalesPersons.SlpName : "",

                    DocTotal = n.DocTotal,
                    DocTotalSy = n.DocTotalSy
                })
                .OrderBy(x => x.DocEntry).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
                resultTransaccion.DataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<DraftsQueryEntity>> GetByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DraftsQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                string[] warehouse = await _db.Warehouses
                .Where(n => n.U_FIB_ALMPRO == "Y")
                .Select(n => n.WhsCode) // <-- seleccionas solo el campo string
                .AsNoTracking()
                .ToArrayAsync();


                var adminInfo = await _db.AdminInfo
                .Select(n => new
                {
                    MaMainCurncy = n.MainCurncy
                })
                .FirstOrDefaultAsync();

                var data = await _db.Drafts
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new DraftsQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    ObjType = n.ObjType,
                    DocType = n.DocType,
                    DocStatus = n.DocStatus,
                    WddStatus = n.WddStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    U_FIB_DocStPkg = n.U_FIB_DocStPkg,
                    U_FIB_IsPkg = n.U_FIB_IsPkg,
                    CardCode = n.CardCode,
                    CardName = n.CardName,
                    CntctCode = n.CntctCode,
                    NumAtCard = n.NumAtCard,
                    DocCur = n.DocCur,
                    CurrencyList = _db.CurrencyCodes
                                   .Where(c => c.CurrCode == n.DocCur)
                                   .Select(c => new CurrencyCodesEntity
                                   {
                                       CurrCode = c.CurrCode,
                                       CurrName = c.CurrName
                                   })
                                   .ToList(),
                    DocRate = n.DocRate,

                    GroupNum = n.GroupNum,

                    PayToCode = n.PayToCode,
                    // ✅ DIRECCIONES DE PAGO (CRD1 AdresType = 'B')
                    PayAddressList = _db.Addresses
                                     .Where(a => a.CardCode == n.CardCode && a.AdresType == "B")
                                     .OrderBy(a => a.LineNum)
                                     .Select(a => new AddressesEntity
                                     {
                                         CardCode = a.CardCode,
                                         AdresType = a.AdresType,
                                         Address = a.Address,
                                         Street = a.Street,
                                         LineNum = a.LineNum
                                     })
                                     .ToList(),
                    Address = Utilidades.QuitarSaltosLinea(n.Address),
                    ShipToCode = n.ShipToCode,
                    // ✅ DIRECCIONES DE DESPACHO (CRD1 AdresType = 'S')
                    ShipAddressList = _db.Addresses
                                      .Where(a => a.CardCode == n.CardCode && a.AdresType == "S")
                                      .OrderBy(a => a.LineNum)
                                      .Select(a => new AddressesEntity
                                      {
                                          CardCode = a.CardCode,
                                          AdresType = a.AdresType,
                                          Address = a.Address,
                                          Street = a.Street,
                                          LineNum = a.LineNum
                                      })
                                      .ToList(),
                    Address2 = Utilidades.QuitarSaltosLinea(n.Address2),

                    U_BPP_MDCT = n.U_BPP_MDCT,
                    U_BPP_MDRT = n.U_BPP_MDRT,
                    U_BPP_MDNT = n.U_BPP_MDNT,
                    U_FIB_CODT = n.U_FIB_CODT,
                    // ✅ DIRECCIONES DE DESPACHO (CRD1 AdresType = 'S')
                    AgencyAddressList = _db.Addresses
                                        .Where(a => a.CardCode == n.U_BPP_MDCT && a.AdresType == "S")
                                        .OrderBy(a => a.LineNum)
                                        .Select(a => new AddressesEntity
                                        {
                                            CardCode = a.CardCode,
                                            AdresType = a.AdresType,
                                            Address = a.Address,
                                            Street = a.Street,
                                            LineNum = a.LineNum
                                        })
                                        .ToList(),
                    U_BPP_MDDT = Utilidades.QuitarSaltosLinea(n.U_BPP_MDDT),

                    U_TipoFlete = n.U_TipoFlete,
                    U_ValorFlete = n.U_ValorFlete,
                    U_FIB_TFLETE = n.U_FIB_TFLETE,
                    U_FIB_IMPSEG = n.U_FIB_IMPSEG,
                    U_FIB_PUERTO = n.U_FIB_PUERTO,

                    U_STR_TVENTA = n.U_STR_TVENTA,

                    SlpCode = n.SlpCode,
                    U_NroOrden = n.U_NroOrden,
                    U_OrdenCompra = n.U_OrdenCompra,
                    Comments = n.Comments,

                    SubTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal - n.VatSum + n.DiscSum : n.DocTotalSy - n.VatSumSy + n.DiscSumSy,
                    DiscPrcnt = n.DiscPrcnt ?? 0,
                    DiscSum = adminInfo.MaMainCurncy == n.DocCur ? n.DiscSum : n.DiscSumSy,
                    VatSum = adminInfo.MaMainCurncy == n.DocCur ? n.VatSum : n.VatSumSy,
                    DocTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal : n.DocTotalSy,


                    // 🔹 ANEXOS
                    Attachments2 = n.Attachments2 == null ? null : new Attachments2QueryEntity
                    {
                        AbsEntry = n.Attachments2.AbsEntry,
                        Lines = n.Attachments2.Lines.Select(a => new Attachments2LinesQueryEntity
                        {
                            AbsEntry = a.AbsEntry,
                            Line = a.Line,
                            SrcPath = a.srcPath,
                            TrgtPath = a.trgtPath,
                            FileName = a.FileName,
                            FileExt = a.FileExt,
                            Date = a.Date
                        }).ToList()
                    },

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines.Select(s => new DraftsLinesQueryEntity
                    {
                        DocEntry = s.DocEntry,
                        LineNum = s.LineNum,
                        LineStatus = s.LineStatus,
                        ObjType = s.ObjType,
                        ItemCode = s.ItemCode,
                        Dscription = s.Dscription,
                        AcctCode = s.AcctCode,
                        FormatCode = s.ChartOfAccounts != null ? s.ChartOfAccounts.Segment_0 + "-" + s.ChartOfAccounts.Segment_1 + "-" + s.ChartOfAccounts.Segment_2 : "",
                        AcctName = s.ChartOfAccounts != null ? s.ChartOfAccounts.AcctName : "",
                        WhsCode = s.WhsCode,

                        UnitMsr = s.UnitMsr,
                        Quantity = s.Quantity,
                        OpenQty = s.OpenQty,
                        OnHand = _db.ItemWarehouseInfo
                                 .Where(w => w.ItemCode == s.Item.ItemCode && warehouse.Contains(w.WhsCode))
                                 .Sum(w => (decimal?)w.OnHand) ?? 0,

                        Currency = s.Currency,
                        PriceBefDi = s.PriceBefDi,
                        DiscPrcnt = s.DiscPrcnt ?? 0,
                        Price = s.Price,

                        TaxCode = s.TaxCode,
                        VatPrcnt = s.VatPrcnt ?? 0,
                        VatSum = adminInfo.MaMainCurncy == s.Currency ? s.VatSum : s.VatSumSy,
                        LineTotal = adminInfo.MaMainCurncy == s.Currency ? s.LineTotal : s.TotalSumSy,

                        U_FIB_LinStPkg = s.U_FIB_LinStPkg ?? s.LineStatus,
                        U_FIB_OpQtyPkg = s.U_FIB_OpQtyPkg ?? s.OpenQty,
                        U_S_PartAranc1 = s.U_S_PartAranc1 ?? "",
                        U_tipoOpT12 = s.U_tipoOpT12 ?? "",
                        U_tipoOpT12Nam = s.OperationType != null ? s.U_tipoOpT12 + " - " + s.OperationType.U_descrp : "",
                    }).ToList()
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.Data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<DraftsStatusQueryEntity>> GetStatusByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DraftsStatusQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.Drafts
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new DraftsStatusQueryEntity
                {
                    DocStatus = n.DocStatus,
                    WddStatus = n.WddStatus
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.Data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        #endregion



        #region <<< OPERACIONES >>>

        public async Task<ResultadoTransaccionResponse<DraftsEntity>> SetCreate(DraftsCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DraftsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Documents drafts = null;
            Attachments2 attachments = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();



                    // Se crea el objeto de documento borrador
                    drafts = company.GetBusinessObject(BoObjectTypes.oDrafts);



                    #region <<< CABECERA >>>

                    drafts.DocDate = value.DocDate;
                    drafts.DocDueDate = value.DocDueDate;
                    drafts.TaxDate = value.TaxDate;

                    drafts.DocObjectCode = BoObjectTypes.oOrders;

                    drafts.DocType = value.DocType switch
                    {
                        "I" => BoDocumentTypes.dDocument_Items,
                        "S" => BoDocumentTypes.dDocument_Service,
                        _ => throw new ArgumentException($"DocType inválido para SAP Business One: '{value.DocType}'. Se esperaba 'I' (Artículo) o 'S' (Servicio)."),
                    };

                    drafts.UserFields.Fields.Item("U_FIB_DocStPkg").Value = value.U_FIB_DocStPkg;
                    drafts.UserFields.Fields.Item("U_FIB_IsPkg").Value = value.U_FIB_IsPkg;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    drafts.CardCode = value.CardCode;
                    drafts.CardName = value.CardName;
                    drafts.ContactPersonCode = value.CntctCode;
                    drafts.NumAtCard = value.NumAtCard;
                    drafts.DocCurrency = value.DocCur;
                    drafts.DocRate = value.DocRate;

                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    drafts.PayToCode = value.PayToCode;
                    drafts.Address = value.Address;
                    drafts.ShipToCode = value.ShipToCode;
                    drafts.Address2 = value.Address2;

                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================
                    drafts.PaymentGroupCode = value.GroupNum;

                    // ===========================================================================================
                    // AGENCIA
                    // ===========================================================================================
                    drafts.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                    drafts.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    drafts.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    drafts.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                    drafts.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    drafts.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                    drafts.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                    drafts.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                    drafts.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                    drafts.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;
                    drafts.UserFields.Fields.Item("U_FIB_NEMBA").Value = value.U_FIB_NEMBA;
                    drafts.UserFields.Fields.Item("U_FIB_DEMBA").Value = value.U_FIB_DEMBA;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    drafts.SalesPersonCode = value.SlpCode;
                    drafts.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                    drafts.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    drafts.Comments = value.Comments;

                    // ===========================================================================================
                    // TOTALES
                    // ===========================================================================================
                    drafts.DiscountPercent = value.DiscPrcnt;
                    drafts.DocTotal = value.DocTotal;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    drafts.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                    #endregion


                    #region <<< DETALLE >>>

                    bool isItem = value.DocType == "I";
                    bool isService = value.DocType == "S";

                    foreach (var line in value.Lines)
                    {
                        if (isItem)
                        {
                            drafts.Lines.ItemCode = line.ItemCode;
                            drafts.Lines.WarehouseCode = line.WhsCode;
                            drafts.Lines.MeasureUnit = line.UnitMsr;
                            drafts.Lines.Quantity = line.Quantity;
                        }

                        if (isService)
                        {
                            drafts.Lines.AccountCode = line.AcctCode;
                        }

                        drafts.Lines.ItemDescription = line.Dscription;

                        drafts.Lines.Currency = line.Currency;
                        drafts.Lines.UnitPrice = line.PriceBefDi;
                        drafts.Lines.DiscountPercent = line.DiscPrcnt;
                        drafts.Lines.Price = line.Price;

                        drafts.Lines.TaxCode = line.TaxCode;
                        drafts.Lines.LineTotal = line.LineTotal;

                        // UDFs
                        drafts.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                        drafts.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                        drafts.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        drafts.Lines.UserFields.Fields.Item("U_S_PartAranc1").Value = line.U_S_PartAranc1;
                        drafts.Lines.Add();
                    }

                    #endregion


                    #region <<< ATTACHMENTS >>>

                    if (value.Attachments2?.Lines?.Count > 0)
                    {
                        attachments = company.GetBusinessObject(BoObjectTypes.oAttachments2);

                        foreach (var item in value.Attachments2.Lines)
                        {
                            attachments.Lines.Add();
                            attachments.Lines.SourcePath = item.SrcPath;
                            attachments.Lines.FileName = item.FileName;
                            attachments.Lines.FileExtension = item.FileExt;
                            attachments.Lines.Override = BoYesNoEnum.tYES;
                        }

                        if (attachments.Add() != 0)
                        {
                            company.GetLastError(out int errorCode, out string errorMessage);
                            throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                        }

                        // 🔥 IMPORTANTE: SIEMPRE antes del Add (SAP 9.2)
                        drafts.AttachmentEntry = int.Parse(company.GetNewObjectKey());
                    }

                    #endregion


                    if (drafts.Add() != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }



                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "El documento borrador registrado con éxito.";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(drafts, attachments);
                }

                return resultTransaccion;
            });
        }

        public Task<ResultadoTransaccionResponse<DraftsEntity>> SetSaveDraftToDocument(DraftsCreateToDocumentEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DraftsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType!.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Documents drafts = null;
            Attachments2 attachments = null;

            try
            {
                // 🔹 Conexión a SAP
                var company = _companyProviderSap.GetCompany();



                // 🔹 Se crea el objeto de orden de venta
                drafts = company.GetBusinessObject(BoObjectTypes.oDrafts);

                // 🔹 Validar existencia de la orden de venta
                if (!drafts.GetByKey(value.DocEntry))
                {
                    throw new Exception("No existe el documento borrador.");
                }


                #region <<< CABECERA >>>

                drafts.DocDate = value.DocDate;
                drafts.DocDueDate = value.DocDueDate;
                drafts.TaxDate = value.TaxDate;

                // ===========================================================================================
                // SOCIO DE NEGOCIO
                // ===========================================================================================
                drafts.CardCode = value.CardCode;
                drafts.ContactPersonCode = value.CntctCode;
                drafts.NumAtCard = value.NumAtCard;
                drafts.DocCurrency = value.DocCur;
                drafts.DocRate = value.DocRate;

                // ===========================================================================================
                // LOGÍSTICA
                // ===========================================================================================
                drafts.PayToCode = value.PayToCode;
                drafts.Address = value.Address;
                drafts.ShipToCode = value.ShipToCode;
                drafts.Address2 = value.Address2;


                // ===========================================================================================
                // FINANZAS
                // ===========================================================================================
                drafts.PaymentGroupCode = value.GroupNum;

                // ===========================================================================================
                // AGENCIA
                // ===========================================================================================
                drafts.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                drafts.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                drafts.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                drafts.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                drafts.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                // ===========================================================================================
                // EXPORTACIÓN
                // ===========================================================================================
                drafts.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                drafts.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                drafts.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                drafts.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                drafts.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;
                drafts.UserFields.Fields.Item("U_FIB_NEMBA").Value = value.U_FIB_NEMBA;
                drafts.UserFields.Fields.Item("U_FIB_DEMBA").Value = value.U_FIB_DEMBA;

                // ===========================================================================================
                // PIE
                // ===========================================================================================
                drafts.SalesPersonCode = value.SlpCode;
                drafts.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                drafts.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                drafts.Comments = value.Comments;

                // ===========================================================================================
                // TOTALES
                // ===========================================================================================
                drafts.DiscountPercent = value.DiscPrcnt;
                drafts.DocTotal = value.DocTotal;

                // ===========================================================================================
                // AUDITORÍA
                // ===========================================================================================
                drafts.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                #endregion


                #region <<< DETALLE >>>

                bool isItem = value.DocType == "I";
                bool isService = value.DocType == "S";

                foreach (var line in value.Lines)
                {
                    drafts.Lines.Add();

                    if (isItem)
                    {
                        drafts.Lines.ItemCode = line.ItemCode;
                        drafts.Lines.WarehouseCode = line.WhsCode;
                        drafts.Lines.Quantity = line.Quantity;
                    }

                    if (isService)
                    {
                        drafts.Lines.AccountCode = line.AcctCode;
                    }

                    drafts.Lines.ItemDescription = line.Dscription;
                    drafts.Lines.Currency = line.Currency;
                    drafts.Lines.UnitPrice = line.PriceBefDi;
                    drafts.Lines.DiscountPercent = line.DiscPrcnt;
                    drafts.Lines.Price = line.Price;

                    drafts.Lines.TaxCode = line.TaxCode;
                    drafts.Lines.LineTotal = line.LineTotal;

                    drafts.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                    drafts.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                    drafts.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                    drafts.Lines.UserFields.Fields.Item("U_S_PartAranc1").Value = line.U_S_PartAranc1;
                }

                #endregion


                #region <<< ATTACHMENTS >>>

                // SIEMPRE SE CREA UN NUEVO ANEXO
                if (value.Attachments2?.Lines?.Count > 0)
                {
                    attachments = company.GetBusinessObject(BoObjectTypes.oAttachments2);

                    foreach (var item in value.Attachments2.Lines)
                    {
                        attachments.Lines.Add();
                        attachments.Lines.SourcePath = item.SrcPath;
                        attachments.Lines.FileName = item.FileName;
                        attachments.Lines.FileExtension = item.FileExt;
                        attachments.Lines.Override = BoYesNoEnum.tYES;
                    }

                    if (attachments.Add() != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }

                    // 🔥 IMPORTANTE: SIEMPRE antes del Add (SAP 9.2)
                    drafts.AttachmentEntry = int.Parse(company.GetNewObjectKey());
                }

                #endregion


                if (drafts.SaveDraftToDocument() != 0)
                {
                    company.GetLastError(out int errorCode, out string errorMessage);
                    throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                }



                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "La orden de venta registrada con éxito.";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(drafts, attachments);
            }

            return Task.FromResult(resultTransaccion);
        }

        public async Task<ResultadoTransaccionResponse<DraftsEntity>> SetUpdate(DraftsUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DraftsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Documents drafts = null;
            Attachments2 attachments = null;
            
            return await Task.Run(() =>
            {
                try
                {
                    // 🔹 Conexión a SAP
                    var company = _companyProviderSap.GetCompany();



                    // 🔹 Se crea el objeto de orden de venta
                    drafts = company.GetBusinessObject(BoObjectTypes.oDrafts);

                    // 🔹 Validar existencia de la orden de venta
                    if (!drafts.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe el documento borrador.");
                    }


                    #region <<< CABECERA >>>

                    drafts.DocDate = value.DocDate;
                    drafts.DocDueDate = value.DocDueDate;
                    drafts.TaxDate = value.TaxDate;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    drafts.CardCode = value.CardCode;
                    drafts.ContactPersonCode = value.CntctCode;
                    drafts.NumAtCard = value.NumAtCard;
                    drafts.DocCurrency = value.DocCur;
                    drafts.DocRate = value.DocRate;

                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    drafts.PayToCode = value.PayToCode;
                    drafts.Address = value.Address;
                    drafts.ShipToCode = value.ShipToCode;
                    drafts.Address2 = value.Address2;


                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================
                    drafts.PaymentGroupCode = value.GroupNum;

                    // ===========================================================================================
                    // AGENCIA
                    // ===========================================================================================
                    drafts.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                    drafts.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    drafts.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    drafts.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                    drafts.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    drafts.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                    drafts.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                    drafts.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                    drafts.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                    drafts.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;
                    drafts.UserFields.Fields.Item("U_FIB_NEMBA").Value = value.U_FIB_NEMBA;
                    drafts.UserFields.Fields.Item("U_FIB_DEMBA").Value = value.U_FIB_DEMBA;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    drafts.SalesPersonCode = value.SlpCode;
                    drafts.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                    drafts.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    drafts.Comments = value.Comments;

                    // ===========================================================================================
                    // TOTALES
                    // ===========================================================================================
                    drafts.DiscountPercent = value.DiscPrcnt;
                    drafts.DocTotal = value.DocTotal;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    drafts.UserFields.Fields.Item("U_UsrUpdate").Value = value.U_UsrUpdate;

                    #endregion


                    #region <<< DETALLE >>>

                    bool isItem = value.DocType == "I";
                    bool isService = value.DocType == "S";

                    // NUEVO: SE AGREGA NUEVO ITEM
                    foreach (var line in value.Lines.Where(x => x.Record == 1))
                    {
                        drafts.Lines.Add();

                        if (isItem)
                        {
                            drafts.Lines.ItemCode = line.ItemCode;
                            drafts.Lines.WarehouseCode = line.WhsCode;
                            drafts.Lines.Quantity = line.Quantity;
                        }

                        if (isService)
                        {
                            drafts.Lines.AccountCode = line.AcctCode;
                        }

                        drafts.Lines.ItemDescription = line.Dscription;
                        drafts.Lines.Currency = line.Currency;
                        drafts.Lines.UnitPrice = line.PriceBefDi;
                        drafts.Lines.DiscountPercent = line.DiscPrcnt;
                        drafts.Lines.Price = line.Price;

                        drafts.Lines.TaxCode = line.TaxCode;
                        drafts.Lines.LineTotal = line.LineTotal;

                        drafts.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                        drafts.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                        drafts.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        drafts.Lines.UserFields.Fields.Item("U_S_PartAranc1").Value = line.U_S_PartAranc1;
                    }

                    // EXISTE: SE MODIFICA EL ITEM
                    foreach (var line in value.Lines.Where(x => x.Record == 2))
                    {
                        for (int i = 0; i < drafts.Lines.Count; i++)
                        {
                            drafts.Lines.SetCurrentLine(i);

                            if (drafts.Lines.LineNum != line.LineNum)
                                continue;

                            if (isItem)
                            {
                                drafts.Lines.ItemCode = line.ItemCode;
                                drafts.Lines.WarehouseCode = line.WhsCode;
                                drafts.Lines.Quantity = line.Quantity;
                            }

                            if (isService)
                            {
                                drafts.Lines.AccountCode = line.AcctCode;
                            }

                            drafts.Lines.ItemDescription = line.Dscription;
                            drafts.Lines.Currency = line.Currency;
                            drafts.Lines.UnitPrice = line.PriceBefDi;
                            drafts.Lines.DiscountPercent = line.DiscPrcnt;
                            drafts.Lines.Price = line.Price;

                            drafts.Lines.TaxCode = line.TaxCode;
                            drafts.Lines.LineTotal = line.LineTotal;

                            drafts.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                            drafts.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                            drafts.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                            drafts.Lines.UserFields.Fields.Item("U_S_PartAranc1").Value = line.U_S_PartAranc1;
                        }
                    }

                    // EXISTE: SE ELIMINA EL ITEM
                    foreach (var line in value.Lines.Where(x => x.Record == 3))
                    {
                        for (int i = 0; i < drafts.Lines.Count; i++)
                        {
                            drafts.Lines.SetCurrentLine(i);
                            if (drafts.Lines.LineNum == line.LineNum)
                            {
                                drafts.Lines.Delete();
                                break;
                            }
                        }
                    }

                    #endregion


                    #region <<< ATTACHMENTS >>>

                    // SIEMPRE SE CREA UN NUEVO ANEXO
                    if (value.Attachments2?.Lines?.Count > 0)
                    {
                        attachments = company.GetBusinessObject(BoObjectTypes.oAttachments2);

                        foreach (var item in value.Attachments2.Lines)
                        {
                            attachments.Lines.Add();
                            attachments.Lines.SourcePath = item.SrcPath;
                            attachments.Lines.FileName = item.FileName;
                            attachments.Lines.FileExtension = item.FileExt;
                            attachments.Lines.Override = BoYesNoEnum.tYES;
                        }

                        if (attachments.Add() != 0)
                        {
                            company.GetLastError(out int errorCode, out string errorMessage);
                            throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                        }

                        // 🔥 IMPORTANTE: SIEMPRE antes del Add (SAP 9.2)
                        drafts.AttachmentEntry = int.Parse(company.GetNewObjectKey());
                    }

                    #endregion


                    if (drafts.Update() != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "El documento borrador actualizado con éxito.";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(drafts, attachments);
                }

                return resultTransaccion;
            });
        }

        #endregion



        #region <<< IMPRESIONES >>>

        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetPrintNationalDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var adminInfo = await _db.AdminInfo
                .Select(a => new AdminInfoQueryEntity
                {
                    MaMainCurncy = a.MainCurncy,

                    PrintHeadr = a.PrintHeadr
                })
                .FirstOrDefaultAsync();


                var data = await _db.Drafts
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new DraftsQueryEntity
                {
                    DocNum = n.DocNum,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    DocTime = n.DocTime.ToString().Insert(n.DocTime.ToString().Length - 2, ":").PadLeft(5, '0'),

                    // SOCIO DE NEGOCIOS
                    LicTradNum = n.BusinessPartners.LicTradNum ?? "",
                    CardName = n.CardName ?? "",
                    DocCur = n.DocCur ?? "",
                    // FINANZAS
                    PymntGroup = n.PaymentTermsTypes.PymntGroup,

                    // LOGÍSTICA
                    Address2 = n.Address2 ?? "",

                    // AGENCIA
                    U_BPP_MDNT = n.U_BPP_MDNT ?? "",
                    U_BPP_MDDT = n.U_BPP_MDDT ?? "",

                    // SALES EMPLOYEE
                    SlpName = n.SalesPersons.SlpName,
                    U_OrdenCompra = n.U_OrdenCompra ?? "",
                    Comments = n.Comments ?? "",

                    //// TOTALES
                    SubTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal - n.VatSum + n.DiscSum : n.DocTotalSy - n.VatSumSy + n.DiscSumSy,
                    DiscPrcnt = n.DiscPrcnt ?? 0,
                    DiscSum = adminInfo.MaMainCurncy == n.DocCur ? n.DiscSum : n.DiscSumSy,
                    VatSum = adminInfo.MaMainCurncy == n.DocCur ? n.VatSum : n.VatSumSy,
                    DocTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal : n.DocTotalSy,

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines
                    .OrderBy(s => s.LineNum)
                    .Select(s => new DraftsLinesQueryEntity
                    {
                        LineNum = s.LineNum,
                        ItemCode = s.ItemCode,
                        Dscription = s.Dscription ?? "",
                        UnitMsr = s.UnitMsr ?? "",
                        Quantity = s.Quantity,
                        Price = s.Price,
                        LineTotal = adminInfo.MaMainCurncy == s.Currency ? s.LineTotal : s.TotalSumSy,
                    }).ToList()
                })
                .FirstOrDefaultAsync();


                var header = new HeaderDraftsNational()
                {
                    PrintHeadr = adminInfo.PrintHeadr,

                    DocNum = data.DocNum.ToString(),
                    DocDate = data.DocDate.ToString("dd/MM/yyyy"),
                    DocDueDate = data.DocDueDate == null ? "" : Convert.ToDateTime(data.DocDueDate).ToString("dd/MM/yyyy"),
                    DocTime = data.DocTime,

                    CardName = data.CardName,
                    LicTradNum = data.LicTradNum,

                    Address2 = data.Address2,

                    U_OrdenCompra = data.U_OrdenCompra,
                };


                var footer = new FooterDraftsNational()
                {
                    Texto = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Resources", "txt", "natTexto.txt")),
                };


                iTextSharp.text.Document doc = new iTextSharp.text.Document();
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                doc.SetMargins(10f, 10f, 165f, 200f);
                MemoryStream ms = new MemoryStream();
                iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                // Our custom Header and Footer is done using Event Handler
                var pageEventHelper = new PageEventHelperDraftsNational();
                write.PageEvent = pageEventHelper;

                // Colocamos la fuente que deseamos que tenga el documento
                iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
                iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                iTextSharp.text.Font parrafoNegrita5 = new iTextSharp.text.Font(helvetica, 5.5f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
                iTextSharp.text.Font parrafoNegrita7 = new iTextSharp.text.Font(helvetica, 7f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

                // Define the page header
                pageEventHelper.Header = header;
                pageEventHelper.Footer = footer;

                doc.Open();


                //============================
                //TABLA: DETALLE
                var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 3f, 20f, 46f, 7f, 6f, 6f, 12f }) { WidthPercentage = 100 };
                var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                for (int i = 0; i < data.Lines.Count; i++)
                {
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase((i + 1).ToString(), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].ItemCode, parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Dscription, parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].UnitMsr.ToUpper(), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Quantity.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Price.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"{data.DocCur} {data.Lines[i].LineTotal:N2}", parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                }

                doc.Add(tbl);


                tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 16f, 1f, 65f, 6f, 12f }) { WidthPercentage = 100 };
                // Fila 1
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Empleado del departamento de ventas", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.SlpName, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("SubTotal", parrafoNegrita5)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"{data.DocCur} {data.SubTotal:N2}", parrafoNegrita5)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 2
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Condiciones de pago", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.PymntGroup, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Impuesto", parrafoNegrita5)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"{data.DocCur} {data.VatSum:N2}", parrafoNegrita5)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 3
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Datos de Transporte", parrafoNegrita7)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Total", parrafoNegrita5)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"{data.DocCur} {data.DocTotal:N2}", parrafoNegrita5)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 4
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.U_BPP_MDNT, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 5
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.U_BPP_MDDT, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 6
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Comments, parrafoNormal)) { BorderWidth = 0, PaddingTop = 10, PaddingBottom = 3, Colspan = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 10, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 10, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                doc.Add(tbl);


                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Se generó correctamente el archivo.s";
                resultTransaccion.Data = file;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetPrintExportPlantaDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var adminInfo = await _db.AdminInfo
                .Select(a => new AdminInfoQueryEntity
                {

                    PrintHeadr = a.PrintHeadr,
                })
                .FirstOrDefaultAsync();


                var data = await _db.Drafts
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new DraftsQueryEntity
                {
                    DocNum = n.DocNum,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    DocTime = n.DocTime.ToString().Insert(n.DocTime.ToString().Length - 2, ":").PadLeft(5, '0'),

                    // SOCIO DE NEGOCIOS
                    LicTradNum = n.BusinessPartners.LicTradNum ?? "",
                    CardName = n.CardName ?? "",

                    // LOGÍSTICA
                    Address = n.Address ?? "",
                    Address2 = n.Address2 ?? "",

                    // AGENCIA
                    U_BPP_MDNT = n.U_BPP_MDNT ?? "",
                    U_BPP_MDDT = n.U_BPP_MDDT ?? "",

                    U_OrdenCompra = n.U_OrdenCompra ?? "",
                    Comments = n.Comments ?? "",

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines
                    .OrderBy(s => s.LineNum)
                    .Select(s => new DraftsLinesQueryEntity
                    {
                        LineNum = s.LineNum,
                        ItemCode = s.ItemCode,
                        Dscription = s.Dscription ?? "",
                        UnitMsr = s.UnitMsr ?? "",
                        Quantity = s.Quantity,
                        OpenQty = s.OpenQty,
                        Delivered = s.Quantity - s.OpenQty
                    }).ToList()
                })
                .FirstOrDefaultAsync();


                var header = new HeaderDraftsExportPlanta()
                {
                    PrintHeadr = adminInfo.PrintHeadr,

                    DocNum = data.DocNum.ToString(),
                    DocDate = data.DocDate.ToString("dd/MM/yyyy"),
                    DocDueDate = data.DocDueDate == null ? null : Convert.ToDateTime(data.DocDueDate).ToString("dd/MM/yyyy"),
                    DocTime = data.DocTime,

                    CardName = data.CardName,
                    LicTradNum = data.LicTradNum,

                    Address = data.Address,
                    Address2 = data.Address2,

                    U_OrdenCompra = data.U_OrdenCompra,
                };


                var footer = new FooterDraftsExportPlanta()
                {
                    U_BPP_MDNT = data.U_BPP_MDNT,
                    U_BPP_MDDT = data.U_BPP_MDDT,
                    Comments = data.Comments
                };


                iTextSharp.text.Document doc = new iTextSharp.text.Document();
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                doc.SetMargins(10f, 10f, 165f, 200f);
                MemoryStream ms = new MemoryStream();
                iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                // Our custom Header and Footer is done using Event Handler
                var pageEventHelper = new PageEventHelperDraftsExportPlanta();
                write.PageEvent = pageEventHelper;

                // Colocamos la fuente que deseamos que tenga el documento
                iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
                iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);

                // Define the page header
                pageEventHelper.Header = header;
                pageEventHelper.Footer = footer;

                doc.Open();


                //============================
                //TABLA: DETALLE
                var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 3f, 22f, 47f, 7f, 6f, 7f, 8f }) { WidthPercentage = 100 };
                var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                for (int i = 0; i < data.Lines.Count; i++)
                {
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase((i + 1).ToString(), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].ItemCode, parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Dscription, parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].UnitMsr.ToUpper(), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Convert.ToDecimal(data.Lines[i].Quantity).ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Convert.ToDecimal(data.Lines[i].OpenQty).ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Delivered.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                }

                doc.Add(tbl);

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Se generó correctamente el archivo.s";
                resultTransaccion.Data = file;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetPrintExportClienteDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var adminInfo = await _db.AdminInfo
                .Select(a => new AdminInfoQueryEntity
                {
                    MaMainCurncy = a.MainCurncy,

                    PrintHeadr = a.PrintHeadr,
                    Phone1 = a.Phone1,
                    Fax = a.Fax,

                    Street = a.AdminInfo1.Street,
                    County = a.AdminInfo1.County,
                    City = a.AdminInfo1.City,

                    CountryName = a.AdminInfo1.Countries.Name
                })
                .FirstOrDefaultAsync();


                var data = await _db.Drafts
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new DraftsQueryEntity
                {
                    DocNum = n.DocNum,
                    TaxDate = n.TaxDate,

                    // SOCIO DE NEGOCIOS
                    CardCode = n.CardCode,
                    LicTradNum = n.BusinessPartners.LicTradNum ?? "",
                    CardName = n.CardName ?? "",
                    DocCur = n.DocCur ?? "",
                    CurrName = n.CurrencyCodes.CurrName,

                    // FINANZAS
                    PymntGroup = n.PaymentTermsTypes.PymntGroup,

                    // LOGÍSTICA
                    Address = n.Address ?? "",
                    Address2 = n.Address2 ?? "",

                    // EXPORTACION
                    TipoFleteDescr = _db.UserDefinedFields1
                                    .Where(u =>
                                        u.TableID == "ORDR" &&
                                        u.FieldID == 84 &&
                                        u.FldValue == n.U_TipoFlete)
                                    .Select(u => u.Descr)
                                    .FirstOrDefault(),
                    U_FIB_PUERTO = n.U_FIB_PUERTO, // Condicion de embarque
                    U_FIB_NEMBA = n.U_FIB_NEMBA ?? "",
                    U_FIB_DEMBA = n.U_FIB_DEMBA ?? "",
                    U_STR_FEMB = n.U_STR_FEMB ?? "",
                    U_FIB_TFLETE = n.U_FIB_TFLETE ?? 0,
                    U_FIB_IMPSEG = n.U_FIB_IMPSEG ?? 0,

                    // SALES EMPLOYEE
                    SlpName = n.SalesPersons.SlpName,
                    U_OrdenCompra = n.U_OrdenCompra ?? "",
                    Comments = n.Comments ?? "",

                    // TOTALES
                    SubTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal - n.VatSum + n.DiscSum : n.DocTotalSy - n.VatSumSy + n.DiscSumSy,
                    DocTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal : n.DocTotalSy,

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines
                    .OrderBy(s => s.LineNum)
                    .Select(s => new DraftsLinesQueryEntity
                    {
                        LineNum = s.LineNum,
                        ItemCode = s.ItemCode,
                        Dscription = s.Dscription ?? "",
                        UnitMsr = s.UnitMsr ?? "",
                        Quantity = s.Quantity,
                        Price = s.Price,
                        LineTotal = adminInfo.MaMainCurncy == s.Currency ? s.LineTotal : s.TotalSumSy,
                    }).ToList()
                })
                .FirstOrDefaultAsync();


                var header = new HeaderDraftsExportCliente()
                {
                    PrintHeadr = adminInfo.PrintHeadr,
                    Phone1 = adminInfo.Phone1,
                    Fax = adminInfo.Fax,

                    Street = adminInfo.Street,
                    County = adminInfo.County,
                    City = adminInfo.City,

                    CountryName = adminInfo.CountryName,

                    DocNum = data.DocNum.ToString(),
                    TaxDate = data.TaxDate.ToString("dd/MM/yyyy"),

                    PymntGroup = data.PymntGroup,

                    CardCode = data.CardCode,
                    CardName = data.CardName,
                    LicTradNum = data.LicTradNum,
                    DocCurr = data.DocCur,
                    CurrName = data.CurrName,

                    Address = data.Address,
                    Address2 = data.Address2,

                    TipoFleteDescr = data.TipoFleteDescr,
                    U_FIB_PUERTO = data.U_FIB_PUERTO,
                    U_FIB_NEMBA = data.U_FIB_NEMBA,
                    U_FIB_DEMBA = data.U_FIB_DEMBA,
                    U_STR_FEMB = data.U_STR_FEMB,

                    SlpName = data.SlpName[..Math.Min(35, data.SlpName.Length)],
                    U_OrdenCompra = data.U_OrdenCompra,
                };

                var footer = new FooterDraftsExportCliente()
                {
                    PrintHeadr = adminInfo.PrintHeadr,
                    CardName = data.CardName,
                    BancoContinental = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Resources", "txt", "expBancoContinental.txt")),
                    BancoPichincha = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Resources", "txt", "expBancoPichincha.txt")),
                    BancoSantander = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Resources", "txt", "expBancoSantander.txt")),
                    BancoScotiabank = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Resources", "txt", "expBancoScotiabank.txt")),
                    BancoInterbank = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Resources", "txt", "expBancoInterbank.txt")),
                    Texto = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Resources", "txt", "expTexto.txt")),
                };


                iTextSharp.text.Document doc = new iTextSharp.text.Document();
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                doc.SetMargins(10f, 10f, 291f, 240f);
                MemoryStream ms = new MemoryStream();
                iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                // Our custom Header and Footer is done using Event Handler
                var pageEventHelper = new PageEventHelperDraftsExportCliente();
                write.PageEvent = pageEventHelper;

                // Colocamos la fuente que deseamos que tenga el documento
                iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
                iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);

                // Define the page header
                pageEventHelper.Header = header;
                pageEventHelper.Footer = footer;

                doc.Open();


                //============================
                //TABLA: DETALLE
                var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 3f, 22f, 47f, 7f, 6f, 7f, 8f }) { WidthPercentage = 100 };
                var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                for (int i = 0; i < data.Lines.Count; i++)
                {
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase((i + 1).ToString(), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].ItemCode, parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Dscription, parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].UnitMsr.ToUpper(), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Convert.ToDecimal(data.Lines[i].Quantity).ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Convert.ToDecimal(data.Lines[i].Price).ToString("N3"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].LineTotal.ToString("N3"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                }

                doc.Add(tbl);


                tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 79f, 13f, 8f }) { WidthPercentage = 100 };
                // Fila 1
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(HelperAmountToLetters.AmountToLetters(data.DocTotal, data.CurrName), parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Importe FOB", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data?.SubTotal.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 2
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Comments, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Rowspan = 4 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Importe Flete", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.U_FIB_TFLETE?.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 3
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Importe Seguro", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.U_FIB_IMPSEG?.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 4
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Total", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data?.DocTotal.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 5
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                doc.Add(tbl);

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Se generó correctamente el archivo.s";
                resultTransaccion.Data = file;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        #endregion
    }


    #region <<< NATIONAL >>>

    public class HeaderDraftsNational
    {
        public string PrintHeadr { get; set; }
        public string DocNum { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string DocTime { get; set; }


        /// <summary>
        /// SOCIO DE NEGOCIOS
        /// </summary>
        public string CardName { get; set; }
        public string LicTradNum { get; set; }


        /// <summary>
        /// LOGÍSTICA
        /// </summary>
        public string Address2 { get; set; }


        /// <summary>
        /// SALES EMPLOYEE
        /// </summary>
        public string U_OrdenCompra { get; set; }
    }

    public class FooterDraftsNational
    {
        public string Texto { get; set; }
    }

    public class PageEventHelperDraftsNational : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate headerTemplate, footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;

        #region Properties
        public HeaderDraftsNational Header { get; set; }
        public FooterDraftsNational Footer { get; set; }
        #endregion

        // we override the onOpenDocument method
        public override void OnOpenDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            try
            {
                bfTitulo = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA_BOLD, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                bfTexto = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(100, 100);
            }
            catch (iTextSharp.text.DocumentException)
            {
            }
            catch (IOException)
            {
            }
        }
        public override void OnStartPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnStartPage(writer, doc);
            iTextSharp.text.Rectangle pageSize = doc.PageSize;

            iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoNegrita = new iTextSharp.text.Font(helvetica, 7f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

            //Logo
            var pathLogo = Path.Combine(Environment.CurrentDirectory, "logos", "fibrafil-logo.jpg");
            var logo = iTextSharp.text.Image.GetInstance(pathLogo);
            logo.ScaleToFit(100f, 50f);
            logo.SetAbsolutePosition(pageSize.GetLeft(12), pageSize.GetTop(65));
            cb.AddImage(logo);


            #region <<< DATOS DE LA ORDEN >>>

            // Nombre de la empresa
            cb.BeginText();
            cb.SetFontAndSize(bfTitulo, 12f);
            cb.SetTextMatrix(pageSize.GetLeft(250), pageSize.GetTop(40));
            cb.ShowText(Header.PrintHeadr);
            cb.EndText();

            // página
            int pageN = writer.PageNumber;
            string text = "" + pageN + " / ";
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 6.5f);
            cb.SetTextMatrix(pageSize.GetRight(95), pageSize.GetTop(130));
            cb.ShowText(text);
            cb.EndText();

            float len = bfTexto.GetWidthPoint(text, 6.5f);
            cb.AddTemplate(headerTemplate, pageSize.GetRight(95) + len, pageSize.GetTop(130));

            #endregion


            /*
             ================================================
             TABLA 1: HEADER - DATOS DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DATOS DE LA ORDEN >>>

            var tblOrder = new iTextSharp.text.pdf.PdfPTable(new float[] { 45f, 55f });
            tblOrder.TotalWidth = 160;

            // Fila 1
            var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ORDEN DE VENTA", parrafoTitulo)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5, Colspan = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblOrder.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Número de pedido", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocNum, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Fecha", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocDate, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 4
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Fecha de entrega", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocDueDate, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 5
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Hora", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocTime, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 6
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("R.U.C.", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.LicTradNum, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 7
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Orden de compra", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.U_OrdenCompra, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 7
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Página", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            // Ubicación de la tabla de la cabecera hacía la derecha en la página
            tblOrder.WriteSelectedRows(0, -1, pageSize.GetLeft(425), pageSize.GetTop(10), cb);

            #endregion



            /*
             ================================================
             TABLA 1: HEADER - DATOS DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DATOS DE CLIENTE >>>

            var tblCliente = new iTextSharp.text.pdf.PdfPTable(new float[] { 6f, 2f, 92f });
            tblCliente.TotalWidth = 400;

            // Fila 1
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("PARA", parrafoNegrita)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.CardName, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("DE", parrafoNegrita)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.Address2, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);

            // Ubicación de la tabla de la cabecera hacía la derecha en la página
            tblCliente.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetTop(80), cb);

            #endregion



            /*
             ================================================
             TABLA 4: HEADER - DETALLE DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DETALLE DE LA ORDEN >>>

            float startX = pageSize.GetLeft(10);
            float startY = pageSize.GetTop(150);

            var tblDetail = new iTextSharp.text.pdf.PdfPTable(new float[] { 3f, 20f, 46f, 7f, 6f, 6f, 12f });
            tblDetail.TotalWidth = 575;
            tblDetail.LockedWidth = true;

            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("#", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Código", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Descripcion", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("UM", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Cantidad", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Precio", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Total", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);

            tblDetail.WriteSelectedRows(0, -1, startX, startY, cb);

            #endregion
        }
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnEndPage(writer, doc);
            iTextSharp.text.Rectangle pageSize = doc.PageSize;
            iTextSharp.text.Font parrafoTexto = new iTextSharp.text.Font(bfTitulo, 5.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(bfTitulo, 7f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoTitulo = new iTextSharp.text.Font(bfTitulo, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.White);
            iTextSharp.text.Font parrafoSubTitulo = new iTextSharp.text.Font(bfTitulo, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);


            /*
                =====================================================
                Codigo para que el número de página muestre en el pie
                =====================================================
            */
            //int pageN = writer.PageNumber;
            //string text = "Página " + pageN + "/";
            //float len = bfTexto.GetWidthPoint(text, 8);
            //iTextSharp.text.Rectangle pageSize = document.PageSize;
            //cb.SetRgbColorFill(100, 100, 100);
            //cb.BeginText();
            //cb.SetFontAndSize(bfTexto, 8);
            //cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetBottom(30));
            //cb.ShowText(text);
            //cb.EndText();
            //cb.AddTemplate(footerTemplate, pageSize.GetLeft(15) + len, pageSize.GetBottom(30));


            /*
            ================================================
                TABLA 1: FOOTER - BANCOS
            ================================================
            */
            var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 100f })
            {
                TotalWidth = 575,
                LockedWidth = true
            };

            // Fila 1
            var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.Texto, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);

            // ======================================================
            // DIBUJAR TABLA
            // ======================================================
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(170), cb);
        }
        public override void OnCloseDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnCloseDocument(writer, doc);
            /*
                ==========================================================
                Codigo para que el número de página muestre en la cabecera
                ==========================================================
            */
            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bfTexto, 6.5f);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            headerTemplate.EndText();

            /*
               =====================================================
               Codigo para que el número de página muestre en el pie
               =====================================================
           */
            //footerTemplate.BeginText();
            //footerTemplate.SetFontAndSize(bfTexto, 8);
            //footerTemplate.SetTextMatrix(0, 0);
            //footerTemplate.ShowText("" + (writer.PageNumber - 1));
            //footerTemplate.EndText();
        }
    }

    #endregion


    #region <<< EXPORTACION - PLANTA >>>

    public class HeaderDraftsExportPlanta
    {
        public string PrintHeadr { get; set; }
        public string DocNum { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string DocTime { get; set; }


        /// <summary>
        /// SOCIO DE NEGOCIOS
        /// </summary>
        public string CardName { get; set; }
        public string LicTradNum { get; set; }


        /// <summary>
        /// LOGÍSTICA
        /// </summary>
        public string Address { get; set; }
        public string Address2 { get; set; }


        /// <summary>
        /// SALES EMPLOYEE
        /// </summary>
        public string U_OrdenCompra { get; set; }
    }

    public class FooterDraftsExportPlanta
    {
        public string U_BPP_MDNT { get; set; }
        public string U_BPP_MDDT { get; set; }
        public string Comments { get; set; }
    }

    public class PageEventHelperDraftsExportPlanta : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate headerTemplate, footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;

        #region Properties
        public HeaderDraftsExportPlanta Header { get; set; }
        public FooterDraftsExportPlanta Footer { get; set; }
        #endregion

        // we override the onOpenDocument method
        public override void OnOpenDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            try
            {
                bfTitulo = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA_BOLD, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                bfTexto = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(100, 100);
            }
            catch (iTextSharp.text.DocumentException)
            {
            }
            catch (IOException)
            {
            }
        }
        public override void OnStartPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnStartPage(writer, doc);
            iTextSharp.text.Rectangle pageSize = doc.PageSize;

            iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoNegrita = new iTextSharp.text.Font(helvetica, 7f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

            //Logo
            var pathLogo = Path.Combine(Environment.CurrentDirectory, "logos", "fibrafil-logo.jpg");
            var logo = iTextSharp.text.Image.GetInstance(pathLogo);
            logo.ScaleToFit(100f, 50f);
            logo.SetAbsolutePosition(pageSize.GetLeft(12), pageSize.GetTop(65));
            cb.AddImage(logo);


            #region <<< DATOS DE LA ORDEN >>>

            // Nombre de la empresa
            cb.BeginText();
            cb.SetFontAndSize(bfTitulo, 12f);
            cb.SetTextMatrix(pageSize.GetLeft(250), pageSize.GetTop(40));
            cb.ShowText(Header.PrintHeadr);
            cb.EndText();

            // página
            int pageN = writer.PageNumber;
            string text = "" + pageN + " / ";
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 6.5f);
            cb.SetTextMatrix(pageSize.GetRight(95), pageSize.GetTop(130));
            cb.ShowText(text);
            cb.EndText();

            float len = bfTexto.GetWidthPoint(text, 6.5f);
            cb.AddTemplate(headerTemplate, pageSize.GetRight(95) + len, pageSize.GetTop(130));

            #endregion


            /*
             ================================================
             TABLA 1: HEADER - DATOS DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DATOS DE LA ORDEN >>>

            var tblOrder = new iTextSharp.text.pdf.PdfPTable(new float[] { 45f, 55f });
            tblOrder.TotalWidth = 160;

            // Fila 1
            var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ORDEN DE VENTA", parrafoTitulo)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5, Colspan = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblOrder.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Número de pedido", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocNum, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Fecha", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocDate, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 4
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Fecha de entrega", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocDueDate, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 5
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Hora", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocTime, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 6
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("R.U.C.", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.LicTradNum, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 7
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Orden de compra", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.U_OrdenCompra, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 7
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Página", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            // Ubicación de la tabla de la cabecera hacía la derecha en la página
            tblOrder.WriteSelectedRows(0, -1, pageSize.GetLeft(425), pageSize.GetTop(10), cb);

            #endregion



            /*
             ================================================
             TABLA 1: HEADER - DATOS DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DATOS DE CLIENTE >>>

            var tblCliente = new iTextSharp.text.pdf.PdfPTable(new float[] { 7f, 93f });
            tblCliente.TotalWidth = 400;

            // Fila 1
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("PARA:", parrafoNegrita)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.CardName, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.Address2, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);

            // Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.Address, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);

            // Ubicación de la tabla de la cabecera hacía la derecha en la página
            tblCliente.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetTop(80), cb);

            #endregion



            /*
             ================================================
             TABLA 4: HEADER - DETALLE DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DETALLE DE LA ORDEN >>>

            float startX = pageSize.GetLeft(10);
            float startY = pageSize.GetTop(150);

            var tblDetail = new iTextSharp.text.pdf.PdfPTable(new float[] { 3f, 22f, 47f, 7f, 6f, 7f, 8f });
            tblDetail.TotalWidth = 575;
            tblDetail.LockedWidth = true;

            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("#", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Código", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Descripcion", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("UM", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Cantidad", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Pendiente", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Despachodo", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);

            tblDetail.WriteSelectedRows(0, -1, startX, startY, cb);

            #endregion
        }
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnEndPage(writer, doc);
            iTextSharp.text.Rectangle pageSize = doc.PageSize;
            iTextSharp.text.Font parrafoTexto = new iTextSharp.text.Font(bfTitulo, 5.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(bfTitulo, 7f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoTitulo = new iTextSharp.text.Font(bfTitulo, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.White);
            iTextSharp.text.Font parrafoSubTitulo = new iTextSharp.text.Font(bfTitulo, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);


            /*
                =====================================================
                Codigo para que el número de página muestre en el pie
                =====================================================
            */
            //int pageN = writer.PageNumber;
            //string text = "Página " + pageN + "/";
            //float len = bfTexto.GetWidthPoint(text, 8);
            //iTextSharp.text.Rectangle pageSize = document.PageSize;
            //cb.SetRgbColorFill(100, 100, 100);
            //cb.BeginText();
            //cb.SetFontAndSize(bfTexto, 8);
            //cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetBottom(30));
            //cb.ShowText(text);
            //cb.EndText();
            //cb.AddTemplate(footerTemplate, pageSize.GetLeft(15) + len, pageSize.GetBottom(30));


            /*
            ================================================
                TABLA 1: FOOTER - BANCOS
            ================================================
            */
            var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 10f, 1f, 89f })
            {
                TotalWidth = pageSize.Width - doc.LeftMargin - doc.RightMargin,
                LockedWidth = true
            };
            //tbl.TotalWidth = 575;

            // Fila 1
            var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Observaciones", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoTexto)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.Comments, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Empresa Transporte", parrafoTexto)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDNT, parrafoTexto)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);

            // Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Dirección", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoTexto)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDDT, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);

            // ======================================================
            // POSICIÓN RESPETANDO MÁRGENES
            // ======================================================
            float x = pageSize.Left + doc.LeftMargin;
            float y = pageSize.Bottom + doc.BottomMargin;

            // ======================================================
            // DIBUJAR TABLA
            // ======================================================
            tbl.WriteSelectedRows(0, -1, x, y, cb);

            // Ubicación de la tabla TEXTO
            //tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(120), cb);
        }
        public override void OnCloseDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnCloseDocument(writer, doc);
            /*
                ==========================================================
                Codigo para que el número de página muestre en la cabecera
                ==========================================================
            */
            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bfTexto, 6.5f);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            headerTemplate.EndText();

            /*
               =====================================================
               Codigo para que el número de página muestre en el pie
               =====================================================
           */
            //footerTemplate.BeginText();
            //footerTemplate.SetFontAndSize(bfTexto, 8);
            //footerTemplate.SetTextMatrix(0, 0);
            //footerTemplate.ShowText("" + (writer.PageNumber - 1));
            //footerTemplate.EndText();
        }
    }

    #endregion


    #region <<< EXPORTACION - CLIENTE >>>

    public class HeaderDraftsExportCliente
    {
        public string PrintHeadr { get; set; }
        public string Phone1 { get; set; }
        public string Fax { get; set; }
        public string Street { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public string CountryName { get; set; }

        public string DocNum { get; set; }
        public string TaxDate { get; set; }


        /// <summary>
        /// FINANZAS
        /// </summary>
        public string PymntGroup { get; set; }


        /// <summary>
        /// SOCIO DE NEGOCIOS
        /// </summary>
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string LicTradNum { get; set; }
        public string DocCurr { get; set; }
        public string CurrName { get; set; }


        /// <summary>
        /// LOGÍSTICA
        /// </summary>
        public string Address { get; set; }
        public string Address2 { get; set; }


        /// <summary>
        /// EXPORTACION
        /// </summary>
        public string TipoFleteDescr { get; set; }
        public string U_FIB_PUERTO { get; set; }
        public string? U_FIB_NEMBA { get; set; }
        public string? U_FIB_DEMBA { get; set; }
        public string U_STR_FEMB { get; set; }


        /// <summary>
        /// SALES EMPLOYEE
        /// </summary>

        public string SlpName { get; set; }
        public string U_OrdenCompra { get; set; }
    }
    public class FooterDraftsExportCliente
    {
        public string PrintHeadr { get; set; }
        public string CardName { get; set; }
        public string BancoContinental { get; set; }
        public string BancoPichincha { get; set; }
        public string BancoSantander { get; set; }
        public string BancoScotiabank { get; set; }
        public string BancoInterbank { get; set; }
        public string Texto { get; set; }

    }
    public class PageEventHelperDraftsExportCliente : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate headerTemplate, footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;

        #region Properties
        public HeaderDraftsExportCliente Header { get; set; }
        public FooterDraftsExportCliente Footer { get; set; }
        #endregion

        // we override the onOpenDocument method
        public override void OnOpenDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            try
            {
                bfTitulo = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA_BOLD, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                bfTexto = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(100, 100);
            }
            catch (iTextSharp.text.DocumentException)
            {
            }
            catch (IOException)
            {
            }
        }
        public override void OnStartPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnStartPage(writer, doc);
            iTextSharp.text.Rectangle pageSize = doc.PageSize;

            iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoNegrita = new iTextSharp.text.Font(helvetica, 7f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);


            //Logo
            var pathLogo = Path.Combine(Environment.CurrentDirectory, "logos", "fibrafil-logo.jpg");
            var logo = iTextSharp.text.Image.GetInstance(pathLogo);
            logo.ScaleToFit(100f, 50f);
            logo.SetAbsolutePosition(pageSize.GetLeft(12), pageSize.GetTop(65));
            cb.AddImage(logo);


            #region <<< DATOS DE LA ORDEN >>>

            // Nombre de la empresa
            cb.BeginText();
            cb.SetFontAndSize(bfTitulo, 12f);
            cb.SetTextMatrix(pageSize.GetLeft(150), pageSize.GetTop(22));
            cb.ShowText(Header.PrintHeadr);
            cb.EndText();

            // Dirección de la empresa
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 8f);
            cb.SetTextMatrix(pageSize.GetLeft(150), pageSize.GetTop(40));
            cb.ShowText(Header.Street);
            cb.EndText();
            // Distrito - Cuidad - País
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 8f);
            cb.SetTextMatrix(pageSize.GetLeft(150), pageSize.GetTop(50));
            cb.ShowText(Header.County + "-" + Header.City + "-" + Header.CountryName);
            cb.EndText();

            // Telefono
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 8f);
            cb.SetTextMatrix(pageSize.GetLeft(150), pageSize.GetTop(68));
            cb.ShowText("Telf");
            cb.EndText();
            // :
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 8f);
            cb.SetTextMatrix(pageSize.GetLeft(170), pageSize.GetTop(68));
            cb.ShowText(":");
            cb.EndText();
            // Telefono
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 8f);
            cb.SetTextMatrix(pageSize.GetLeft(180), pageSize.GetTop(68));
            cb.ShowText(Header.Phone1);
            cb.EndText();

            // Fax
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 8f);
            cb.SetTextMatrix(pageSize.GetLeft(150), pageSize.GetTop(86));
            cb.ShowText("Fax");
            cb.EndText();
            // :
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 8f);
            cb.SetTextMatrix(pageSize.GetLeft(170), pageSize.GetTop(86));
            cb.ShowText(":");
            cb.EndText();
            // Fax
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 8f);
            cb.SetTextMatrix(pageSize.GetLeft(180), pageSize.GetTop(86));
            cb.ShowText(Header.Fax);
            cb.EndText();

            // página
            int pageN = writer.PageNumber;
            string text = "" + pageN + " / ";
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 6.5f);
            cb.SetTextMatrix(pageSize.GetRight(118), pageSize.GetTop(113));
            cb.ShowText(text);
            cb.EndText();

            float len = bfTexto.GetWidthPoint(text, 6.5f);
            cb.AddTemplate(headerTemplate, pageSize.GetRight(118) + len, pageSize.GetTop(113));

            #endregion


            /*
             ================================================
             TABLA 1: HEADER - DATOS DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DATOS DE LA ORDEN >>>

            var tblOrder = new iTextSharp.text.pdf.PdfPTable(new float[] { 30f, 70f });
            tblOrder.TotalWidth = 160;

            // Fila 1
            var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("SALES ORDER - ORDEN DE VENTA", parrafoTitulo)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5, Colspan = 2, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblOrder.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ORDEN N°", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocNum, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("FECHA", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.TaxDate, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 4
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("CLIENTE N°", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.CardCode, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 5
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("VENDEDOR", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.SlpName, parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            //// Fila 6
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("PAGINA", parrafoNegrita)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5 };
            tblOrder.AddCell(c1);

            // Ubicación de la tabla de la cabecera hacía la derecha en la página
            tblOrder.WriteSelectedRows(0, -1, pageSize.GetLeft(425), pageSize.GetTop(10), cb);

            #endregion



            /*
             ================================================
             TABLA 2: HEADER - DIRECCIONES
            ================================================
            */
            #region <<< TABLA DE DIRECCIONES >>>

            float alturaLinea = 40f;
            float startX = pageSize.GetLeft(10);
            float startY = pageSize.GetTop(110);
            float margenEntreTablas = 10f;

            var tblDireccion = new iTextSharp.text.pdf.PdfPTable(new float[] { 49f, 2f, 49f });
            tblDireccion.TotalWidth = 575;
            tblDireccion.LockedWidth = true;

            // Fila 1
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("VENDIDO A:", parrafoNegrita)) { BorderWidth = 0, PaddingLeft = 5, PaddingTop = 10, PaddingRight = 5, PaddingBottom = 5 };
            tblDireccion.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNegrita)) { BorderWidth = 0, PaddingLeft = 5, PaddingTop = 10, PaddingRight = 5, PaddingBottom = 5 };
            tblDireccion.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("EMBARCADO A: ", parrafoNegrita)) { BorderWidth = 0, PaddingLeft = 5, PaddingTop = 10, PaddingRight = 5, PaddingBottom = 5 };
            tblDireccion.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.CardName, parrafoNormal)) { BorderWidth = 1, BorderWidthBottom = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 3 };
            tblDireccion.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 3 };
            tblDireccion.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(string.IsNullOrWhiteSpace(Header.U_FIB_NEMBA) ? Header.CardName : Header.U_FIB_NEMBA, parrafoNormal)) { BorderWidth = 1, BorderWidthBottom = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 3 };
            tblDireccion.AddCell(c1);

            // Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.LicTradNum, parrafoNormal)) { BorderWidth = 1, BorderWidthTop = 0, BorderWidthBottom = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 3 };
            tblDireccion.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 3 };
            tblDireccion.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 1, BorderWidthTop = 0, BorderWidthBottom = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 3 };
            tblDireccion.AddCell(c1);

            // Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.Address, parrafoNormal)) { BorderWidth = 1, BorderWidthTop = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 10, FixedHeight = alturaLinea, NoWrap = false };
            tblDireccion.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(" ", parrafoNormal)) { BorderWidth = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 10, FixedHeight = alturaLinea, NoWrap = false };
            tblDireccion.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(string.IsNullOrWhiteSpace(Header.U_FIB_DEMBA) ? Header.Address2 : Header.U_FIB_DEMBA, parrafoNormal)) { BorderWidth = 1, BorderWidthTop = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 10, FixedHeight = alturaLinea, NoWrap = false };
            tblDireccion.AddCell(c1);

            // Ubicación de la tabla de la cabecera hacía la izquierda en la página
            tblDireccion.WriteSelectedRows(0, -1, startX, startY, cb);


            // 🔥 ALTURA REAL DE LA TABLA
            float alturaDirecciones = tblDireccion.TotalHeight;

            #endregion


            /*
             ================================================
             TABLA 3: HEADER - DATOS DE EXPORTACION
            ================================================
            */
            #region <<< TABLA DE DATOS DE EXPORTACION >>>

            float startYTabla4 = startY - alturaDirecciones - margenEntreTablas;

            var tblExport = new iTextSharp.text.pdf.PdfPTable(new float[] { 17f, 1f, 32f, 17f, 1f, 32f });
            tblExport.TotalWidth = 575;
            tblExport.LockedWidth = true;

            // Fila 1
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ORDEN COMPRA CLIENTE", parrafoNegrita)) { BorderWidth = 1, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.U_OrdenCompra, parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("CONDICION PAGO", parrafoNegrita)) { BorderWidth = 1, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.PymntGroup, parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("FECHA DE EMBAQUE", parrafoNegrita)) { BorderWidth = 1, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.U_STR_FEMB, parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("CONDICION EMBARQUE", parrafoNegrita)) { BorderWidth = 1, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.U_FIB_PUERTO, parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);

            // Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("PAIS DE ORIGEN", parrafoNegrita)) { BorderWidth = 1, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("PERU", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("VIA DE EMBARQUE", parrafoNegrita)) { BorderWidth = 1, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.TipoFleteDescr, parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);

            // Fila 4
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("PUERTO DE EMBARQUE", parrafoNegrita)) { BorderWidth = 1, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("CALLAO", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("MONEDA", parrafoNegrita)) { BorderWidth = 1, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, BorderWidthRight = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"{Header.DocCurr} {Header.CurrName}", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblExport.AddCell(c1);

            // Ubicación de la tabla de la cabecera hacía la derecha en la página
            tblExport.WriteSelectedRows(0, -1, startX, startYTabla4, cb);


            float alturaExportacion = tblExport.TotalHeight;

            #endregion


            /*
             ================================================
             TABLA 4: HEADER - DETALLE DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DETALLE DE LA ORDEN >>>

            float startYTabla5 = startYTabla4 - alturaExportacion - margenEntreTablas;

            var tblDetail = new iTextSharp.text.pdf.PdfPTable(new float[] { 3f, 22f, 47f, 7f, 6f, 7f, 8f });
            tblDetail.TotalWidth = 575;
            tblDetail.LockedWidth = true;

            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("#", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Item N°", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Descripcion", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("UM", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Cant", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("P.Unit", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Total", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);

            tblDetail.WriteSelectedRows(0, -1, startX, startYTabla5, cb);

            #endregion
        }
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnEndPage(writer, doc);
            iTextSharp.text.Rectangle pageSize = doc.PageSize;
            iTextSharp.text.Font parrafoTexto = new iTextSharp.text.Font(bfTitulo, 5.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(bfTitulo, 7f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoTitulo = new iTextSharp.text.Font(bfTitulo, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.White);
            iTextSharp.text.Font parrafoSubTitulo = new iTextSharp.text.Font(bfTitulo, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

            /*
                =====================================================
                Codigo para que el número de página muestre en el pie
                =====================================================
            */
            //int pageN = writer.PageNumber;
            //string text = "Página " + pageN + "/";
            //float len = bfTexto.GetWidthPoint(text, 8);
            //iTextSharp.text.Rectangle pageSize = document.PageSize;
            //cb.SetRgbColorFill(100, 100, 100);
            //cb.BeginText();
            //cb.SetFontAndSize(bfTexto, 8);
            //cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetBottom(30));
            //cb.ShowText(text);
            //cb.EndText();
            //cb.AddTemplate(footerTemplate, pageSize.GetLeft(15) + len, pageSize.GetBottom(30));


            /*
            ================================================
                TABLA 1: FOOTER - BANCOS
            ================================================
            */
            var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 23f, 1f, 18f, 1f, 20f, 1f, 18f, 1f, 17f });
            tbl.TotalWidth = 575;

            var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.BancoContinental, parrafoNormal)) { BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoTexto)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.BancoPichincha, parrafoNormal)) { BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoTexto)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.BancoSantander, parrafoNormal)) { BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoTexto)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.BancoScotiabank, parrafoNormal)) { BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoTexto)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.BancoInterbank, parrafoNormal)) { BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);

            // Ubicación de la tabla TEXTO
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(260), cb);


            /*
            ================================================
                TABLA 1: FOOTER - TEXTO
            ================================================
            */
            tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 100f });
            tbl.TotalWidth = 575;

            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.Texto, parrafoTexto)) { BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);

            // Ubicación de la tabla TEXTO
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(170), cb);

            /*
            ================================================
                TABLA 1: FOOTER - FIRMA
            ================================================
            */

            tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 45f, 10f, 45f });
            tbl.TotalWidth = 550;

            // Fila 1
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Aprobado por:", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 20 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.PrintHeadr, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 20 };
            tbl.AddCell(c1);

            //// Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.CardName, parrafoNormal)) { BorderWidth = 0, BorderWidthTop = 1, PaddingTop = 5, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Comercio Exterior", parrafoNormal)) { BorderWidth = 0, BorderWidthTop = 1, PaddingTop = 5, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tbl.AddCell(c1);

            // Ubicación de la tabla FIRMA
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(20), pageSize.GetBottom(60), cb);
        }
        public override void OnCloseDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnCloseDocument(writer, doc);
            /*
                ==========================================================
                Codigo para que el número de página muestre en la cabecera
                ==========================================================
            */
            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bfTexto, 6.5f);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            headerTemplate.EndText();

            /*
               =====================================================
               Codigo para que el número de página muestre en el pie
               =====================================================
           */
            //footerTemplate.BeginText();
            //footerTemplate.SetFontAndSize(bfTexto, 8);
            //footerTemplate.SetTextMatrix(0, 0);
            //footerTemplate.ShowText("" + (writer.PageNumber - 1));
            //footerTemplate.EndText();
        }
    }

    #endregion
}
