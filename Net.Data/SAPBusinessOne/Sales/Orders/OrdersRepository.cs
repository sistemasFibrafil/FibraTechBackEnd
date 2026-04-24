using System;
using System.IO;
using SAPbobsCOM;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Data.SqlClient;
using Net.Business.Entities;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Sales.Orders.Close;
using Net.Business.Entities.SAPBusinessOne.Sales.Orders.Create;
using Net.Business.Entities.SAPBusinessOne.Sales.Orders.Update;
using Net.Business.Entities.SAPBusinessOne.Common.Attachments2.Query;
namespace Net.Data.SAPBusinessOne
{
    public class OrdersRepository : RepositoryBase<OrdersEntity>, IOrdersRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_SEGUIMIENTO_BY_FILTER = DB_ESQUEMA + "VEN_GetListOrdenVentaSeguimientoByFilter";
        const string SP_GET_LIST_SEGUIMIENTO_DETALLADO_DIRECCION_FISCAL_BY_FILTER = DB_ESQUEMA + "VEN_GetListOVSeguimientoDetalladoDireccionFiscalByFilter";
        const string SP_GET_LIST_SEGUIMIENTO_DETALLADO_DIRECCION_DESPACHO_BY_FILTER = DB_ESQUEMA + "VEN_GetListOVSeguimientoDetalladoDireccionDespachoByFilter";
        const string SP_GET_LIST_PENDIENTE_STOCK_ALMACEN_PRODUCCION_BY_FECHA = DB_ESQUEMA + "VEN_GetListOrdenVentaPendienteStockAlmacenProduccionByFecha";
        const string SP_GET_LIST_PROGRAMACION_BY_FECHA = DB_ESQUEMA + "VEN_GetListOrdenVentaProgramacionByFecha";
        const string SP_GET_LIST_PRELIMINAR_PENDIENTE_BY_FECHA = DB_ESQUEMA + "VEN_GetListOrdenVentaPreliminarPendienteByFecha";

        const string SP_GET_LIST_SODIMAC_BY_FILTRO = DB_ESQUEMA + "VEN_GetListOrdenVentaSodimacPendienteByFiltro";
        const string SP_GET_SODIMAC_BY_ID = DB_ESQUEMA + "VEN_GetOrdenVentaSodimacPendienteById";


        public OrdersRepository(IConnectionSQL context, IConfiguration configuration, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }


        #region <<< CONSULTAS >>>

        public async Task<ResultadoTransaccionResponse<OrdersOpenQueryEntity>> GetListOpen()
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersOpenQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.Orders
                .Where(n => n.DocType == "I" && n.DocStatus == "O" && n.U_FIB_DocStPkg == "O" && n.U_FIB_IsPkg == "Y")
                .Select(s => new OrdersOpenQueryEntity
                {
                    DocEntry = s.DocEntry,
                    DocNum = s.DocNum,
                    WhsCode = s.Lines
                              .Where(l => l.LineStatus == "O")
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
        public async Task<ResultadoTransaccionResponse<OrdersQueryEntity>> GetListByFilter(OrdersFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.Orders
                .AsNoTracking()
                .Where(x => x.DocDate >= value.StartDate && x.DocDate <= value.EndDate);

                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.DocNum.ToString(), $"%{filter}%") ||
                        EF.Functions.Like(n.CardCode, $"%{filter}%") ||
                        EF.Functions.Like(n.CardName, $"%{filter}%")
                    );
                }

                if (!string.IsNullOrWhiteSpace(value.DocStatus))
                {
                    var docStatus = value.DocStatus.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(x => docStatus.Contains(x.DocStatus));
                }

