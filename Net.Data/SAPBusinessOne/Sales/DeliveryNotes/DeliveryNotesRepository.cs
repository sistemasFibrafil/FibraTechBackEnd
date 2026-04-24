using System;
using System.IO;
using SAPbobsCOM;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Close;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Create;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Update;
using Net.Business.Entities.SAPBusinessOne.Sales.DeliveryNotes.Cancel;
namespace Net.Data.SAPBusinessOne
{
    public class DeliveryNotesRepository : RepositoryBase<DeliveryNotesEntity>, IDeliveryNotesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        public DeliveryNotesRepository(IConnectionSQL context, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }


        #region <<< CONSULTAS >>>

        public async Task<ResultadoTransaccionResponse<DeliveryNotesQueryEntity>> GetListByFilter(DeliveryNotesFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DeliveryNotesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.DeliveryNotes
                .AsNoTracking()
                .Where(x => x.DocDate >= value.StartDate && x.DocDate <= value.EndDate);


                // FILTRO DE TEXTO
                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    bool hasLetter = filter.Any(char.IsLetter);
                    bool hasDigit = filter.Any(char.IsDigit);
                    bool hasHyphen = filter.Contains('-');

                    if (hasLetter)
                    {
                        query = query.Where(n =>
                        EF.Functions.Like(n.U_BPP_MDSD, $"%{filter}%") ||
                        EF.Functions.Like(n.U_BPP_MDSD + "-" + n.U_BPP_MDCD, $"%{filter}%")
                        );
                    }
                    else if (hasDigit && hasHyphen)
                    {
                        query = query.Where(n =>
                            EF.Functions.Like(n.U_BPP_MDSD + "-" + n.U_BPP_MDCD, $"%{filter}%")
                        );
                    }
                    else if (hasDigit && !hasLetter && !hasHyphen)
                    {
                        query = query.Where(n =>
                            EF.Functions.Like(n.DocNum.ToString(), $"%{filter}%") ||
                            EF.Functions.Like(n.CardCode, $"%{filter}%") ||
                            EF.Functions.Like(n.CardName, $"%{filter}%") ||
                            EF.Functions.Like(n.U_BPP_MDSD, $"%{filter}%") ||
                            EF.Functions.Like(n.U_BPP_MDCD, $"%{filter}%")
                        );
                    }
                }

                if (!string.IsNullOrWhiteSpace(value.DocStatus))
                {
                    var docStatus = value.DocStatus.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(x => docStatus.Contains(x.DocStatus));
                }

