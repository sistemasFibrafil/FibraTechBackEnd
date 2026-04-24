using System;
using SAPbobsCOM;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Sales.Invoices.Create;
using Net.Business.Entities.SAPBusinessOne.Sales.Invoices.Update;
using Net.Business.Entities.SAPBusinessOne.Sales.Invoices.Cancel;
namespace Net.Data.SAPBusinessOne
{
    public class InvoicesRepository : RepositoryBase<InvoicesEntity>, IInvoicesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        public InvoicesRepository(IConnectionSQL context, IConfiguration configuration, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }


        #region <<< CONSULTAS >>>

        public async Task<ResultadoTransaccionResponse<InvoicesOpenQueryEntity>> GetListOpen()
        {
            var resultTransaccion = new ResultadoTransaccionResponse<InvoicesOpenQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.Invoices
                .Where(n => n.isIns == "Y" && n.DocType == "I" && n.InvntSttus == "O" && n.U_FIB_DocStPkg == "O" && n.U_FIB_IsPkg == "Y")
                .Select(s => new InvoicesOpenQueryEntity
                {
                    DocEntry = s.DocEntry,
                    DocNum = s.DocNum,
                    WhsCode = s.Lines
                              .Where(l => l.OpenQty > 0)
                              .OrderBy(l => l.LineNum)          // ✅ ordenar por LineNum
                              .Select(l => l.WhsCode)           // ✅ luego tomar WhsCode
                              .FirstOrDefault()                 // ✅ primera línea
                })
                .OrderByDescending(n => n.DocEntry)
                .ToListAsync();

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
        public async Task<ResultadoTransaccionResponse<InvoicesQueryEntity>> GetListByFilter(InvoicesFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<InvoicesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.Invoices
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

                // FILTRO DE ESTATUS DE DOCUMENTO
                if (!string.IsNullOrWhiteSpace(value.DocStatus))
                {
                    var docStatus = value.DocStatus.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(x => docStatus.Contains(x.DocStatus));
                }

                // FILTRO DE SUBTIPO DE DOCUMENTO
                if (!string.IsNullOrWhiteSpace(value.DocSubType))
                {
                    var DocSubType = value.DocSubType.Trim();
                    query = query.Where(x => x.DocSubType == DocSubType);
                }

                // FILTRO DE FACTURAS DE RESERVA
                if (!string.IsNullOrWhiteSpace(value.isIns))
                {
                    var isIns = value.isIns.Trim();
                    query = query.Where(x => x.isIns == isIns);
                }

                var list = await query
                .Select(n => new InvoicesQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    Canceled = n.CANCELED,
                    DocStatus = n.DocStatus,
                    InvntSttus = n.InvntSttus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,

                    U_BPP_MDSD = n.U_BPP_MDSD,
                    U_BPP_MDCD = n.U_BPP_MDCD,

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
        public async Task<ResultadoTransaccionResponse<InvoicesQueryEntity>> GetByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<InvoicesQueryEntity>
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


                var data = await _db.Invoices
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new InvoicesQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    ObjType = n.ObjType,
                    DocType = n.DocType,
                    Canceled = n.CANCELED,
                    DocStatus = n.DocStatus,
                    InvntSttus = n.InvntSttus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,

                    U_BPP_MDTD = n.U_BPP_MDTD,
                    U_BPP_MDSD = n.U_BPP_MDSD,
                    U_BPP_MDCD = n.U_BPP_MDCD,

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

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines.Select(s => new Invoices1QueryEntity
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

                        U_FIB_OpQtyPkg = s.U_FIB_OpQtyPkg,
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
        
        #endregion


        #region <<< OPERACIONES >>>
        public async Task<ResultadoTransaccionResponse<InvoicesEntity>> SetCreate(InvoicesCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<InvoicesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            Documents invoices = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Empieza la transacción en SAP
                    if (!company.InTransaction) company.StartTransaction();


                    // Se crea el objeto de la factura de venta
                    invoices = company.GetBusinessObject(BoObjectTypes.oInvoices);


                    #region <<< CABECERA >>>

                    invoices.DocDate = value.DocDate;
                    invoices.DocDueDate = value.DocDueDate;
                    invoices.TaxDate = value.TaxDate;
                    invoices.DocObjectCode = BoObjectTypes.oInvoices;

                    invoices.ReserveInvoice = value.ReserveInvoice switch
                    {
                        "Y" => BoYesNoEnum.tYES,
                        "N" => BoYesNoEnum.tNO,
                        _ => throw new ArgumentException($"ReserveInvoice inválido para SAP Business One: '{value.ReserveInvoice}'. Se esperaba 'Y' o 'N'."),
                    };

                    invoices.DocType = value.DocType switch
                    {
                        "I" => BoDocumentTypes.dDocument_Items,
                        "S" => BoDocumentTypes.dDocument_Service,
                        _ => throw new ArgumentException($"DocType inválido para SAP Business One: '{value.DocType}'. Se esperaba 'I' (Artículo) o 'S' (Servicio)."),
                    };

                    // ===========================================================================================
                    // SUNAT
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_BPP_MDTD").Value = value.U_BPP_MDTD;
                    invoices.UserFields.Fields.Item("U_BPP_MDSD").Value = value.U_BPP_MDSD;
                    invoices.UserFields.Fields.Item("U_BPP_MDCD").Value = value.U_BPP_MDCD;

                    // ===========================================================================================
                    // PICKING
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_FIB_IsPkg").Value = value.U_FIB_IsPkg;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    invoices.CardCode = value.CardCode;
                    invoices.CardName = value.CardName;
                    invoices.ContactPersonCode = value.CntctCode;
                    invoices.NumAtCard = value.NumAtCard;
                    invoices.DocCurrency = value.DocCur;
                    invoices.DocRate = value.DocRate;

                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    invoices.PayToCode = value.PayToCode;
                    invoices.Address = value.Address;
                    invoices.ShipToCode = value.ShipToCode;
                    invoices.Address2 = value.Address2;

                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================
                    invoices.GroupNumber = value.GroupNum;

                    // ===========================================================================================
                    // AGENCIA
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                    invoices.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    invoices.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    invoices.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                    invoices.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                    invoices.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                    invoices.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                    invoices.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                    invoices.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;

                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    invoices.SalesPersonCode = value.SlpCode;
                    invoices.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                    invoices.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    invoices.Comments = value.Comments;

                    // ===========================================================================================
                    // TOTALES
                    // ===========================================================================================
                    invoices.DiscountPercent = value.DiscPrcnt;
                    invoices.DocTotal = value.DocTotal;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                    #endregion


                    #region <<< DETALLE >>>

                    bool isItem = value.DocType == "I";
                    bool isService = value.DocType == "S";

                    foreach (var line in value.Lines)
                    {
                        if (line.BaseEntry != 0)
                        {
                            invoices.Lines.BaseEntry = (int)line.BaseEntry;
                            invoices.Lines.BaseLine = (int)line.BaseLine;
                            invoices.Lines.BaseType = (int)line.BaseType;
                        }

                        if (isItem)
                        {
                            invoices.Lines.ItemCode = line.ItemCode;
                            invoices.Lines.WarehouseCode = line.WhsCode;
                            invoices.Lines.Quantity = line.Quantity;
                        }

                        if (isService)
                        {
                            invoices.Lines.AccountCode = line.AcctCode;
                        }

                        invoices.Lines.ItemDescription = line.Dscription;
                        invoices.Lines.Currency = line.Currency;
                        invoices.Lines.UnitPrice = line.PriceBefDi;
                        invoices.Lines.DiscountPercent = line.DiscPrcnt;
                        invoices.Lines.Price = line.Price;

                        invoices.Lines.TaxCode = line.TaxCode;
                        invoices.Lines.LineTotal = line.LineTotal;

                        // UDFs
                        invoices.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                        invoices.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        invoices.Lines.Add();
                    }

                    #endregion


                    var reg = invoices.Add();


                    if (reg != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }


                    // Confirma la transacción
                    if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_Commit);


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "La factura de venta registrada con éxito.";
                }
                catch (Exception ex)
                {
                    if (company != null && company.Connected)
                    {
                        if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }

                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(invoices);
                }

                return resultTransaccion;
            });
        }
        public async Task<ResultadoTransaccionResponse<InvoicesEntity>> SetUpdate(InvoicesUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<InvoicesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Documents invoices = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    invoices = company.GetBusinessObject(BoObjectTypes.oInvoices);


                    if (!invoices.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la factura de reserva.");
                    }


                    #region <<< CABECERA >>>

                    if (invoices.DocumentStatus == BoStatus.bost_Open && (BoApprovalRequestStatusEnum)invoices.AuthorizationStatus != BoApprovalRequestStatusEnum.arsApproved)
                    {
                        invoices.DocDueDate = value.DocDueDate;
                    }
                    invoices.DocObjectCode = BoObjectTypes.oInvoices;

                    invoices.ReserveInvoice = value.ReserveInvoice switch
                    {
                        "Y" => BoYesNoEnum.tYES,
                        "N" => BoYesNoEnum.tNO,
                        _ => throw new ArgumentException($"ReserveInvoice inválido para SAP Business One: '{value.ReserveInvoice}'. Se esperaba 'Y' o 'N'."),
                    };

                    // ===========================================================================================
                    // SUNAT
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_BPP_MDTD").Value = value.U_BPP_MDTD;
                    invoices.UserFields.Fields.Item("U_BPP_MDSD").Value = value.U_BPP_MDSD;
                    invoices.UserFields.Fields.Item("U_BPP_MDCD").Value = value.U_BPP_MDCD;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================                    
                    invoices.CardCode = value.CardCode;
                    invoices.ContactPersonCode = value.CntctCode;
                    invoices.NumAtCard = value.NumAtCard;
                    invoices.DocCurrency = value.DocCur;
                    invoices.DocRate = value.DocRate;

                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    invoices.PayToCode = value.PayToCode;
                    invoices.Address = value.Address;
                    invoices.ShipToCode = value.ShipToCode;
                    invoices.Address2 = value.Address2;

                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================
                    invoices.GroupNumber = value.GroupNum;

                    // ===========================================================================================
                    // AGENCIA
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                    invoices.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    invoices.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    invoices.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                    invoices.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;
                    
                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                    invoices.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                    invoices.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                    invoices.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                    invoices.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;
                    
                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;
                    
                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    invoices.SalesPersonCode = value.SlpCode;
                    invoices.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                    invoices.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    invoices.Comments = value.Comments;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    invoices.UserFields.Fields.Item("U_UsrUpdate").Value = value.U_UsrUpdate;

                    #endregion
                    

                    var reg = invoices.Update();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "La factura de reserva actualizada con éxito.";
                    }
                    else
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(invoices);
                }

                return resultTransaccion;
            });
        }
        public async Task<ResultadoTransaccionResponse<InvoicesEntity>> SetCancel(InvoicesCancelEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<InvoicesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Recordset rs = null;
            Company company = null;
            Documents invoices = null;
            Documents invoicesCancel = null;
            Documents invoicesUpdate = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Iniciar transacción
                    if (!company.InTransaction)
                        company.StartTransaction();


                    invoices = company.GetBusinessObject(BoObjectTypes.oInvoices);


                    #region <<< CANCELAR >>>

                    if (!invoices.GetByKey(value.DocEntry))
                        throw new Exception("No existe la factura.");

                    invoicesCancel = invoices.CreateCancellationDocument();

                    if (invoicesCancel == null)
                        throw new Exception("No se pudo crear el documento de cancelación.");

                    int regCancel = invoicesCancel.Add();

                    if (regCancel != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                    }

                    #endregion


                    #region <<< ACTUALIZAR UDF CON SQL >>>

                    // Obtener el código de tipo de documento de la entrega
                    var u_BPP_TDTD = _db.TipoDocumentoSunat
                   .Where(x => x.U_FIB_FVAN == "Y")
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
                        UPDATE OINV
                        SET 
                            U_BPP_MDTD = '{documentNumberingSeriesSunat.U_BPP_NDTD}',
                            U_BPP_MDSD = '{documentNumberingSeriesSunat.U_BPP_NDSD}',
                            U_BPP_MDCD = '{documentNumberingSeriesSunat.U_BPP_NDCD}',
                            U_UsrCreate = '{value.U_UsrCreate}'
                        WHERE DocEntry = {docEntry}
                    ");

                    #endregion


                    #region <<< ACTUALIZAR >>>

                    invoicesUpdate = company.GetBusinessObject(BoObjectTypes.oInvoices);

                    if (!invoicesUpdate.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la factura cancelada.");
                    }

                    invoicesUpdate.Indicator = "ZA";
                    invoicesUpdate.UserFields.Fields.Item("U_UsrCancel").Value = value.U_UsrCancel;

                    var regUpdate = invoicesUpdate.Update();

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
                    resultTransaccion.ResultadoDescripcion = "Factura cancelada con éxito.";
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
                    _companyProviderSap.LiberarObjetosCOM(rs, invoices, invoicesCancel, invoicesUpdate);
                }

                return resultTransaccion;
            });
        }

        #endregion
    }
}