                var list = await query
                .Select(n => new OrdersQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    DocType = n.DocType,
                    Canceled = n.CANCELED,
                    DocStatus = n.DocStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,

                    U_FIB_IsPkg = n.U_FIB_IsPkg,

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
        public async Task<ResultadoTransaccionResponse<OrdersQueryEntity>> GetByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                string[]  warehouse = await _db.Warehouses
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


                var data = await _db.Orders
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new OrdersQueryEntity
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
                    Lines = n.Lines.Select(s => new Orders1QueryEntity
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
        public async Task<ResultadoTransaccionResponse<OrdersQueryEntity>> GetToCopy(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersQueryEntity>
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


                var data = await _db.Orders
                .Where(n => n.DocStatus == "O" && n.DocEntry == docEntry)
                .Select(n => new OrdersQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    ObjType = n.ObjType,
                    DocType = n.DocType,
                    DocStatus = n.DocStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    U_FIB_DocStPkg = n.U_FIB_DocStPkg,
                    U_FIB_IsPkg = n.U_FIB_IsPkg,

                    // Socio de negocios
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

                    // Finanzas
                    GroupNum = n.GroupNum,

                    // Logística
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
                    Address = n.Address,
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
                    Address2 = n.Address2,

                    // Agencia
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
                    U_BPP_MDDT = n.U_BPP_MDDT,

                    // Exportación
                    U_TipoFlete = n.U_TipoFlete,
                    U_ValorFlete = n.U_ValorFlete,
                    U_FIB_TFLETE = n.U_FIB_TFLETE,
                    U_FIB_IMPSEG = n.U_FIB_IMPSEG,
                    U_FIB_PUERTO = n.U_FIB_PUERTO,

                    // Otros
                    U_STR_TVENTA = n.U_STR_TVENTA,

                    // Empleado
                    SlpCode = n.SlpCode,
                    U_NroOrden = n.U_NroOrden,
                    U_OrdenCompra = n.U_OrdenCompra,
                    Comments = n.Comments,

                    // Totales
                    SubTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal - n.VatSum + n.DiscSum : n.DocTotalSy - n.VatSumSy + n.DiscSumSy,
                    DiscPrcnt = n.DiscPrcnt ?? 0,
                    DiscSum = adminInfo.MaMainCurncy == n.DocCur ? n.DiscSum : n.DiscSumSy,
                    VatSum = adminInfo.MaMainCurncy == n.DocCur ? n.VatSum : n.VatSumSy,
                    DocTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal : n.DocTotalSy,

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines
                    .Where(s => s.LineStatus == "O")   // ✅ SOLO líneas abiertas
                    .Select(s => new Orders1QueryEntity
                    {
                        DocEntry = s.DocEntry,
                        LineNum = s.LineNum,
                        LineStatus = s.LineStatus,
                        ObjType = s.ObjType,

                        BaseEntry = s.DocEntry,
                        BaseLine = s.LineNum,
                        BaseType = int.Parse(n.ObjType),

                        ItemCode = s.ItemCode,
                        Dscription = s.Dscription,
                        AcctCode = s.AcctCode,
                        FormatCode = s.ChartOfAccounts != null ? s.ChartOfAccounts.Segment_0 + "-" + s.ChartOfAccounts.Segment_1 + "-" + s.ChartOfAccounts.Segment_2 : "",
                        AcctName = s.ChartOfAccounts != null ? s.ChartOfAccounts.AcctName : "",
                        WhsCode = s.WhsCode,
                        UnitMsr = s.UnitMsr,
                        Quantity = s.OpenQty,
                        OpenQty = s.OpenQty,
                        //OnHand = s.Item.OnHand,
                        OnHand = _db.ItemWarehouseInfo
                                 .Where(w => w.ItemCode == s.Item.ItemCode && warehouse.Contains(w.WhsCode))
                                 .Sum(w => (decimal?)w.OnHand) ?? 0,

                        Currency = s.Currency,
                        PriceBefDi = s.PriceBefDi,
                        DiscPrcnt = s.DiscPrcnt ?? 0,
                        Price = s.Price,
                        TaxCode = s.TaxCode,

                        VatPrcnt = s.VatPrcnt ?? 0,
                        VatSum = 0,
                        LineTotal = 0,

                        // UDFs
                        U_FIB_LinStPkg = s.U_FIB_LinStPkg ?? "O",
                        U_FIB_OpQtyPkg = s.U_FIB_OpQtyPkg ?? (n.DocType == "I" ? s.OpenQty : 0),
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
        public async Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListSeguimientoByFilter(OrdersSeguimientoFindEntity value)
        {
            var response = new List<OrdersFechaQueryEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersFechaQueryEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_SEGUIMIENTO_BY_FILTER, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@StartDate", value.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", value.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@BusinessPartnerGroups", value.BusinessPartnerGroup));
                        cmd.Parameters.Add(new SqlParameter("@SalesEmployee", value.SalesEmployee));
                        cmd.Parameters.Add(new SqlParameter("@DocumentType", value.DocumentType));
                        cmd.Parameters.Add(new SqlParameter("@Status", value.Status));
                        cmd.Parameters.Add(new SqlParameter("@Customer", value.Customer));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdersFechaQueryEntity>)context.ConvertTo<OrdersFechaQueryEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultTransaccion.dataList = response;
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetSeguimientoByFilterExcel(OrdersSeguimientoFindEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Ventas" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("País", CellValues.String),
                    ExportToExcel.ConstructCell("Departamento", CellValues.String),
                    ExportToExcel.ConstructCell("Provincia", CellValues.String),
                    ExportToExcel.ConstructCell("Ciudad", CellValues.String),
                    ExportToExcel.ConstructCell("Tipo Documento", CellValues.String),
                    ExportToExcel.ConstructCell("Número Documento", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Contabilización", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Emisión", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Entrega", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Creación", CellValues.String),
                    ExportToExcel.ConstructCell("Estado", CellValues.String),
                    ExportToExcel.ConstructCell("Moneda", CellValues.String),
                    ExportToExcel.ConstructCell("TC", CellValues.String),
                    ExportToExcel.ConstructCell("Total Documento SOL", CellValues.String),
                    ExportToExcel.ConstructCell("Total Documento USD", CellValues.String),
                    ExportToExcel.ConstructCell("Total Documento SYS", CellValues.String),
                    ExportToExcel.ConstructCell("Código Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Condición de Pago", CellValues.String),
                    ExportToExcel.ConstructCell("Código División", CellValues.String),
                    ExportToExcel.ConstructCell("División", CellValues.String),
                    ExportToExcel.ConstructCell("Código Sector", CellValues.String),
                    ExportToExcel.ConstructCell("Sector", CellValues.String),
                    ExportToExcel.ConstructCell("Días Atraso", CellValues.String),
                    ExportToExcel.ConstructCell("Días Vencimiento", CellValues.String),
                    ExportToExcel.ConstructCell("Origen Cliente", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListSeguimientoByFilter(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                        ExportToExcel.ConstructCell(item.PaisDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.DepartamentoDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.ProvinciaDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.CiudadDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomTipDocumento.ToString(), CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroDocumento.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.TaxDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.DocDueDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.CreateDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.NomStatus, CellValues.String),
                        ExportToExcel.ConstructCell(item.DocCur, CellValues.String),
                        ExportToExcel.ConstructCell(item.Rate.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocTotal.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocTotalFC.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocTotalSy.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SlpCode.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SlpName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomConPago, CellValues.String),
                        ExportToExcel.ConstructCell(item.IdDivision, CellValues.String),
                        ExportToExcel.ConstructCell(item.Division, CellValues.String),
                        ExportToExcel.ConstructCell(item.IdSector, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sector, CellValues.String),
                        ExportToExcel.ConstructCell(item.DiasAtraso.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DiasVenc, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomOriCliente, CellValues.String));
                        sheetData.Append(row);
                    }

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                resultTransaccion.data = ms;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListSeguimientoDetalladoDireccionFiscalByFilter(OrdersSeguimientoFindEntity value)
        {
            var response = new List<OrdersFechaQueryEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersFechaQueryEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_SEGUIMIENTO_DETALLADO_DIRECCION_FISCAL_BY_FILTER, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@StartDate", value.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", value.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@BusinessPartnerGroups", value.BusinessPartnerGroup));
                        cmd.Parameters.Add(new SqlParameter("@SalesEmployee", value.SalesEmployee));
                        cmd.Parameters.Add(new SqlParameter("@DocumentType", value.DocumentType));
                        cmd.Parameters.Add(new SqlParameter("@Status", value.Status));
                        cmd.Parameters.Add(new SqlParameter("@Customer", value.Customer));
                        cmd.Parameters.Add(new SqlParameter("@Item", value.Item));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdersFechaQueryEntity>)context.ConvertTo<OrdersFechaQueryEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultTransaccion.dataList = response;
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetSeguimientoDetalladoDireccionFiscalByFilterExcel(OrdersSeguimientoFindEntity value)
        {
            var ms = new MemoryStream();
            var resultadoTransaccion = new ResultadoTransaccionResponse<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Ventas" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("País", CellValues.String),
                    ExportToExcel.ConstructCell("Departamento", CellValues.String),
                    ExportToExcel.ConstructCell("Provincia", CellValues.String),
                    ExportToExcel.ConstructCell("Ciudad", CellValues.String),
                    ExportToExcel.ConstructCell("Tipo Documento", CellValues.String),
                    ExportToExcel.ConstructCell("Número Documento", CellValues.String),
                    ExportToExcel.ConstructCell("Número Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Número Órden Compra", CellValues.String),
                    ExportToExcel.ConstructCell("Número Factura", CellValues.String),
                    ExportToExcel.ConstructCell("Número Línea", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Contabilización", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Emisión", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Entrega", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Creación", CellValues.String),
                    ExportToExcel.ConstructCell("Estado", CellValues.String),
                    ExportToExcel.ConstructCell("Código Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Código de Grupo de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Grupo de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de SubGrupo de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de SubGrupo de Artículo 2", CellValues.String),
                    ExportToExcel.ConstructCell("Medida", CellValues.String),
                    ExportToExcel.ConstructCell("Color", CellValues.String),
                    ExportToExcel.ConstructCell("Código de Alamcén", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Alamcén", CellValues.String),
                    ExportToExcel.ConstructCell("UM Compra", CellValues.String),
                    ExportToExcel.ConstructCell("UM Venta", CellValues.String),
                    ExportToExcel.ConstructCell("UM Inventario", CellValues.String),
                    ExportToExcel.ConstructCell("Stock", CellValues.String),
                    ExportToExcel.ConstructCell("Pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Solicitado", CellValues.String),
                    ExportToExcel.ConstructCell("Disponible", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad", CellValues.String),
                    ExportToExcel.ConstructCell("Rollo Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Kg Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Tonelada Pedida", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad Pendiente por Despachar", CellValues.String),
                    ExportToExcel.ConstructCell("Rollo Pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Kg Pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Tonelada Pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Importe Pendiente USD", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad Despachada", CellValues.String),
                    ExportToExcel.ConstructCell("Moneda", CellValues.String),
                    ExportToExcel.ConstructCell("TC", CellValues.String),
                    ExportToExcel.ConstructCell("Precio", CellValues.String),
                    ExportToExcel.ConstructCell("Total Línea SOL", CellValues.String),
                    ExportToExcel.ConstructCell("Total Línea USD", CellValues.String),
                    ExportToExcel.ConstructCell("Total Línea SYS", CellValues.String),
                    ExportToExcel.ConstructCell("Total Documento USD", CellValues.String),
                    ExportToExcel.ConstructCell("Código Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Condición de Pago", CellValues.String),
                    ExportToExcel.ConstructCell("Código División", CellValues.String),
                    ExportToExcel.ConstructCell("División", CellValues.String),
                    ExportToExcel.ConstructCell("Código Sector", CellValues.String),
                    ExportToExcel.ConstructCell("Sector", CellValues.String),
                    ExportToExcel.ConstructCell("Peso Promedio Kg", CellValues.String),
                    ExportToExcel.ConstructCell("Días Atraso", CellValues.String),
                    ExportToExcel.ConstructCell("Días Vencimiento", CellValues.String),
                    ExportToExcel.ConstructCell("Origen Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Sede", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListSeguimientoDetalladoDireccionFiscalByFilter(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                        ExportToExcel.ConstructCell(item.PaisDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.DepartamentoDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.ProvinciaDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.CiudadDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomTipDocumento.ToString(), CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroDocumento.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroOrdenCompra, CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroFactura == null ? null : item.NumeroFactura.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.LineNum.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.TaxDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.DocDueDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.CreateDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.NomStatus, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.CodGpoArticulo.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NomGpoArticulo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGpoArticulo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGpoArticulo2, CellValues.String),
                        ExportToExcel.ConstructCell(item.Medida, CellValues.String),
                        ExportToExcel.ConstructCell(item.Color, CellValues.String),
                        ExportToExcel.ConstructCell(item.WhsCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.WhsName, CellValues.String),
                        ExportToExcel.ConstructCell(item.BuyUnitMsr, CellValues.String),
                        ExportToExcel.ConstructCell(item.SalUnitMsr, CellValues.String),
                        ExportToExcel.ConstructCell(item.InvntryUom, CellValues.String),
                        ExportToExcel.ConstructCell(item.StockProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PendienteProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SolicitadoProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DisponibleProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Quantity.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.RolloPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.KgPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.ToneladaPedida.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.OpenQty.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.RolloPendiente.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.KgPendiente.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.ToneladaPendiente.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.LineTotEarring.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DelivrdQty.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Currency, CellValues.String),
                        ExportToExcel.ConstructCell(item.Rate.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Price.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.LineTotal.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalFrgn.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalSumSy.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocTotalSy.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SlpCode.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SlpName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomConPago, CellValues.String),
                        ExportToExcel.ConstructCell(item.IdDivision, CellValues.String),
                        ExportToExcel.ConstructCell(item.Division, CellValues.String),
                        ExportToExcel.ConstructCell(item.IdSector, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sector, CellValues.String),
                        ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DiasAtraso.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DiasVenc, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomOriCliente, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sede, CellValues.String));
                        sheetData.Append(row);
                    }

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

                resultadoTransaccion.IdRegistro = 0;
                resultadoTransaccion.ResultadoCodigo = 0;
                resultadoTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                resultadoTransaccion.data = ms;
            }
            catch (Exception ex)
            {
                resultadoTransaccion.IdRegistro = -1;
                resultadoTransaccion.ResultadoCodigo = -1;
                resultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultadoTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListSeguimientoDetalladoDireccionDespachoByFilter(OrdersSeguimientoFindEntity value)
        {
            var response = new List<OrdersFechaQueryEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersFechaQueryEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_SEGUIMIENTO_DETALLADO_DIRECCION_DESPACHO_BY_FILTER, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@StartDate", value.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", value.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@BusinessPartnerGroups", value.BusinessPartnerGroup));
                        cmd.Parameters.Add(new SqlParameter("@SalesEmployee", value.SalesEmployee));
                        cmd.Parameters.Add(new SqlParameter("@DocumentType", value.DocumentType));
                        cmd.Parameters.Add(new SqlParameter("@Status", value.Status));
                        cmd.Parameters.Add(new SqlParameter("@Customer", value.Customer));
                        cmd.Parameters.Add(new SqlParameter("@Item", value.Item));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdersFechaQueryEntity>)context.ConvertTo<OrdersFechaQueryEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultTransaccion.dataList = response;
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetSeguimientoDetalladoDireccionDespachoByFilterExcel(OrdersSeguimientoFindEntity value)
        {
            var ms = new MemoryStream();
            var resultadoTransaccion = new ResultadoTransaccionResponse<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Ventas" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("País", CellValues.String),
                    ExportToExcel.ConstructCell("Departamento", CellValues.String),
                    ExportToExcel.ConstructCell("Provincia", CellValues.String),
                    ExportToExcel.ConstructCell("Ciudad", CellValues.String),
                    ExportToExcel.ConstructCell("Ubigeo", CellValues.String),
                    ExportToExcel.ConstructCell("Tipo Documento", CellValues.String),
                    ExportToExcel.ConstructCell("Número Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Número Órden Compra", CellValues.String),
                    ExportToExcel.ConstructCell("Número Factura", CellValues.String),
                    ExportToExcel.ConstructCell("Número Línea", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Contabilización", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Emisión", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Entrega", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Creación", CellValues.String),
                    ExportToExcel.ConstructCell("Estado", CellValues.String),
                    ExportToExcel.ConstructCell("Código Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Código de Grupo de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Grupo de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de SubGrupo de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de SubGrupo de Artículo 2", CellValues.String),
                    ExportToExcel.ConstructCell("Medida", CellValues.String),
                    ExportToExcel.ConstructCell("Color", CellValues.String),
                    ExportToExcel.ConstructCell("Código de Alamcén", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Alamcén", CellValues.String),
                    ExportToExcel.ConstructCell("UM Compra", CellValues.String),
                    ExportToExcel.ConstructCell("UM Venta", CellValues.String),
                    ExportToExcel.ConstructCell("UM Inventario", CellValues.String),
                    ExportToExcel.ConstructCell("Stock", CellValues.String),
                    ExportToExcel.ConstructCell("Pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Solicitado", CellValues.String),
                    ExportToExcel.ConstructCell("Disponible", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad", CellValues.String),
                    ExportToExcel.ConstructCell("Rollo Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Kg Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Tonelada Pedida", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad Pendiente por Despachar", CellValues.String),
                    ExportToExcel.ConstructCell("Rollo Pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Kg Pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Tonelada Pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Importe Pendiente USD", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad Despachada", CellValues.String),
                    ExportToExcel.ConstructCell("Moneda", CellValues.String),
                    ExportToExcel.ConstructCell("TC", CellValues.String),
                    ExportToExcel.ConstructCell("Precio", CellValues.String),
                    ExportToExcel.ConstructCell("Total Línea SOL", CellValues.String),
                    ExportToExcel.ConstructCell("Total Línea USD", CellValues.String),
                    ExportToExcel.ConstructCell("Total Línea SYS", CellValues.String),
                    ExportToExcel.ConstructCell("Total Documento USD", CellValues.String),
                    ExportToExcel.ConstructCell("Código Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Condición de Pago", CellValues.String),
                    ExportToExcel.ConstructCell("Código División", CellValues.String),
                    ExportToExcel.ConstructCell("División", CellValues.String),
                    ExportToExcel.ConstructCell("Código Sector", CellValues.String),
                    ExportToExcel.ConstructCell("Sector", CellValues.String),
                    ExportToExcel.ConstructCell("Peso Promedio Kg", CellValues.String),
                    ExportToExcel.ConstructCell("Días Atraso", CellValues.String),
                    ExportToExcel.ConstructCell("Días Vencimiento", CellValues.String),
                    ExportToExcel.ConstructCell("Origen Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Sede", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListSeguimientoDetalladoDireccionDespachoByFilter(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                        ExportToExcel.ConstructCell(item.PaisDD, CellValues.String),
                        ExportToExcel.ConstructCell(item.DepartamentoDD, CellValues.String),
                        ExportToExcel.ConstructCell(item.ProvinciaDD, CellValues.String),
                        ExportToExcel.ConstructCell(item.CiudadDD, CellValues.String),
                        ExportToExcel.ConstructCell(item.UbigeoDD, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomTipDocumento.ToString(), CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroOrdenCompra, CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroFactura == null ? null : item.NumeroFactura.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.LineNum.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.TaxDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.DocDueDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.CreateDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.NomStatus, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.CodGpoArticulo.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NomGpoArticulo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGpoArticulo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGpoArticulo2, CellValues.String),
                        ExportToExcel.ConstructCell(item.Medida, CellValues.String),
                        ExportToExcel.ConstructCell(item.Color, CellValues.String),
                        ExportToExcel.ConstructCell(item.WhsCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.WhsName, CellValues.String),
                        ExportToExcel.ConstructCell(item.BuyUnitMsr, CellValues.String),
                        ExportToExcel.ConstructCell(item.SalUnitMsr, CellValues.String),
                        ExportToExcel.ConstructCell(item.InvntryUom, CellValues.String),
                        ExportToExcel.ConstructCell(item.StockProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PendienteProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SolicitadoProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DisponibleProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Quantity.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.RolloPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.KgPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.ToneladaPedida.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.OpenQty.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.RolloPendiente.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.KgPendiente.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.ToneladaPendiente.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.LineTotEarring.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DelivrdQty.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Currency, CellValues.String),
                        ExportToExcel.ConstructCell(item.Rate.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Price.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.LineTotal.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalFrgn.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalSumSy.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocTotalSy.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SlpCode.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SlpName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomConPago, CellValues.String),
                        ExportToExcel.ConstructCell(item.IdDivision, CellValues.String),
                        ExportToExcel.ConstructCell(item.Division, CellValues.String),
                        ExportToExcel.ConstructCell(item.IdSector, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sector, CellValues.String),
                        ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DiasAtraso.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DiasVenc, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomOriCliente, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sede, CellValues.String));
                        sheetData.Append(row);
                    }

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

                resultadoTransaccion.IdRegistro = 0;
                resultadoTransaccion.ResultadoCodigo = 0;
                resultadoTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                resultadoTransaccion.data = ms;
            }
            catch (Exception ex)
            {
                resultadoTransaccion.IdRegistro = -1;
                resultadoTransaccion.ResultadoCodigo = -1;
                resultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultadoTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListOrdenVentaPendienteStockAlmacenProduccionByFecha(FilterRequestEntity value)
        {
            var response = new List<OrdersFechaQueryEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersFechaQueryEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_PENDIENTE_STOCK_ALMACEN_PRODUCCION_BY_FECHA, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdersFechaQueryEntity>)context.ConvertTo<OrdersFechaQueryEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultTransaccion.dataList = response;
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetOrdenVentaPendienteStockAlmacenProduccionExcelByFecha(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Ventas" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("País", CellValues.String),
                    ExportToExcel.ConstructCell("Departamento", CellValues.String),
                    ExportToExcel.ConstructCell("Provincia", CellValues.String),
                    ExportToExcel.ConstructCell("Ciudad", CellValues.String),
                    ExportToExcel.ConstructCell("Tipo de Documento", CellValues.String),
                    ExportToExcel.ConstructCell("Número Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Número Factura", CellValues.String),
                    ExportToExcel.ConstructCell("Número Línea", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Contabilización", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Emisión", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Entrega", CellValues.String),
                    ExportToExcel.ConstructCell("Código Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Código de Grupo de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Grupo de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de SubGrupo de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de SubGrupo de Artículo 2", CellValues.String),
                    ExportToExcel.ConstructCell("Medida", CellValues.String),
                    ExportToExcel.ConstructCell("Color", CellValues.String),
                    ExportToExcel.ConstructCell("UM Compra", CellValues.String),
                    ExportToExcel.ConstructCell("UM Venta", CellValues.String),
                    ExportToExcel.ConstructCell("UM Inventario", CellValues.String),
                    ExportToExcel.ConstructCell("Stock", CellValues.String),
                    ExportToExcel.ConstructCell("Pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Solicitado", CellValues.String),
                    ExportToExcel.ConstructCell("Disponible", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad", CellValues.String),
                    ExportToExcel.ConstructCell("Rollo Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Kg Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Tonelada Pedida", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad Pendiente por Despachar", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad Despachada", CellValues.String),
                    ExportToExcel.ConstructCell("Moneda", CellValues.String),
                    ExportToExcel.ConstructCell("TC", CellValues.String),
                    ExportToExcel.ConstructCell("Precio", CellValues.String),
                    ExportToExcel.ConstructCell("Total Línea SOL", CellValues.String),
                    ExportToExcel.ConstructCell("Total Línea USD", CellValues.String),
                    ExportToExcel.ConstructCell("Total Línea SYS", CellValues.String),
                    ExportToExcel.ConstructCell("Total Documento USD", CellValues.String),
                    ExportToExcel.ConstructCell("Código Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Condición de Pago", CellValues.String),
                    ExportToExcel.ConstructCell("Código División", CellValues.String),
                    ExportToExcel.ConstructCell("División", CellValues.String),
                    ExportToExcel.ConstructCell("Código Sector", CellValues.String),
                    ExportToExcel.ConstructCell("Sector", CellValues.String),
                    ExportToExcel.ConstructCell("Peso Promedio Kg", CellValues.String),
                    ExportToExcel.ConstructCell("Días Atraso", CellValues.String),
                    ExportToExcel.ConstructCell("Días Vencimiento", CellValues.String),
                    ExportToExcel.ConstructCell("Origen Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Sede", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListOrdenVentaPendienteStockAlmacenProduccionByFecha(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                        ExportToExcel.ConstructCell(item.PaisDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.DepartamentoDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.ProvinciaDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.CiudadDF, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomTipDocumento.ToString(), CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroFactura == null ? null : item.NumeroFactura.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.LineNum.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.TaxDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.DocDueDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.CodGpoArticulo.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NomGpoArticulo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGpoArticulo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGpoArticulo2, CellValues.String),
                        ExportToExcel.ConstructCell(item.Medida, CellValues.String),
                        ExportToExcel.ConstructCell(item.Color, CellValues.String),
                        ExportToExcel.ConstructCell(item.BuyUnitMsr, CellValues.String),
                        ExportToExcel.ConstructCell(item.SalUnitMsr, CellValues.String),
                        ExportToExcel.ConstructCell(item.InvntryUom, CellValues.String),
                        ExportToExcel.ConstructCell(item.StockProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PendienteProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SolicitadoProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DisponibleProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Quantity.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.RolloPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.KgPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.ToneladaPedida.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.OpenQty.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DelivrdQty.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Currency, CellValues.String),
                        ExportToExcel.ConstructCell(item.Rate.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Price.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.LineTotal.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalFrgn.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalSumSy.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocTotalSy.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SlpCode.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SlpName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomConPago, CellValues.String),
                        ExportToExcel.ConstructCell(item.IdDivision, CellValues.String),
                        ExportToExcel.ConstructCell(item.Division, CellValues.String),
                        ExportToExcel.ConstructCell(item.IdSector, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sector, CellValues.String),
                        ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DiasAtraso.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DiasVenc, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomOriCliente, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sede, CellValues.String));
                        sheetData.Append(row);
                    }

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                resultTransaccion.data = ms;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListOrdenVentaProgramacionByFecha(FilterRequestEntity value)
        {
            var response = new List<OrdersFechaQueryEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersFechaQueryEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_PROGRAMACION_BY_FECHA, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdersFechaQueryEntity>)context.ConvertTo<OrdersFechaQueryEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultTransaccion.dataList = response;
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetOrdenVentaProgramacionExcelByFecha(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Ventas" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Tipo de Documento", CellValues.String),
                    ExportToExcel.ConstructCell("Número Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Número Factura", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Contabilización", CellValues.String),
                    ExportToExcel.ConstructCell("Código Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Grupo de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("UM", CellValues.String),
                    ExportToExcel.ConstructCell("Stock", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad Pendiente por Despachar", CellValues.String),
                    ExportToExcel.ConstructCell("Días de Antiguedad", CellValues.String),
                    ExportToExcel.ConstructCell("Sede", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListOrdenVentaProgramacionByFecha(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomTipDocumento.ToString(), CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroFactura == null ? null : item.NumeroFactura.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomGpoArticulo, CellValues.String),
                        ExportToExcel.ConstructCell(item.SalUnitMsr, CellValues.String),
                        ExportToExcel.ConstructCell(item.StockProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Quantity.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.OpenQty.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.RolloPendiente.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DiasAntiguedad.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Sede, CellValues.String));
                        sheetData.Append(row);
                    }

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                resultTransaccion.data = ms;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<OrdersSodimacQueryEntity>> GetListOrdenVentaSodimacPendienteByFiltro(FilterRequestEntity value)
        {
            var response = new List<OrdersSodimacQueryEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersSodimacQueryEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_SODIMAC_BY_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdersSodimacQueryEntity>)context.ConvertTo<OrdersSodimacQueryEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultTransaccion.dataList = response;
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<OrdersSodimacQueryEntity>> GetOrdenVentaSodimacPendienteById(FilterRequestEntity value)
        {
            var response = new OrdersSodimacQueryEntity();
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersSodimacQueryEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_SODIMAC_BY_ID, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", value.Id1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<OrdersSodimacQueryEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Datos obtenidos con éxito ..!";
                    resultTransaccion.data = response;
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<OrdersFechaQueryEntity>> GetListOrdenVentaPreliminarPendienteByFecha(FilterRequestEntity value)
        {
            var response = new List<OrdersFechaQueryEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersFechaQueryEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_PRELIMINAR_PENDIENTE_BY_FECHA, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@GrupoCliente", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@EmpleadoVenta", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdersFechaQueryEntity>)context.ConvertTo<OrdersFechaQueryEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultTransaccion.dataList = response;
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetListOrdenVentaPreliminarPendienteExcelByFecha(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultadoTransaccion = new ResultadoTransaccionResponse<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Orden de venta - Preliminar" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Número Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Contabilización", CellValues.String),
                    ExportToExcel.ConstructCell("Código Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Stock", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad", CellValues.String),
                    ExportToExcel.ConstructCell("Peso", CellValues.String),
                    ExportToExcel.ConstructCell("Kg Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Precio", CellValues.String),
                    ExportToExcel.ConstructCell("Importe USD", CellValues.String),
                    ExportToExcel.ConstructCell("Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Origen Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Sede", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListOrdenVentaPreliminarPendienteByFecha(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroDocumento.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.StockProduccion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Quantity.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Peso.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.KgPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Price.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalSumSy.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SlpName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomOriCliente, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sede, CellValues.String));
                        sheetData.Append(row);
                    }

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

                resultadoTransaccion.IdRegistro = 0;
                resultadoTransaccion.ResultadoCodigo = 0;
                resultadoTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                resultadoTransaccion.data = ms;
            }
            catch (Exception ex)
            {
                resultadoTransaccion.IdRegistro = -1;
                resultadoTransaccion.ResultadoCodigo = -1;
                resultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultadoTransaccion;
        }

        #endregion


        #region <<< OPERACIONES >>>

        public async Task<ResultadoTransaccionResponse<OrdersEntity>> SetCreate(OrdersCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Documents orders = null;
            Attachments2 attachments = null;
            var aplicaAprobacion = false;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    #region <<< VALIDACIÓN DE AUTORIZACIÓN >>>

                    /*
                        Se valida el límite de crédito del cliente solo si se ha asignado un grupo de clientes a dicho cliente.
                        Esta consulta es de Querymager para gestionar la autorización por límite de crédito en SAP Business One, considerando el grupo de clientes asignado al cliente.
                        Se ha creado un modelo con el usuario -->> indirectcc <<-- para gestionar indipendientemente, sino no pasa por el modelo de autorización por límte de crédito
                    */
                    if (value.GroupNum != -1)
                    {
                        var result = _db.BusinessPartners
                        .Where(x => x.CardCode == value.CardCode)
                        .Select(x => new
                        {
                            snCre = (x.CreditLine ?? 0),
                            snBal = x.GroupCode == 114 ? ((decimal?)value.DocTotal + (x.Balance ?? 0) + (x.OrdersBal ?? 0)) : null
                        })
                        .FirstOrDefault();

                        var snCre = result?.snCre ?? 0;
                        var snBal = result?.snBal ?? 0;

                        if (snBal > snCre)
                        {
                            company = _companyProviderSap.GetCompanyAuth();
                            aplicaAprobacion = true;
                        }
                    }

                    #endregion


                    // Se crea el objeto de orden de venta
                    orders = company.GetBusinessObject(BoObjectTypes.oOrders);


                    #region <<< CABECERA >>>

                    orders.DocDate = value.DocDate;
                    orders.DocDueDate = value.DocDueDate;
                    orders.TaxDate = value.TaxDate;

                    orders.DocObjectCode = BoObjectTypes.oOrders;

                    orders.DocType = value.DocType switch
                    {
                        "I" => BoDocumentTypes.dDocument_Items,
                        "S" => BoDocumentTypes.dDocument_Service,
                        _ => throw new ArgumentException($"DocType inválido para SAP Business One: '{value.DocType}'. Se esperaba 'I' (Artículo) o 'S' (Servicio)."),
                    };

                    orders.UserFields.Fields.Item("U_FIB_DocStPkg").Value = value.U_FIB_DocStPkg;
                    orders.UserFields.Fields.Item("U_FIB_IsPkg").Value = value.U_FIB_IsPkg;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    orders.CardCode = value.CardCode;
                    orders.CardName = value.CardName;
                    orders.ContactPersonCode = value.CntctCode;
                    orders.NumAtCard = value.NumAtCard;
                    orders.DocCurrency = value.DocCur;
                    orders.DocRate = value.DocRate;

                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    orders.PayToCode = value.PayToCode;
                    orders.Address = value.Address;
                    orders.ShipToCode = value.ShipToCode;
                    orders.Address2 = value.Address2;

                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================
                    orders.GroupNumber = value.GroupNum;

                    // ===========================================================================================
                    // AGENCIA
                    // ===========================================================================================
                    orders.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                    orders.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    orders.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    orders.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                    orders.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    orders.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                    orders.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                    orders.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                    orders.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                    orders.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;

                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    orders.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    orders.SalesPersonCode = value.SlpCode;
                    orders.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                    orders.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    orders.Comments = value.Comments;

                    // ===========================================================================================
                    // TOTALES
                    // ===========================================================================================
                    orders.DiscountPercent = value.DiscPrcnt;
                    orders.DocTotal = value.DocTotal;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    orders.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                    #endregion


                    #region <<< DETALLE >>>

                    bool isItem = value.DocType == "I";
                    bool isService = value.DocType == "S";

                    foreach (var line in value.Lines)
                    {
                        if (isItem)
                        {
                            orders.Lines.ItemCode = line.ItemCode;
                            orders.Lines.WarehouseCode = line.WhsCode;
                            orders.Lines.MeasureUnit = line.UnitMsr;
                            orders.Lines.Quantity = line.Quantity;
                        }

                        if (isService)
                        {
                            orders.Lines.AccountCode = line.AcctCode;
                        }

                        orders.Lines.ItemDescription = line.Dscription;

                        orders.Lines.Currency = line.Currency;
                        orders.Lines.UnitPrice = line.PriceBefDi;
                        orders.Lines.DiscountPercent = line.DiscPrcnt;
                        orders.Lines.Price = line.Price;

                        orders.Lines.TaxCode = line.TaxCode;
                        orders.Lines.LineTotal = line.LineTotal;

                        // UDFs
                        orders.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                        orders.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                        orders.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        orders.Lines.Add();
                    }

                    #endregion


                    #region <<< APROBACIÓN >>>

                    bool requiereAprobacion = orders.GetApprovalTemplates() == 0;

                    if (requiereAprobacion)
                    {
                        orders.Document_ApprovalRequests.SetCurrentLine(0);
                        orders.Document_ApprovalRequests.Remarks = "Orden de Venta creada desde FibraTech";
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
                        orders.AttachmentEntry = int.Parse(company.GetNewObjectKey());
                    }

                    #endregion


                    if (orders.Add() != 0)
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
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    if(aplicaAprobacion)
                    {
                        _companyProviderSap.DisconnectCompany();
                    }
                    _companyProviderSap.LiberarObjetosCOM(orders, attachments);
                }

                return resultTransaccion;
            });
        }
        public async Task<ResultadoTransaccionResponse<OrdersEntity>> SetUpdate(OrdersUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Documents orders = null;
            Attachments2 attachments = null;
            Attachments2 newAttachment = null;
            Attachments2 oldAttachment = null;


            return await Task.Run(() =>
            {
                try
                {
                    // 🔹 Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    // 🔹 Se crea el objeto de orden de venta
                    orders = company.GetBusinessObject(BoObjectTypes.oOrders);

                    // 🔹 Validar existencia de la orden de venta
                    if (!orders.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la orden de venta.");
                    }


                    #region <<< CABECERA >>>

                    if (orders.DocumentStatus == BoStatus.bost_Open)
                    {
                        orders.DocDate = value.DocDate;
                        orders.DocDueDate = value.DocDueDate;
                    }                    
                    orders.TaxDate = value.TaxDate;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    orders.CardCode = value.CardCode;
                    orders.ContactPersonCode = value.CntctCode;
                    orders.NumAtCard = value.NumAtCard;
                    if (orders.DocumentStatus == BoStatus.bost_Open)
                    {
                        orders.DocCurrency = value.DocCur;
                        orders.DocRate = value.DocRate;
                    }

                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    if (orders.DocumentStatus == BoStatus.bost_Open && (BoApprovalRequestStatusEnum)orders.AuthorizationStatus != BoApprovalRequestStatusEnum.arsApproved)
                    {
                        orders.PayToCode = value.PayToCode;
                        orders.Address = value.Address;
                        orders.ShipToCode = value.ShipToCode;
                        orders.Address2 = value.Address2;
                    }


                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================
                    if (orders.DocumentStatus == BoStatus.bost_Open && (BoApprovalRequestStatusEnum)orders.AuthorizationStatus != BoApprovalRequestStatusEnum.arsApproved)
                    {
                        orders.GroupNumber = value.GroupNum;
                    }

                    // ===========================================================================================
                    // AGENCIA
                    // ===========================================================================================
                    // Código de agencia de transporte
                    orders.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                    // RUC de la agencia de transporte
                    orders.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    // Nombre de la agencia de transporte
                    orders.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    // Código de dirección de la agencia de transporte
                    orders.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                    // Dirección de la agencia de transporte
                    orders.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    orders.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                    orders.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                    orders.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                    orders.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                    orders.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;

                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    orders.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    orders.SalesPersonCode = value.SlpCode;
                    orders.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                    orders.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    orders.Comments = value.Comments;

                    // ===========================================================================================
                    // TOTALES
                    // ===========================================================================================
                    orders.DiscountPercent = value.DiscPrcnt;
                    orders.DocTotal = value.DocTotal;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    orders.UserFields.Fields.Item("U_UsrUpdate").Value = value.U_UsrUpdate;

                    #endregion


                    #region <<< DETALLE >>>

                    bool isItem = value.DocType == "I";
                    bool isService = value.DocType == "S";

                    // NUEVO: SE AGREGA NUEVO ITEM
                    foreach (var line in value.Lines.Where(x => x.Record == 1))
                    {
                        orders.Lines.Add();

                        if (isItem)
                        {
                            orders.Lines.ItemCode = line.ItemCode;
                            orders.Lines.WarehouseCode = line.WhsCode;
                            orders.Lines.Quantity = line.Quantity;
                        }

                        if (isService)
                        {
                            orders.Lines.AccountCode = line.AcctCode;
                        }

                        orders.Lines.ItemDescription = line.Dscription;
                        orders.Lines.Currency = line.Currency;
                        orders.Lines.UnitPrice = line.PriceBefDi;
                        orders.Lines.DiscountPercent = line.DiscPrcnt;
                        orders.Lines.Price = line.Price;

                        orders.Lines.TaxCode = line.TaxCode;
                        orders.Lines.LineTotal = line.LineTotal;

                        orders.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                        orders.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                        orders.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                    }

                    // EXISTE: SE MODIFICA EL ITEM
                    foreach (var line in value.Lines.Where(x => x.Record == 2 && x.LineStatus == "O"))
                    {
                        for (int i = 0; i < orders.Lines.Count; i++)
                        {
                            orders.Lines.SetCurrentLine(i);

                            if (orders.Lines.LineNum != line.LineNum)
                                continue;

                            if (isItem)
                            {
                                orders.Lines.ItemCode = line.ItemCode;
                                orders.Lines.WarehouseCode = line.WhsCode;
                                orders.Lines.Quantity = line.Quantity;
                            }

                            if (isService)
                            {
                                orders.Lines.AccountCode = line.AcctCode;
                            }

                            orders.Lines.ItemDescription = line.Dscription;
                            orders.Lines.Currency = line.Currency;
                            orders.Lines.UnitPrice = line.PriceBefDi;
                            orders.Lines.DiscountPercent = line.DiscPrcnt;
                            orders.Lines.Price = line.Price;

                            orders.Lines.TaxCode = line.TaxCode;
                            orders.Lines.LineTotal = line.LineTotal;

                            orders.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                            orders.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                            orders.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        }
                    }

                    // EXISTE: SE ELIMINA EL ITEM
                    foreach (var line in value.Lines.Where(x => x.Record == 3))
                    {
                        for (int i = 0; i < orders.Lines.Count; i++)
                        {
                            orders.Lines.SetCurrentLine(i);
                            if (orders.Lines.LineNum == line.LineNum)
                            {
                                orders.Lines.Delete();
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
                        orders.AttachmentEntry = int.Parse(company.GetNewObjectKey());
                    }

                    #endregion


                    if (orders.Update() != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "La orden de venta actualizada con éxito.";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(orders, oldAttachment, newAttachment);
                }

                return resultTransaccion;
            });
        }
        public async Task<ResultadoTransaccionResponse<OrdersEntity>> SetClose(OrdersCloseEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Documents orders = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    orders = company.GetBusinessObject(BoObjectTypes.oOrders);


                    if (!orders.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la orden de venta.");
                    }


                    var regClose = orders.Close();


                    if (regClose == 0)
                    {
                        // Configurar el descuento 100% en "Autorizaciones generales" en SAP Business One, sino saldrá error al actualizar
                        orders = company.GetBusinessObject(BoObjectTypes.oOrders);


                        if (!orders.GetByKey(value.DocEntry))
                        {
                            throw new Exception("No existe la orden de venta.");
                        }


                        orders.UserFields.Fields.Item("U_UsrClose").Value = value.U_UsrClose;


                        var regUpdate = orders.Update();


                        if (regUpdate != 0)
                        {
                            company.GetLastError(out int errorCode, out string errorMessage);
                            throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                        }

                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "Orden de venta cerrada con éxito ..!";
                    }
                    else
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
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
                    _companyProviderSap.LiberarObjetosCOM(orders);
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


                var data = await _db.Orders
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new OrdersQueryEntity
                {
                    DocNum = n.DocNum,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    DocTime = n.DocTime.ToString().Insert(n.DocTime.ToString().Length - 2, ":").PadLeft(5, '0'),

                    // SOCIO DE NEGOCIOS
                    LicTradNum = n.BusinessPartners.LicTradNum ?? "",
                    CardName = n.CardName ?? "",

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
                    .Select(s => new Orders1QueryEntity
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


                var header = new HeaderOrdersNational()
                {
                    PrintHeadr = adminInfo.PrintHeadr,

                    DocNum = data.DocNum.ToString(),
                    DocDate = data.DocDate.ToString("dd/MM/yyyy"),
                    DocDueDate = data.DocDueDate.ToString("dd/MM/yyyy"),
                    DocTime = data.DocTime,

                    CardName = data.CardName,
                    LicTradNum = data.LicTradNum,

                    Address2 = data.Address2,

                    U_OrdenCompra = data.U_OrdenCompra,
                };


                var footer = new FooterOrdersNational()
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
                var pageEventHelper = new PageEventHelperOrdersNational();
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
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Quantity.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].OpenQty.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Delivered.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                }

                doc.Add(tbl);


                tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 16f, 1f, 62f, 13f, 8f }) { WidthPercentage = 100 };
                // Fila 1
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Empleado del departamento de ventas", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.SlpName, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("SubTotal", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.SubTotal.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 2
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Condiciones de pago", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.PymntGroup, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Descuento", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.DiscSum.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 3
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Datos de Transporte", parrafoNegrita)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Impuesto", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.VatSum.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                tbl.AddCell(c1);

                // Fila 4
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.U_BPP_MDNT, parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3, Colspan = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Total", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.DocTotal.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
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


                var data = await _db.Orders
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new OrdersQueryEntity
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
                    .Select(s => new Orders1QueryEntity
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


                var header = new HeaderOrdersExportPlanta()
                {
                    PrintHeadr = adminInfo.PrintHeadr,

                    DocNum = data.DocNum.ToString(),
                    DocDate = data.DocDate.ToString("dd/MM/yyyy"),
                    DocDueDate = data.DocDueDate.ToString("dd/MM/yyyy"),
                    DocTime = data.DocTime,

                    CardName = data.CardName,
                    LicTradNum = data.LicTradNum,

                    Address = data.Address,
                    Address2 = data.Address2,

                    U_OrdenCompra = data.U_OrdenCompra,
                };


                var footer = new FooterOrdersExportPlanta()
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
                var pageEventHelper = new PageEventHelperOrdersExportPlanta();
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
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Quantity.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].OpenQty.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
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

                    CountryName = a.AdminInfo1.CountryEntity.Name
                })
                .FirstOrDefaultAsync();


                var data = await _db.Orders
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new OrdersQueryEntity
                {
                    DocNum = n.DocNum,
                    TaxDate = n.TaxDate,

                    // SOCIO DE NEGOCIOS
                    CardCode = n.CardCode,
                    LicTradNum = n.BusinessPartners.LicTradNum ?? "",
                    CardName = n.CardName ?? "",
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
                    .Select(s => new Orders1QueryEntity
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


                var header = new HeaderOrdersExportCliente()
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
                    CardName    = data.CardName,
                    LicTradNum = data.LicTradNum,
                    DocCur = data.DocCur,

                    Address = data.Address,
                    Address2 = data.Address2,

                    TipoFleteDescr = data.TipoFleteDescr,
                    U_FIB_PUERTO = data.U_FIB_PUERTO,
                    U_STR_FEMB = data.U_STR_FEMB,

                    SlpName = data.SlpName[..Math.Min(35, data.SlpName.Length)],
                    U_OrdenCompra = data.U_OrdenCompra,
                };

                var footer = new FooterOrdersExportCliente()
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
                var pageEventHelper = new PageEventHelperOrdersExportCliente();
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
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Quantity.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].Price.ToString("N3"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data.Lines[i].LineTotal.ToString("N3"), parrafoNormal)) { BorderWidth = 1, PaddingBottom = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                }
                
                doc.Add(tbl);


                tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 79f, 13f, 8f }) { WidthPercentage = 100 };
                // Fila 1
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(HelperAmountToLetters.AmountToLetters(data.DocTotal,data.CurrName), parrafoNormal)) { BorderWidth = 0, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Importe FOB", parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(data?.SubTotal.ToString("N2"), parrafoNormal)) { BorderWidth = 1, PaddingTop = 3, PaddingBottom = 3 , HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT };
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

    public class HeaderOrdersNational
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

    public class FooterOrdersNational
    {
        public string Texto { get; set; }
    }

    public class PageEventHelperOrdersNational : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate headerTemplate, footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;

        #region Properties
        public HeaderOrdersNational Header { get; set; }
        public FooterOrdersNational Footer { get; set; }
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
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(150), cb);
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

    public class HeaderOrdersExportPlanta
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

    public class FooterOrdersExportPlanta
    {
        public string U_BPP_MDNT { get; set; }
        public string U_BPP_MDDT { get; set; }
        public string Comments { get; set; }
    }

    public class PageEventHelperOrdersExportPlanta : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate headerTemplate, footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;

        #region Properties
        public HeaderOrdersExportPlanta Header { get; set; }
        public FooterOrdersExportPlanta Footer { get; set; }
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

    public class HeaderOrdersExportCliente
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
        public string DocCur { get; set; }


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
        public string U_STR_FEMB { get; set; }


        /// <summary>
        /// SALES EMPLOYEE
        /// </summary>

        public string SlpName { get; set; }
        public string U_OrdenCompra { get; set; }
    }
    public class FooterOrdersExportCliente
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
    public class PageEventHelperOrdersExportCliente : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate headerTemplate, footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;

        #region Properties
        public HeaderOrdersExportCliente Header { get; set; }
        public FooterOrdersExportCliente Footer { get; set; }
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
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.CardName, parrafoNormal)) { BorderWidth = 1, BorderWidthBottom = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 3 };
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
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Header.Address2, parrafoNormal)) { BorderWidth = 1, BorderWidthTop = 0, PaddingLeft = 5, PaddingTop = 3, PaddingRight = 5, PaddingBottom = 10, FixedHeight = alturaLinea, NoWrap = false };
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
            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 1, BorderWidthLeft = 0, PaddingTop = 3, PaddingBottom = 5 };
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
            var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 23f, 1f, 18f ,1f, 20f, 1f, 18f, 1f, 17f });
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
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(230), cb);


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
            tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetBottom(140), cb);

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