                var list = await query
                .Select(n => new DeliveryNotesQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    DocType = n.DocType,
                    Canceled = n.CANCELED,
                    DocStatus = n.DocStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,

                    U_BPP_MDSD = n.U_BPP_MDSD,
                    U_BPP_MDCD = n.U_BPP_MDCD,

                    U_FIB_FromPkg = n.U_FIB_FromPkg,

                    CardCode = n.CardCode,
                    CardName = n.CardName,
                    GroupCode = n.BusinessPartners.GroupCode,
                    GroupName = n.BusinessPartners.BusinessPartnerGroups.GroupName,
                    DocCur = n.DocCur,

                    SlpName = n.SalesPersons != null ? n.SalesPersons.SlpName : "",

                    DocTotal = n.DocTotal,
                    DocTotalSy = n.DocTotalSy,
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
        public async Task<ResultadoTransaccionResponse<DeliveryNotesQueryEntity>> GetByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DeliveryNotesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var adminInfo = await _db.AdminInfo
                .Select(n => new
                {
                    MaMainCurncy = n.MainCurncy
                })
                .FirstOrDefaultAsync();

                var data = await _db.DeliveryNotes
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new DeliveryNotesQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    ObjType = n.ObjType,
                    DocType = n.DocType,
                    DocStatus = n.DocStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,

                    U_BPP_MDTD = n.U_BPP_MDTD,
                    U_BPP_MDSD = n.U_BPP_MDSD,
                    U_BPP_MDCD = n.U_BPP_MDCD,

                    // SOCIO DE NEGOCIOS
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

                    // FINANZAS
                    GroupNum = n.GroupNum,

                    // LOGÍSTICA
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

                    // AGENCIA
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

                    // TRANSPORTISTA
                    U_FIB_TIP_TRANS = n.U_FIB_TIP_TRANS,
                    U_FIB_COD_TRA = n.U_FIB_COD_TRA,
                    U_FIB_TIPDOC_TRA = n.U_FIB_TIPDOC_TRA,
                    U_FIB_RUC_TRANS2 = n.U_FIB_RUC_TRANS2,
                    U_FIB_TRANS2 = n.U_FIB_TRANS2,
                    U_BPP_MDVC = n.U_BPP_MDVC,
                    U_FIB_TIPDOC_COND = n.U_FIB_TIPDOC_COND,
                    U_FIB_NUMDOC_COD = n.U_FIB_NUMDOC_COD,
                    U_FIB_NOM_COND = n.U_FIB_NOM_COND,
                    U_FIB_APE_COND = n.U_FIB_APE_COND,
                    U_BPP_MDFN = n.U_BPP_MDFN,
                    U_BPP_MDFC = n.U_BPP_MDFC,

                    // EXPORTACION
                    U_RUCDestInter = n.U_RUCDestInter,
                    U_DestGuiaInter = n.U_DestGuiaInter,
                    U_DireccDestInter = n.U_DireccDestInter,
                    U_STR_NCONTENEDOR = n.U_STR_NCONTENEDOR,
                    U_STR_NPRESCINTO = n.U_STR_NPRESCINTO,
                    U_FIB_NPRESCINTO2 = n.U_FIB_NPRESCINTO2,
                    U_FIB_NPRESCINTO3 = n.U_FIB_NPRESCINTO3,
                    U_FIB_NPRESCINTO4 = n.U_FIB_NPRESCINTO4,

                    // OTROS
                    U_STR_TVENTA = n.U_STR_TVENTA,
                    U_BPP_MDMT = n.U_BPP_MDMT,
                    U_BPP_MDOM = n.U_BPP_MDOM,

                    // SALES EMPLOYEE
                    SlpCode = n.SlpCode,
                    U_FIB_NBULTOS = n.U_FIB_NBULTOS ?? 0,
                    U_FIB_KG = n.U_FIB_KG ?? 0,
                    U_NroOrden = n.U_NroOrden,
                    U_OrdenCompra = n.U_OrdenCompra,
                    Comments = n.Comments,

                    // TOTALES
                    SubTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal - n.VatSum + n.DiscSum : n.DocTotalSy - n.VatSumSy + n.DiscSumSy,
                    DiscPrcnt = n.DiscPrcnt ?? 0,
                    DiscSum = adminInfo.MaMainCurncy == n.DocCur ? n.DiscSum : n.DiscSumSy,
                    VatSum = adminInfo.MaMainCurncy == n.DocCur ? n.VatSum : n.VatSumSy,
                    DocTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal : n.DocTotalSy,

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines.Select(s => new DeliveryNotes1QueryEntity
                    {
                        DocEntry = s.DocEntry,
                        LineNum = s.LineNum,
                        LineStatus = s.LineStatus,
                        ObjType = s.ObjType,
                        BaseType = s.BaseType,
                        BaseEntry = s.BaseEntry,
                        BaseLine = s.BaseLine,

                        U_FIB_FromPkg = s.U_FIB_FromPkg,

                        ItemCode = s.ItemCode,
                        Dscription = s.Dscription,
                        AcctCode = s.AcctCode,
                        FormatCode = s.ChartOfAccounts != null ? s.ChartOfAccounts.Segment_0 + "-" + s.ChartOfAccounts.Segment_1 + "-" + s.ChartOfAccounts.Segment_2 : "",
                        AcctName = s.ChartOfAccounts != null ? s.ChartOfAccounts.AcctName : "",
                        WhsCode = s.WhsCode,

                        UnitMsr = s.UnitMsr,
                        OnHand = s.Item.OnHand,
                        Quantity = s.Quantity,
                        OpenQty = s.OpenQty,
                        U_FIB_NBulto = s.U_FIB_NBulto,
                        U_FIB_PesoKg = s.U_FIB_PesoKg,

                        Currency = s.Currency,
                        PriceBefDi = s.PriceBefDi,
                        DiscPrcnt = s.DiscPrcnt ?? 0,
                        Price = s.Price,
                        TaxCode = s.TaxCode,
                        VatPrcnt = s.VatPrcnt ?? 0,
                        VatSum = adminInfo.MaMainCurncy == s.Currency ? s.VatSum : s.VatSumSy,
                        U_tipoOpT12 = s.U_tipoOpT12 ?? "",
                        U_tipoOpT12Nam = s.OperationType != null ? s.OperationType.U_descrp : "",
                        LineTotal = adminInfo.MaMainCurncy == s.Currency ? s.LineTotal : s.TotalSumSy,
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

        #endregion


        #region <<< OPERACIONES >>>

        public async Task<ResultadoTransaccionResponse<DeliveryNotesEntity>> SetCreate(DeliveryNotesCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DeliveryNotesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            Documents deliveryNotes = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralParams = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();

                    // Empieza la transacción en SAP
                    if (!company.InTransaction) company.StartTransaction();

                    // Se crea el objeto de entrega
                    deliveryNotes = company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);


                    #region <<< CABECERA >>>

                    deliveryNotes.DocDate = value.DocDate;
                    deliveryNotes.DocDueDate = value.DocDueDate;
                    deliveryNotes.TaxDate = value.TaxDate;
                    deliveryNotes.DocObjectCode = BoObjectTypes.oOrders;

                    deliveryNotes.DocType = value.DocType switch
                    {
                        "I" => BoDocumentTypes.dDocument_Items,
                        "S" => BoDocumentTypes.dDocument_Service,
                        _ => throw new ArgumentException($"DocType inválido para SAP Business One: '{value.DocType}'. Se esperaba 'I' (Artículo) o 'S' (Servicio)."),
                    };

                    deliveryNotes.UserFields.Fields.Item("U_FIB_FromPkg").Value =
                    value.PickingLines.Count switch
                    {
                        0 => "N",
                        _ => "Y"
                    };

                    // ===========================================================================================
                    // SUNAT
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDTD").Value = value.U_BPP_MDTD;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDSD").Value = value.U_BPP_MDSD;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDCD").Value = value.U_BPP_MDCD;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    deliveryNotes.CardCode = value.CardCode;
                    deliveryNotes.CardName = value.CardName;
                    deliveryNotes.ContactPersonCode = value.CntctCode;
                    deliveryNotes.NumAtCard = value.NumAtCard;
                    deliveryNotes.DocCurrency = value.DocCur;
                    deliveryNotes.DocRate = value.DocRate;

                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    deliveryNotes.PayToCode = value.PayToCode;
                    deliveryNotes.Address = value.Address;
                    deliveryNotes.ShipToCode = value.ShipToCode;
                    deliveryNotes.Address2 = value.Address2;

                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================
                    deliveryNotes.GroupNumber = value.GroupNum;

                    // ===========================================================================================
                    // AGENCIA
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                    // ===========================================================================================
                    // TRANSPORTISTA
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_FIB_TIP_TRANS").Value = value.U_FIB_TIP_TRANS;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_COD_TRA").Value = value.U_FIB_COD_TRA;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_TIPDOC_TRA").Value = value.U_FIB_TIPDOC_TRA;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_RUC_TRANS2").Value = value.U_FIB_RUC_TRANS2;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_TRANS2").Value = value.U_FIB_TRANS2;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDVC").Value = value.U_BPP_MDVC;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_TIPDOC_COND").Value = value.U_FIB_TIPDOC_COND;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NUMDOC_COD").Value = value.U_FIB_NUMDOC_COD;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NOM_COND").Value = value.U_FIB_NOM_COND;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_APE_COND").Value = value.U_FIB_APE_COND;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDFN").Value = value.U_BPP_MDFN;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDFC").Value = value.U_BPP_MDFC;

                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_RUCDestInter").Value = value.U_RUCDestInter;
                    deliveryNotes.UserFields.Fields.Item("U_DestGuiaInter").Value = value.U_DestGuiaInter;
                    deliveryNotes.UserFields.Fields.Item("U_DireccDestInter").Value = value.U_DireccDestInter;
                    deliveryNotes.UserFields.Fields.Item("U_STR_NCONTENEDOR").Value = value.U_STR_NCONTENEDOR;
                    deliveryNotes.UserFields.Fields.Item("U_STR_NPRESCINTO").Value = value.U_STR_NPRESCINTO;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NPRESCINTO2").Value = value.U_FIB_NPRESCINTO2;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NPRESCINTO3").Value = value.U_FIB_NPRESCINTO3;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NPRESCINTO4").Value = value.U_FIB_NPRESCINTO4;

                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDMT").Value = value.U_BPP_MDMT;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDOM").Value = value.U_BPP_MDOM;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    deliveryNotes.SalesPersonCode = value.SlpCode;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NBULTOS").Value = value.U_FIB_NBULTOS;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_KG").Value = value.U_FIB_KG;
                    deliveryNotes.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                    deliveryNotes.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    deliveryNotes.Comments = value.Comments;

                    // ===========================================================================================
                    // TOTALES
                    // ===========================================================================================
                    deliveryNotes.DiscountPercent = value.DiscPrcnt;
                    deliveryNotes.DocTotal = value.DocTotal;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                    #endregion


                    #region <<< DETALLE >>>

                    bool isItem = value.DocType == "I";
                    bool isService = value.DocType == "S";

                    foreach (var line in value.Lines)
                    {
                        if (isItem)
                        {
                            deliveryNotes.Lines.ItemCode = line.ItemCode;
                            deliveryNotes.Lines.WarehouseCode = line.WhsCode;
                            deliveryNotes.Lines.Quantity = line.Quantity;
                        }

                        if (isService)
                        {
                            deliveryNotes.Lines.AccountCode = line.AcctCode;
                        }

                        if (line.BaseType != null)  deliveryNotes.Lines.BaseType = (int)line.BaseType;
                        if (line.BaseEntry != null) deliveryNotes.Lines.BaseEntry = (int)line.BaseEntry;
                        if (line.BaseLine != null) deliveryNotes.Lines.BaseLine = (int)line.BaseLine;

                        deliveryNotes.Lines.ItemDescription = line.Dscription;
                        deliveryNotes.Lines.Currency = line.Currency;
                        deliveryNotes.Lines.UnitPrice = line.PriceBefDi;
                        deliveryNotes.Lines.DiscountPercent = line.DiscPrcnt;
                        deliveryNotes.Lines.Price = line.Price;

                        deliveryNotes.Lines.TaxCode = line.TaxCode;
                        deliveryNotes.Lines.LineTotal = line.LineTotal;

                        deliveryNotes.Lines.UserFields.Fields.Item("U_FIB_FromPkg").Value = 
                        value.PickingLines.Count switch
                        {
                            0 => "N",
                            _ => "Y"
                        };
                        deliveryNotes.Lines.UserFields.Fields.Item("U_FIB_NBulto").Value = line.U_FIB_NBulto;
                        deliveryNotes.Lines.UserFields.Fields.Item("U_FIB_PesoKg").Value = line.U_FIB_PesoKg;
                        deliveryNotes.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        deliveryNotes.Lines.Add();
                    }

                    #endregion


                    var reg = deliveryNotes.Add();


                    if (reg != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }


                    #region <<< PICKING >>>

                    if (value.PickingLines.Count > 0)
                    {
                        // Se obtiene el docEntry del objeto que se acaba de crear
                        var key = company.GetNewObjectKey();
                        var docEntry = key == null ? 0 : int.Parse(key);


                        // Se crea el objeto de la entrega
                        deliveryNotes = company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);


                        // Se obtiene la entrega registrado
                        if (!deliveryNotes.GetByKey(docEntry)) throw new Exception("No existe la entrega.");


                        //// Se valida que la entrega tenga líneas en el detalle
                        if (deliveryNotes.Lines.Count == 0) throw new Exception("No existe el detalle de la entrega.");


                        // Se instancia el servicio de objetos generales
                        oCompService = company.GetCompanyService();
                        // se instancia el objeto de datos generales
                        oGeneralService = oCompService.GetGeneralService("FIB_OPKG");
                        foreach (var line in value.PickingLines)
                        {
                            // Se recorre las líneas de la entrega
                            for (int i = 0; i < deliveryNotes.Lines.Count; i++)
                            {
                                deliveryNotes.Lines.SetCurrentLine(i);
                                if (deliveryNotes.Lines.BaseEntry == line.U_BaseEntry && deliveryNotes.Lines.BaseLine == line.U_BaseLine)
                                {
                                    // Se crea una nueva instancia del objeto de datos generales
                                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                    // Se asigna el valor de clave primaria a actualizar
                                    oGeneralParams.SetProperty("DocEntry", line.DocEntry);
                                    // Se obtiene el objeto de datos generales
                                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                                    // Se asignan los valores a actualizar
                                    oGeneralData.SetProperty("U_TrgetEntry", deliveryNotes.Lines.DocEntry);
                                    oGeneralData.SetProperty("U_TargetType", BoObjectTypes.oDeliveryNotes);
                                    oGeneralData.SetProperty("U_TrgetLine", deliveryNotes.Lines.LineNum);
                                    oGeneralData.SetProperty("U_Status", line.U_Status);
                                    oGeneralData.SetProperty("U_UsrUpdate", line.U_UsrUpdate);
                                    // Se actualiza el objeto de datos generales
                                    oGeneralService.Update(oGeneralData);
                                    oGeneralService.Close(oGeneralParams);
                                    break;
                                }
                            }
                        }
                    }

                    #endregion


                    // Confirma la transacción
                    if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_Commit);


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Entrega registrada con éxito.";
                }
                catch (Exception ex)
                {
                    if (company != null && company.Connected && company.InTransaction)
                        company.EndTransaction(BoWfTransOpt.wf_RollBack);

                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(deliveryNotes, oGeneralData, oCompService, oGeneralService, oGeneralParams);
                }

                return resultTransaccion;
            });
        }
        public async Task<ResultadoTransaccionResponse<DeliveryNotesEntity>> SetUpdate(DeliveryNotesUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DeliveryNotesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            Documents deliveryNotes = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Se crea el objeto de entrega
                    deliveryNotes = company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);

                    if (!deliveryNotes.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la entrega.");
                    }


                    #region <<< CABECERA >>>

                    if (deliveryNotes.DocumentStatus == BoStatus.bost_Open)
                    {
                        deliveryNotes.DocDueDate = value.DocDueDate;
                    }

                    // ===========================================================================================
                    // SUNAT
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDTD").Value = value.U_BPP_MDTD;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDSD").Value = value.U_BPP_MDSD;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDCD").Value = value.U_BPP_MDCD;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    deliveryNotes.CardCode = value.CardCode;
                    deliveryNotes.ContactPersonCode = value.CntctCode;
                    deliveryNotes.NumAtCard = value.NumAtCard;

                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    if(deliveryNotes.DocumentStatus == BoStatus.bost_Open)
                    {
                        deliveryNotes.PayToCode = value.PayToCode;
                        deliveryNotes.Address = value.Address;
                    }

                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================                    

                    // ===========================================================================================
                    // AGENCIA
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                    // ===========================================================================================
                    // TRANSPORTISTA
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_FIB_TIP_TRANS").Value = value.U_FIB_TIP_TRANS;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_COD_TRA").Value = value.U_FIB_COD_TRA;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_TIPDOC_TRA").Value = value.U_FIB_TIPDOC_TRA;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_RUC_TRANS2").Value = value.U_FIB_RUC_TRANS2;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_TRANS2").Value = value.U_FIB_TRANS2;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDVC").Value = value.U_BPP_MDVC;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_TIPDOC_COND").Value = value.U_FIB_TIPDOC_COND;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NUMDOC_COD").Value = value.U_FIB_NUMDOC_COD;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NOM_COND").Value = value.U_FIB_NOM_COND;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_APE_COND").Value = value.U_FIB_APE_COND;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDFN").Value = value.U_BPP_MDFN;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDFC").Value = value.U_BPP_MDFC;

                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_RUCDestInter").Value = value.U_RUCDestInter;
                    deliveryNotes.UserFields.Fields.Item("U_DestGuiaInter").Value = value.U_DestGuiaInter;
                    deliveryNotes.UserFields.Fields.Item("U_DireccDestInter").Value = value.U_DireccDestInter;
                    deliveryNotes.UserFields.Fields.Item("U_STR_NCONTENEDOR").Value = value.U_STR_NCONTENEDOR;
                    deliveryNotes.UserFields.Fields.Item("U_STR_NPRESCINTO").Value = value.U_STR_NPRESCINTO;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NPRESCINTO2").Value = value.U_FIB_NPRESCINTO2;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NPRESCINTO3").Value = value.U_FIB_NPRESCINTO3;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NPRESCINTO4").Value = value.U_FIB_NPRESCINTO4;

                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDMT").Value = value.U_BPP_MDMT;
                    deliveryNotes.UserFields.Fields.Item("U_BPP_MDOM").Value = value.U_BPP_MDOM;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================                    
                    deliveryNotes.UserFields.Fields.Item("U_FIB_NBULTOS").Value = value.U_FIB_NBULTOS;
                    deliveryNotes.UserFields.Fields.Item("U_FIB_KG").Value = value.U_FIB_KG;
                    deliveryNotes.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                    deliveryNotes.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    deliveryNotes.Comments = value.Comments;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    deliveryNotes.UserFields.Fields.Item("U_UsrUpdate").Value = value.U_UsrUpdate;

                    #endregion


                    var reg = deliveryNotes.Update();


                    if (reg != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Entrega registrada con éxito.";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(deliveryNotes);
                }

                return resultTransaccion;
            });
        }
        public async Task<ResultadoTransaccionResponse<DeliveryNotesEntity>> SetCancel(DeliveryNotesCancelEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DeliveryNotesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Recordset rs = null;
            Company company = null;
            Documents deliveryNotes = null;
            Documents deliveryNotesCancel = null;
            Documents deliveryNotesUpdate = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Iniciar transacción
                    if (!company.InTransaction)
                        company.StartTransaction();


                    deliveryNotes = company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);


                    #region <<< CANCELAR >>>

                    if (!deliveryNotes.GetByKey(value.DocEntry))
                        throw new Exception("No existe la entrega.");

                    deliveryNotesCancel = deliveryNotes.CreateCancellationDocument();

                    if (deliveryNotesCancel == null)
                        throw new Exception("No se pudo crear el documento de cancelación.");

                    int regCancel = deliveryNotesCancel.Add();

                    if (regCancel != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                    }

                    #endregion


                    #region <<< ACTUALIZAR UDF CON SQL >>>

                    // Obtener el código de tipo de documento de la entrega
                    var u_BPP_TDTD = _db.TipoDocumentoSunat
                   .Where(x => x.U_FIB_ENAN == "Y")
                   .Select(x => x.U_BPP_TDTD)
                   .FirstOrDefault() ?? "";

                    // Obtener correlativo de documentO
                    var documentNumberingSeriesSunat = _db.DocumentNumberingSeriesSunat
                    .Where(x => x.U_BPP_NDTD == u_BPP_TDTD)
                    .Select(x => new
                    {
                        x.U_BPP_NDTD,
                        x.U_BPP_NDSD,
                        x.U_BPP_NDCD
                    })
                    .FirstOrDefault() ?? throw new Exception("El correlativo de documento anular no existe.");

                    var key = company.GetNewObjectKey();
                    int docEntry = key == null ? 0 : int.Parse(key);

                    rs = company.GetBusinessObject(BoObjectTypes.BoRecordset);

                    rs.DoQuery($@"
                        UPDATE ODLN
                        SET 
                            U_BPP_MDTD = '{documentNumberingSeriesSunat.U_BPP_NDTD}',
                            U_BPP_MDSD = '{documentNumberingSeriesSunat.U_BPP_NDSD}',
                            U_BPP_MDCD = '{documentNumberingSeriesSunat.U_BPP_NDCD}',
                            U_UsrCreate = '{value.U_UsrCreate}'
                        WHERE DocEntry = {docEntry}
                    ");

                    #endregion


                    #region <<< ACTUALIZAR >>>

                    deliveryNotesUpdate = company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);

                    if (!deliveryNotesUpdate.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la entrega cancelada.");
                    }

                    deliveryNotesUpdate.Indicator = "ZA";
                    deliveryNotesUpdate.UserFields.Fields.Item("U_UsrCancel").Value = value.U_UsrCancel;

                    var regUpdate = deliveryNotesUpdate.Update();

                    if (regUpdate != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                    }

                    #endregion


                    // Confima la transacción
                    if (company.InTransaction)
                        company.EndTransaction(BoWfTransOpt.wf_Commit);

                    resultTransaccion.IdRegistro = docEntry;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Entrega cancelada con éxito.";
                }
                catch (Exception ex)
                {
                    if (company != null && company.Connected && company.InTransaction)
                        company.EndTransaction(BoWfTransOpt.wf_RollBack);

                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message;
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(rs, deliveryNotes, deliveryNotesCancel, deliveryNotesUpdate);
                }

                return resultTransaccion;
            });
        }
        public async Task<ResultadoTransaccionResponse<DeliveryNotesEntity>> SetClose(DeliveryNotesCloseEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DeliveryNotesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Recordset rs = null;
            Documents deliveryNotes = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    // Iniciar transacción
                    if (!company.InTransaction)
                        company.StartTransaction();


                    deliveryNotes = company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);


                    #region <<< CERRAR >>>

                    if (!deliveryNotes.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la entrega.");
                    }


                    var regClose = deliveryNotes.Close();


                    if(regClose != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                    }

                    #endregion


                    #region <<< ACTUALIZAR UDF CON SQL >>>

                    rs = company.GetBusinessObject(BoObjectTypes.BoRecordset);

                    rs.DoQuery($@"
                        UPDATE ODLN
                        SET 
                            U_UsrClose = '{value.U_UsrClose}'
                        WHERE DocEntry = {value.DocEntry}
                    ");

                    #endregion


                    // Confima la transacción
                    if (company.InTransaction)
                        company.EndTransaction(BoWfTransOpt.wf_Commit);


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Entrega cerrada con éxito ..!";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(deliveryNotes);
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

                    PrintHeadr = a.PrintHeadr,
                    Phone1 = a.Phone1,
                    Fax = a.Fax,

                    Street = a.AdminInfo1.Street,
                    County = a.AdminInfo1.County,
                    City = a.AdminInfo1.City,

                    CountryName = a.AdminInfo1.CountryEntity.Name
                })
                .FirstOrDefaultAsync();


                var data = await _db.DeliveryNotes
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new DeliveryNotesQueryEntity
                {
                    DocNum = n.DocNum,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    DocTime = n.DocTime.ToString().Insert(n.DocTime.ToString().Length - 2, ":").PadLeft(5, '0'),
                    UserName = n.Users.U_NAME,


                    // SOCIO DE NEGOCIOS
                    LicTradNum = n.BusinessPartners.LicTradNum ?? "",
                    CardName = n.CardName ?? "",
                    Phone1 = n.BusinessPartners.Phone1 ?? "",

                    // FINANZAS
                    PymntGroup = n.PaymentTermsTypes.PymntGroup,

                    // LOGÍSTICA
                    Address2 = n.Address2 ?? "",

                    // AGENCIA
                    U_BPP_MDNT = n.U_BPP_MDNT ?? "",
                    U_BPP_MDRT = n.U_BPP_MDRT ?? "",
                    U_BPP_MDDT = n.U_BPP_MDDT ?? "",

                    // TRANSPORTISTA
                    U_FIB_RUC_TRANS2 = n.U_FIB_RUC_TRANS2 ?? "",
                    U_BPP_MDVC = n.U_BPP_MDVC ?? "",
                    U_BPP_MDFN = n.U_BPP_MDFN ?? "",
                    U_BPP_MDFC = n.U_BPP_MDFC ?? "",

                    // SALES EMPLOYEE
                    SlpName = n.SalesPersons.SlpName,
                    U_OrdenCompra = n.U_OrdenCompra ?? "",
                    U_NroOrden = n.U_NroOrden ?? "",
                    U_FIB_NBULTOS = n.U_FIB_NBULTOS ?? 0,
                    U_FIB_KG = n.U_FIB_KG ?? 0,

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines
                    .OrderBy(s => s.LineNum)
                    .Select(s => new DeliveryNotes1QueryEntity
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


                var header = new HeaderDeliveryNotesNational()
                {
                    PrintHeadr = adminInfo.PrintHeadr,

                    DocNum = data.DocNum.ToString(),
                    DocDate = data.DocDate.ToString("dd/MM/yyyy"),
                    DocDueDate = data.DocDueDate.ToString("dd/MM/yyyy"),
                    DocTime = data.DocTime,

                    CardName = data.CardName,
                    LicTradNum = data.LicTradNum,
                    Phone1 = data.Phone1,

                    PuntoPartida = "",
                    PuntoLlegada = data.Address2,

                    SlpName = data.SlpName,
                    U_OrdenCompra = data.U_OrdenCompra,
                };


                var footer = new FooterDeliveryNotesNational()
                {
                    // AGENCIA
                    U_BPP_MDNT = data.U_BPP_MDNT,
                    U_BPP_MDRT = data.U_BPP_MDRT,
                    U_BPP_MDDT = data.U_BPP_MDDT,

                    // TRANSPORTISTA
                    U_FIB_RUC_TRANS2 = data.U_FIB_RUC_TRANS2,
                    U_BPP_MDVC = data.U_BPP_MDVC,
                    U_BPP_MDFN = data.U_BPP_MDFN,
                    U_BPP_MDFC = data.U_BPP_MDFC,

                    // DOCUMENTO
                    UserName = data.UserName,
                    DocNum = data.DocNum.ToString(),
                    DocTime = data.DocTime,

                    U_NroOrden = data.U_NroOrden,
                    U_FIB_NBULTOS = data.U_FIB_NBULTOS,
                    U_FIB_KG = data.U_FIB_KG,
                };


                iTextSharp.text.Document doc = new iTextSharp.text.Document();
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                doc.SetMargins(10f, 10f, 300f, 280f);
                MemoryStream ms = new MemoryStream();
                iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                // Our custom Header and Footer is done using Event Handler
                var pageEventHelper = new PageEventHelperDeliveryNotesNational();
                write.PageEvent = pageEventHelper;

                // Colocamos la fuente que deseamos que tenga el documento
                iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
                iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                iTextSharp.text.Font parrafoNegrita = new iTextSharp.text.Font(helvetica, 7f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

                // Define the page header
                pageEventHelper.Header = header;
                pageEventHelper.Footer = footer;

                doc.Open();


                //============================
                //TABLA: DETALLE
                var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 3f, 22f, 7f, 10f, 58f }) { WidthPercentage = 100 };
                var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                for (int i = 0; i < data.Lines.Count; i++)
                {
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase((i + 1).ToString(), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].ItemCode, parrafoNormal)) { BorderWidth = 0, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].UnitMsr.ToUpper(), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Quantity.ToString("N2"), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Dscription, parrafoNormal)) { BorderWidth = 0, PaddingBottom = 4 };
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
                resultTransaccion.data = file;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetPrintExportDocEntry(int docEntry)
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
                    CmpnyAddrF = a.CmpnyAddrF,

                    MaMainCurncy = a.MainCurncy,

                    PrintHeadr = a.PrintHeadr,
                    Phone1 = a.Phone1,
                    Fax = a.Fax,

                    Street = a.AdminInfo1.Street,
                    County = a.AdminInfo1.County,
                    City = a.AdminInfo1.City,

                    CountryName = a.AdminInfo1.CountryEntity.Name,

                })
                .FirstOrDefaultAsync();


                var data = await _db.DeliveryNotes
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new DeliveryNotesQueryEntity
                {
                    DocNum = n.DocNum,
                    DocDueDate = n.DocDueDate,
                    DocTime = n.DocTime.ToString().Insert(n.DocTime.ToString().Length - 2, ":").PadLeft(5, '0'),
                    UserName = n.Users.U_NAME,

                    // AGENCIA
                    U_BPP_MDNT = n.U_BPP_MDNT ?? "",
                    U_BPP_MDRT = n.U_BPP_MDRT ?? "",
                    U_BPP_MDDT = n.U_BPP_MDDT ?? "",

                    // TRANSPORTISTA
                    U_FIB_RUC_TRANS2 = n.U_FIB_RUC_TRANS2 ?? "",
                    U_BPP_MDVC = n.U_BPP_MDVC ?? "",
                    U_BPP_MDFN = n.U_BPP_MDFN ?? "",
                    U_BPP_MDFC = n.U_BPP_MDFC ?? "",

                    // EXPORTACIÓN
                    U_DestGuiaInter = n.U_DestGuiaInter ?? "",
                    U_RUCDestInter = n.U_RUCDestInter ?? "",
                    U_DireccDestInter = n.U_DireccDestInter ?? "",
                    U_STR_NCONTENEDOR = n.U_STR_NCONTENEDOR ?? "",
                    U_STR_NPRESCINTO = n.U_STR_NPRESCINTO ?? "",
                    U_FIB_NPRESCINTO2 = n.U_FIB_NPRESCINTO2 ?? "",
                    U_FIB_NPRESCINTO3 = n.U_FIB_NPRESCINTO3 ?? "",
                    U_FIB_NPRESCINTO4 = n.U_FIB_NPRESCINTO4 ?? "",

                    // SALES EMPLOYEE
                    SlpName = n.SalesPersons.SlpName,
                    U_NroOrden = n.U_NroOrden ?? "",
                    U_OrdenCompra = n.U_OrdenCompra ?? "",
                    U_FIB_NBULTOS = n.U_FIB_NBULTOS ?? 0,
                    U_FIB_KG = n.U_FIB_KG ?? 0,

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines
                    .OrderBy(s => s.LineNum)
                    .Select(s => new DeliveryNotes1QueryEntity
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


                var header = new HeaderDeliveryNotesExport()
                {
                    PrintHeadr = adminInfo.PrintHeadr,

                    DocNum = data.DocNum.ToString(),
                    DocDueDate = data.DocDueDate.ToString("dd/MM/yyyy"),
                    DocTime = data.DocTime,

                    U_DestGuiaInter = data.U_DestGuiaInter,
                    U_RUCDestInter = data.U_RUCDestInter,
                    U_DireccDestInter = data.U_DireccDestInter,

                    PuntoPartida = adminInfo.CmpnyAddrF,
                };


                var footer = new FooterDeliveryNotesExport()
                {
                    // DOCUMENTO
                    UserName = data.UserName,
                    DocNum = data.DocNum.ToString(),
                    DocTime = data.DocTime,

                    // AGENCIA
                    U_BPP_MDNT = data.U_BPP_MDNT,
                    U_BPP_MDRT = data.U_BPP_MDRT,
                    U_BPP_MDDT = data.U_BPP_MDDT,

                    // TRANSPORTISTA
                    U_BPP_MDVC = data.U_BPP_MDVC,
                    U_BPP_MDFN = data.U_BPP_MDFN,
                    U_BPP_MDFC = data.U_BPP_MDFC,

                    // EXPORTACIÓN
                    U_STR_NCONTENEDOR = data.U_STR_NCONTENEDOR,
                    U_STR_NPRESCINTO = data.U_STR_NPRESCINTO,
                    U_FIB_NPRESCINTO2 = data.U_FIB_NPRESCINTO2,
                    U_FIB_NPRESCINTO3 = data.U_FIB_NPRESCINTO3,
                    U_FIB_NPRESCINTO4 = data.U_FIB_NPRESCINTO4,

                    U_NroOrden = data.U_NroOrden,
                    U_FIB_NBULTOS = data.U_FIB_NBULTOS,
                    U_FIB_KG = data.U_FIB_KG,
                };


                iTextSharp.text.Document doc = new iTextSharp.text.Document();
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                doc.SetMargins(10f, 10f, 300f, 280f);
                MemoryStream ms = new MemoryStream();
                iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                // Our custom Header and Footer is done using Event Handler
                var pageEventHelper = new PageEventHelperDeliveryNotesExport();
                write.PageEvent = pageEventHelper;

                // Colocamos la fuente que deseamos que tenga el documento
                iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
                iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                iTextSharp.text.Font parrafoNegrita = new iTextSharp.text.Font(helvetica, 7f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

                // Define the page header
                pageEventHelper.Header = header;
                pageEventHelper.Footer = footer;

                doc.Open();


                //============================
                //TABLA: DETALLE
                var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 3f, 22f, 7f, 10f, 58f }) { WidthPercentage = 100 };
                var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                for (int i = 0; i < data.Lines.Count; i++)
                {
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase((i + 1).ToString(), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].ItemCode, parrafoNormal)) { BorderWidth = 0, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].UnitMsr.ToUpper(), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 4 };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Quantity.ToString("N2"), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Dscription, parrafoNormal)) { BorderWidth = 0, PaddingBottom = 4 };
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
                resultTransaccion.data = file;
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

    public class HeaderDeliveryNotesNational
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
        public string Phone1 { get; set; }


        /// <summary>
        /// LOGÍSTICA
        /// </summary>
        public string PuntoPartida { get; set; }
        public string PuntoLlegada { get; set; }


        /// <summary>
        /// SALES EMPLOYEE
        /// </summary>        
        public string SlpName { get; set; }
        public string U_OrdenCompra { get; set; }
    }

    public class FooterDeliveryNotesNational
    {
        public string U_BPP_MDNT { get; set; }
        public string U_BPP_MDDT { get; set; }
        public string U_BPP_MDRT { get; set; }


        public string U_FIB_RUC_TRANS2 { get; set; }
        public string U_BPP_MDVC { get; set; }
        public string U_BPP_MDFN { get; set; }
        public string U_BPP_MDFC { get; set; }


        public string UserName { get; set; }
        public string DocNum { get; set; }
        public string DocTime { get; set; }

        public string U_NroOrden { get; set; }
        public decimal U_FIB_NBULTOS { get; set; }
        public decimal U_FIB_KG { get; set; }
    }

    public class PageEventHelperDeliveryNotesNational : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate headerTemplate, footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;

        #region Properties
        public HeaderDeliveryNotesNational Header { get; set; }
        public FooterDeliveryNotesNational Footer { get; set; }
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


            /*
             ================================================
             TABLA 1: HEADER - DATOS DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DATOS DE CLIENTE >>>

            var tblCliente = new iTextSharp.text.pdf.PdfPTable(new float[] { 12f, 1f, 56f, 10f, 1f, 20f});
            tblCliente.TotalWidth = 575;

            // Fila 1
            var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Fecha", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocDate, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Fecha traslado", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocDueDate, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Cliente", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.CardName, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5, Colspan = 4 };
            tblCliente.AddCell(c1);

            // Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Telefono", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.Phone1, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);

            // Fila 4
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("R.U.C.", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.LicTradNum, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);

            // Fila 5
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Dirección", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5, Colspan = 4 };
            tblCliente.AddCell(c1);

            // Fila 6
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Factura", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);

            // Fila 7
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Domicilio de partida", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.PuntoPartida, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5, Colspan = 4 };
            tblCliente.AddCell(c1);

            // Fila 8
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Domicilio de Llegada", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.PuntoLlegada, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5, Colspan = 4 };
            tblCliente.AddCell(c1);

            // Fila 9
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Asesor Comercial", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.SlpName, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("N° Orden Compra", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.U_OrdenCompra, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5 };
            tblCliente.AddCell(c1);

            // Fila 10
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5, Colspan = 6 };
            tblCliente.AddCell(c1);

            // Fila 11
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Les remitimos en buenas condiciones lo siguiente:", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 5, Colspan = 6 };
            tblCliente.AddCell(c1);

            // Ubicación de la tabla de la cabecera hacía la derecha en la página
            tblCliente.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetTop(125), cb);

            #endregion



            /*
             ================================================
             TABLA 4: HEADER - DETALLE DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DETALLE DE LA ORDEN >>>

            float startX = pageSize.GetLeft(10);
            float startY = pageSize.GetTop(285);

            var tblDetail = new iTextSharp.text.pdf.PdfPTable(new float[] { 3f, 22f, 7f, 10f, 58f });
            tblDetail.TotalWidth = 575;
            tblDetail.LockedWidth = true;

            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("#", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Código", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("UM", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Cantidad", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Descripcion", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);

            tblDetail.WriteSelectedRows(0, -1, startX, startY, cb);

            #endregion
        }
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnEndPage(writer, doc);
            iTextSharp.text.Rectangle pageSize = doc.PageSize;
            iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoNegrita = new iTextSharp.text.Font(helvetica, 7f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);


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
                TABLA 1: FOOTER - TRANSPORTISTA
            ================================================
            */
            var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 15f, 1f, 25f, 7f, 1f, 25f, 5f, 1f, 20f }) { TotalWidth = 575, LockedWidth = true };

            // Fila 1
            var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("N° pedido", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_NroOrden, parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("N° bultos", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_FIB_NBULTOS.ToString("N2"), parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Peso", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_FIB_KG.ToString("N2"), parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Emp. de Trasporte", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDNT, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5, Colspan = 7 };
            tbl.AddCell(c1);

            // Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("R.U.C.", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDRT, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5, Colspan = 7 };
            tbl.AddCell(c1);

            // Fila 4
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Dirección", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDDT, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5, Colspan = 7 };
            tbl.AddCell(c1);

            // Fila 5
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Trasportista", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_FIB_RUC_TRANS2, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5, Colspan = 7 };
            tbl.AddCell(c1);

            // Fila 6
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Placa", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDVC, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5, Colspan = 7 };
            tbl.AddCell(c1);

            // Fila 7
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Conductor", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDFN, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5, Colspan = 7 };
            tbl.AddCell(c1);

            // Fila 8
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Lic. de Conducir", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDFC, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5, Colspan = 7 };
            tbl.AddCell(c1);

            // ======================================================
            // DIBUJAR TABLA
            // ======================================================
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(280), cb);


            /*
            ================================================
                TABLA 1: FOOTER - EMPELADO
            ================================================
            */
            tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 100f }) { TotalWidth = 575, LockedWidth = true };

            // Fila 1
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.UserName, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.DocNum, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);

            // Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.DocTime, parrafoNormal)) { BorderWidth = 0, PaddingTop = 5, PaddingBottom = 5 };
            tbl.AddCell(c1);

            // ======================================================
            // DIBUJAR TABLA
            // ======================================================
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(100), cb);
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


    #region <<< EXPORT >>>

    public class HeaderDeliveryNotesExport
    {
        public string PrintHeadr { get; set; }
        public string DocNum { get; set; }
        public string DocDueDate { get; set; }
        public string DocTime { get; set; }


        /// <summary>
        /// SOCIO DE NEGOCIOS
        /// </summary>
        public string U_DestGuiaInter { get; set; }
        public string U_RUCDestInter { get; set; }
        public string U_DireccDestInter { get; set; }


        /// <summary>
        /// LOGÍSTICA
        /// </summary>
        public string PuntoPartida { get; set; }
    }

    public class FooterDeliveryNotesExport
    {
        public string UserName { get; set; }
        public string DocNum { get; set; }
        public string DocTime { get; set; }

        public string U_BPP_MDNT { get; set; }
        public string U_BPP_MDDT { get; set; }
        public string U_BPP_MDRT { get; set; }

        public string U_BPP_MDVC { get; set; }
        public string U_BPP_MDFN { get; set; }
        public string U_BPP_MDFC { get; set; }

        public string U_STR_NCONTENEDOR { get; set; }
        public string U_STR_NPRESCINTO { get; set; }
        public string U_FIB_NPRESCINTO2 { get; set; }
        public string U_FIB_NPRESCINTO3 { get; set; }
        public string U_FIB_NPRESCINTO4 { get; set; }

        public string U_NroOrden { get; set; }
        public decimal U_FIB_NBULTOS { get; set; }
        public decimal U_FIB_KG { get; set; }
    }

    public class PageEventHelperDeliveryNotesExport : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate headerTemplate, footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;

        #region Properties
        public HeaderDeliveryNotesExport Header { get; set; }
        public FooterDeliveryNotesExport Footer { get; set; }
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


            /*
             ================================================
             TABLA 1: HEADER - DATOS DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DATOS DE CLIENTE >>>

            var tblCliente = new iTextSharp.text.pdf.PdfPTable(new float[] { 12f, 1f, 87f });
            tblCliente.TotalWidth = 575;

            // Fila 1
            var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Fecha", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.DocDueDate, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Destinatario", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.U_DestGuiaInter, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);

            // Fila 4
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("R.U.C.", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.U_RUCDestInter, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);

            // Fila 5
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Dirección", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.U_DireccDestInter, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);

            // Fila 6
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Factura", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);

            // Fila 7
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Domicilio de partida", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.PuntoPartida, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);

            // Fila 8
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Domicilio de Llegada", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.U_DireccDestInter, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tblCliente.AddCell(c1);

            // Fila 10
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 3 };
            tblCliente.AddCell(c1);

            // Fila 11
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Les remitimos en buenas condiciones lo siguiente:", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 3 };
            tblCliente.AddCell(c1);

            // Ubicación de la tabla de la cabecera hacía la derecha en la página
            tblCliente.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetTop(160), cb);

            #endregion



            /*
             ================================================
             TABLA 4: HEADER - DETALLE DE LA ORDEN
            ================================================
            */
            #region <<< TABLA DE DETALLE DE LA ORDEN >>>

            float startX = pageSize.GetLeft(10);
            float startY = pageSize.GetTop(285);

            var tblDetail = new iTextSharp.text.pdf.PdfPTable(new float[] { 3f, 22f, 7f, 10f, 58f });
            tblDetail.TotalWidth = 575;
            tblDetail.LockedWidth = true;

            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("#", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Código", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("UM", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Cantidad", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Descripcion", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
            tblDetail.AddCell(c1);

            tblDetail.WriteSelectedRows(0, -1, startX, startY, cb);

            #endregion
        }
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        {
            base.OnEndPage(writer, doc);
            iTextSharp.text.Rectangle pageSize = doc.PageSize;
            iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoNegrita = new iTextSharp.text.Font(helvetica, 7f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

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
                TABLA 1: FOOTER - TRANSPORTISTA
            ================================================
            */
            var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 10f, 1f, 30f, 7f, 1f, 15f, 7f, 1f, 27f }) { TotalWidth = 575, LockedWidth = true };

            // Fila 1
            var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("N° pedido", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_NroOrden, parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("N° bultos", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_FIB_NBULTOS.ToString("N2"), parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Peso", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_FIB_KG.ToString("N2"), parrafoNormal)) { BorderWidth = 0, BorderWidthBottom = 1, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Trasportista", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDNT, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 7 };
            tbl.AddCell(c1);

            // Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("R.U.C.", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDRT, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 7 };
            tbl.AddCell(c1);

            // Fila 4
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Dirección", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDDT, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 7 };
            tbl.AddCell(c1);

            // Fila 5
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("N° contenedor", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_STR_NCONTENEDOR, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Prescinto 01", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_STR_NPRESCINTO, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 4 };
            tbl.AddCell(c1);

            // Fila 6
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Placa", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDVC, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Prescinto 02", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3};
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_FIB_NPRESCINTO2, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 4 };
            tbl.AddCell(c1);

            // Fila 7
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Conductor", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDFN, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Prescinto 03", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_FIB_NPRESCINTO3, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 4 };
            tbl.AddCell(c1);

            // Fila 8
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Lic. de Conducir", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_BPP_MDFC, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3};
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Prescinto 04", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.U_FIB_NPRESCINTO4, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 4 };
            tbl.AddCell(c1);

            // ======================================================
            // DIBUJAR TABLA
            // ======================================================
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(280), cb);


            /*
            ================================================
                TABLA 1: FOOTER - EMPELADO
            ================================================
            */
            tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 100f }) { TotalWidth = 575, LockedWidth = true };

            // Fila 1
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.UserName, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);

            // Fila 2
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.DocNum, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);

            // Fila 3
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Footer.DocTime, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
            tbl.AddCell(c1);

            // ======================================================
            // DIBUJAR TABLA
            // ======================================================
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(100), cb);
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

