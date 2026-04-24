using System;
using SAPbobsCOM;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
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
                .OrderByDescending(x => x.DocEntry).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
                resultTransaccion.dataList = list;
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
                        U_tipoOpT12 = s.U_tipoOpT12 ?? "",
                        U_tipoOpT12Nam = s.OperationType != null ? s.OperationType.U_descrp : "",
                    }).ToList()
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
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
                resultTransaccion.data = data;
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

            Documents draft = null;
            Attachments2 attachments = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();

                    // Se crea el objeto de documento borrador
                    draft = company.GetBusinessObject(BoObjectTypes.oDrafts);


                    #region <<< CABECERA >>>

                    draft.DocDate = value.DocDate;
                    draft.DocDueDate = value.DocDueDate;
                    draft.TaxDate = value.TaxDate;

                    draft.DocObjectCode = BoObjectTypes.oOrders;

                    draft.DocType = value.DocType switch
                    {
                        "I" => BoDocumentTypes.dDocument_Items,
                        "S" => BoDocumentTypes.dDocument_Service,
                        _ => throw new ArgumentException($"DocType inválido para SAP Business One: '{value.DocType}'. Se esperaba 'I' (Artículo) o 'S' (Servicio)."),
                    };

                    draft.UserFields.Fields.Item("U_FIB_DocStPkg").Value = value.U_FIB_DocStPkg;
                    draft.UserFields.Fields.Item("U_FIB_IsPkg").Value = value.U_FIB_IsPkg;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    draft.CardCode = value.CardCode;
                    draft.CardName = value.CardName;
                    draft.ContactPersonCode = value.CntctCode;
                    draft.NumAtCard = value.NumAtCard;
                    draft.DocCurrency = value.DocCur;
                    draft.DocRate = value.DocRate;

                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    draft.PayToCode = value.PayToCode;
                    draft.Address = value.Address;
                    draft.ShipToCode = value.ShipToCode;
                    draft.Address2 = value.Address2;

                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================
                    draft.GroupNumber = value.GroupNum;

                    // ===========================================================================================
                    // AGENCIA
                    // ===========================================================================================
                    draft.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                    draft.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    draft.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    draft.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                    draft.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    draft.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                    draft.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                    draft.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                    draft.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                    draft.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;

                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    draft.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    draft.SalesPersonCode = value.SlpCode;
                    draft.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                    draft.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    draft.Comments = value.Comments;

                    // ===========================================================================================
                    // TOTALES
                    // ===========================================================================================
                    draft.DiscountPercent = value.DiscPrcnt;
                    draft.DocTotal = value.DocTotal;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    draft.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                    #endregion


                    #region <<< DETALLE >>>

                    bool isItem = value.DocType == "I";
                    bool isService = value.DocType == "S";

                    foreach (var line in value.Lines)
                    {
                        if (isItem)
                        {
                            draft.Lines.ItemCode = line.ItemCode;
                            draft.Lines.WarehouseCode = line.WhsCode;
                            draft.Lines.MeasureUnit = line.UnitMsr;
                            draft.Lines.Quantity = line.Quantity;
                        }

                        if (isService)
                        {
                            draft.Lines.AccountCode = line.AcctCode;
                        }

                        draft.Lines.ItemDescription = line.Dscription;

                        draft.Lines.Currency = line.Currency;
                        draft.Lines.UnitPrice = line.PriceBefDi;
                        draft.Lines.DiscountPercent = line.DiscPrcnt;
                        draft.Lines.Price = line.Price;

                        draft.Lines.TaxCode = line.TaxCode;
                        draft.Lines.LineTotal = line.LineTotal;

                        // UDFs
                        draft.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                        draft.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                        draft.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        draft.Lines.Add();
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
                        draft.AttachmentEntry = int.Parse(company.GetNewObjectKey());
                    }

                    #endregion


                    if (draft.Add() != 0)
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
                    _companyProviderSap.LiberarObjetosCOM(draft, attachments);
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
                drafts.GroupNumber = value.GroupNum;

                // ===========================================================================================
                // AGENCIA
                // ===========================================================================================
                // Código de agencia de transporte
                drafts.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                // RUC de la agencia de transporte
                drafts.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                // Nombre de la agencia de transporte
                drafts.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                // Código de dirección de la agencia de transporte
                drafts.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                // Dirección de la agencia de transporte
                drafts.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                // ===========================================================================================
                // EXPORTACIÓN
                // ===========================================================================================
                drafts.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                drafts.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                drafts.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                drafts.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                drafts.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;

                // ===========================================================================================
                // OTROS
                // ===========================================================================================
                drafts.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;

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

                // NUEVO: SE AGREGA NUEVO ITEM
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


                if (drafts.Add() != 0)
                {
                    company.GetLastError(out int errorCode, out string errorMessage);
                    throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "La orden de venta registrada con éxito.";

                //// 🔹 Conexión a SAP
                //var company = _companyProviderSap.GetCompany();

                //// Se crea el objeto de Draft
                //drafts = company.GetBusinessObject(BoObjectTypes.oDrafts);

                //// 🔹 Validar existencia del borrador
                //if (!drafts.GetByKey(value.DocEntry))
                //{
                //    throw new Exception("La orden de venta borrador no existe en la base de datos.");
                //}

                //// 🔹 Convertir borrador a documento
                //int res = drafts.SaveDraftToDocument();


                //if (res != 0)
                //{
                //    company.GetLastError(out int errorCode, out string errorMessage);
                //    throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                //}

                //resultTransaccion.IdRegistro = 0;
                //resultTransaccion.ResultadoCodigo = 0;
                //resultTransaccion.ResultadoDescripcion = "La orden de venta registrada con éxito.";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(drafts);
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

            Documents draft = null;
            Attachments2 attachments = null;

            return await Task.Run(() =>
            {
                try
                {
                    // 🔹 Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    // 🔹 Se crea el objeto de orden de venta
                    draft = company.GetBusinessObject(BoObjectTypes.oDrafts);

                    // 🔹 Validar existencia de la orden de venta
                    if (!draft.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe el documento borrador.");
                    }


                    #region <<< CABECERA >>>

                    draft.DocDate = value.DocDate;
                    draft.DocDueDate = value.DocDueDate;
                    draft.TaxDate = value.TaxDate;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    draft.CardCode = value.CardCode;
                    draft.ContactPersonCode = value.CntctCode;
                    draft.NumAtCard = value.NumAtCard;
                    draft.DocCurrency = value.DocCur;
                    draft.DocRate = value.DocRate;

                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    draft.PayToCode = value.PayToCode;
                    draft.Address = value.Address;
                    draft.ShipToCode = value.ShipToCode;
                    draft.Address2 = value.Address2;


                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================
                    draft.GroupNumber = value.GroupNum;

                    // ===========================================================================================
                    // AGENCIA
                    // ===========================================================================================
                    // Código de agencia de transporte
                    draft.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                    // RUC de la agencia de transporte
                    draft.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    // Nombre de la agencia de transporte
                    draft.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    // Código de dirección de la agencia de transporte
                    draft.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                    // Dirección de la agencia de transporte
                    draft.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    draft.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                    draft.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                    draft.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                    draft.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                    draft.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;

                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    draft.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    draft.SalesPersonCode = value.SlpCode;
                    draft.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                    draft.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    draft.Comments = value.Comments;

                    // ===========================================================================================
                    // TOTALES
                    // ===========================================================================================
                    draft.DiscountPercent = value.DiscPrcnt;
                    draft.DocTotal = value.DocTotal;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    draft.UserFields.Fields.Item("U_UsrUpdate").Value = value.U_UsrUpdate;

                    #endregion


                    #region <<< DETALLE >>>

                    bool isItem = value.DocType == "I";
                    bool isService = value.DocType == "S";

                    // NUEVO: SE AGREGA NUEVO ITEM
                    foreach (var line in value.Lines.Where(x => x.Record == 1))
                    {
                        draft.Lines.Add();

                        if (isItem)
                        {
                            draft.Lines.ItemCode = line.ItemCode;
                            draft.Lines.WarehouseCode = line.WhsCode;
                            draft.Lines.Quantity = line.Quantity;
                        }

                        if (isService)
                        {
                            draft.Lines.AccountCode = line.AcctCode;
                        }

                        draft.Lines.ItemDescription = line.Dscription;
                        draft.Lines.Currency = line.Currency;
                        draft.Lines.UnitPrice = line.PriceBefDi;
                        draft.Lines.DiscountPercent = line.DiscPrcnt;
                        draft.Lines.Price = line.Price;

                        draft.Lines.TaxCode = line.TaxCode;
                        draft.Lines.LineTotal = line.LineTotal;

                        draft.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                        draft.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                        draft.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                    }

                    // EXISTE: SE MODIFICA EL ITEM
                    foreach (var line in value.Lines.Where(x => x.Record == 2 && x.LineStatus == "O"))
                    {
                        for (int i = 0; i < draft.Lines.Count; i++)
                        {
                            draft.Lines.SetCurrentLine(i);

                            if (draft.Lines.LineNum != line.LineNum)
                                continue;

                            if (isItem)
                            {
                                draft.Lines.ItemCode = line.ItemCode;
                                draft.Lines.WarehouseCode = line.WhsCode;
                                draft.Lines.Quantity = line.Quantity;
                            }

                            if (isService)
                            {
                                draft.Lines.AccountCode = line.AcctCode;
                            }

                            draft.Lines.ItemDescription = line.Dscription;
                            draft.Lines.Currency = line.Currency;
                            draft.Lines.UnitPrice = line.PriceBefDi;
                            draft.Lines.DiscountPercent = line.DiscPrcnt;
                            draft.Lines.Price = line.Price;

                            draft.Lines.TaxCode = line.TaxCode;
                            draft.Lines.LineTotal = line.LineTotal;

                            draft.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                            draft.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                            draft.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        }
                    }

                    // EXISTE: SE ELIMINA EL ITEM
                    foreach (var line in value.Lines.Where(x => x.Record == 3))
                    {
                        for (int i = 0; i < draft.Lines.Count; i++)
                        {
                            draft.Lines.SetCurrentLine(i);
                            if (draft.Lines.LineNum == line.LineNum)
                            {
                                draft.Lines.Delete();
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
                        draft.AttachmentEntry = int.Parse(company.GetNewObjectKey());
                    }

                    #endregion


                    if (draft.Update() != 0)
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
                    _companyProviderSap.LiberarObjetosCOM(draft, attachments);
                }

                return resultTransaccion;
            });
        }

        #endregion
    }
}
