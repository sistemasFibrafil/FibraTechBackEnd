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
namespace Net.Data.Sap
{
    public class OrdersRepository : RepositoryBase<OrdersEntity>, IOrdersRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly DataContextSap _db;
        private readonly CompanyProviderSap _companyProviderSap;

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


        public OrdersRepository(IConnectionSQL context, IConfiguration configuration, DataContextSap db, CompanyProviderSap companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }



        public async Task<ResultadoTransaccionEntity<OrdersEntity>> GetListByFilter(OrdersFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OrdersEntity>
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
                .Select(n => new OrdersEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    DocStatus = n.DocStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    U_FIB_IsPkg = n.U_FIB_IsPkg,
                    CardCode = n.CardCode,
                    CardName = n.CardName,
                    DocCur = n.DocCur,
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

        public async Task<ResultadoTransaccionEntity<OrdersQueryEntity>> GetByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OrdersQueryEntity>
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

                var data = await _db.Orders
                .Where(n => n.DocEntry == docEntry)
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
                    PayAddressList = _db.Direccion
                                     .Where(a => a.CardCode == n.CardCode && a.AdresType == "B")
                                     .OrderBy(a => a.LineNum)
                                     .Select(a => new DireccionEntity
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
                    ShipAddressList = _db.Direccion
                                      .Where(a => a.CardCode == n.CardCode && a.AdresType == "S")
                                      .OrderBy(a => a.LineNum)
                                      .Select(a => new DireccionEntity
                                      {
                                          CardCode = a.CardCode,
                                          AdresType = a.AdresType,
                                          Address = a.Address,
                                          Street = a.Street,
                                          LineNum = a.LineNum
                                      })
                                      .ToList(),
                    Address2 = n.Address2,

                    U_BPP_MDCT = n.U_BPP_MDCT,
                    U_BPP_MDRT = n.U_BPP_MDRT,
                    U_BPP_MDNT = n.U_BPP_MDNT,
                    U_FIB_AgencyToCode = n.U_FIB_AgencyToCode,
                    // ✅ DIRECCIONES DE DESPACHO (CRD1 AdresType = 'S')
                    AgencyAddressList = _db.Direccion
                                        .Where(a => a.CardCode == n.U_BPP_MDCT && a.AdresType == "S")
                                        .OrderBy(a => a.LineNum)
                                        .Select(a => new DireccionEntity
                                        {
                                            CardCode = a.CardCode,
                                            AdresType = a.AdresType,
                                            Address = a.Address,
                                            Street = a.Street,
                                            LineNum = a.LineNum
                                        })
                                        .ToList(),
                    U_BPP_MDDT = n.U_BPP_MDDT,

                    U_TipoFlete = n.U_TipoFlete,
                    U_ValorFlete = n.U_ValorFlete,
                    U_FIB_TFLETE = n.U_FIB_TFLETE,
                    U_FIB_IMPSEG = n.U_FIB_IMPSEG,
                    U_FIB_PUERTO = n.U_FIB_PUERTO,

                    U_STR_TVENTA = n.U_STR_TVENTA,

                    SlpCode = n.SlpCode,

                    Comments = n.Comments,

                    SubTotal = adminInfo.MaMainCurncy == n.DocCur ? (n.DocTotal - n.VatSum + n.DiscSum) : (n.DocTotalSy - n.VatSumSy + n.DiscSumSy),
                    DiscPrcnt = n.DiscPrcnt,
                    DiscSum = adminInfo.MaMainCurncy == n.DocCur ? n.DiscSum : n.DiscSumSy,
                    VatSum = adminInfo.MaMainCurncy == n.DocCur ? n.VatSum : n.VatSumSy,
                    DocTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal : n.DocTotalSy,

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
                        U_FIB_OpQtyPkg = s.U_FIB_OpQtyPkg,
                        OnHand = s.Item.OnHand,
                        Currency = s.Currency,
                        PriceBefDi = s.PriceBefDi,
                        DiscPrcnt = s.DiscPrcnt,
                        Price = s.Price,
                        TaxCode = s.TaxCode,
                        VatPrcnt = s.VatPrcnt,
                        VatSum = adminInfo.MaMainCurncy == s.Currency ? s.VatSum : s.VatSumSy,
                        U_tipoOpT12 = s.U_tipoOpT12 ?? "",
                        U_tipoOpT12Nam = s.TipoOperacion != null ? s.TipoOperacion.U_descrp : "",
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
        public async Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListSeguimientoByFilter(OrdersSeguimientoFindEntity value)
        {
            var response = new List<OrdersFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdersFechaEntity>();

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
                            response = (List<OrdersFechaEntity>)context.ConvertTo<OrdersFechaEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoByFilterExcel(OrdersSeguimientoFindEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();
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
        public async Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListSeguimientoDetalladoDireccionFiscalByFilter(OrdersSeguimientoFindEntity value)
        {
            var response = new List<OrdersFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdersFechaEntity>();

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
                            response = (List<OrdersFechaEntity>)context.ConvertTo<OrdersFechaEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoDetalladoDireccionFiscalByFilterExcel(OrdersSeguimientoFindEntity value)
        {
            var ms = new MemoryStream();
            var resultadoTransaccion = new ResultadoTransaccionEntity<MemoryStream>();
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

        public async Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListSeguimientoDetalladoDireccionDespachoByFilter(OrdersSeguimientoFindEntity value)
        {
            var response = new List<OrdersFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdersFechaEntity>();

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
                            response = (List<OrdersFechaEntity>)context.ConvertTo<OrdersFechaEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoDetalladoDireccionDespachoByFilterExcel(OrdersSeguimientoFindEntity value)
        {
            var ms = new MemoryStream();
            var resultadoTransaccion = new ResultadoTransaccionEntity<MemoryStream>();
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


        public async Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListOrdenVentaPendienteStockAlmacenProduccionByFecha(FilterRequestEntity value)
        {
            var response = new List<OrdersFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdersFechaEntity>();

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
                            response = (List<OrdersFechaEntity>)context.ConvertTo<OrdersFechaEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetOrdenVentaPendienteStockAlmacenProduccionExcelByFecha(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();
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


        public async Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListOrdenVentaProgramacionByFecha(FilterRequestEntity value)
        {
            var response = new List<OrdersFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdersFechaEntity>();

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
                            response = (List<OrdersFechaEntity>)context.ConvertTo<OrdersFechaEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetOrdenVentaProgramacionExcelByFecha(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();
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


        public async Task<ResultadoTransaccionEntity<OrdenVentaSodimacSapEntity>> GetListOrdenVentaSodimacPendienteByFiltro(FilterRequestEntity value)
        {
            var response = new List<OrdenVentaSodimacSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdenVentaSodimacSapEntity>();

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
                            response = (List<OrdenVentaSodimacSapEntity>)context.ConvertTo<OrdenVentaSodimacSapEntity>(reader);
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

        public async Task<ResultadoTransaccionEntity<OrdenVentaSodimacSapEntity>> GetOrdenVentaSodimacPendienteById(FilterRequestEntity value)
        {
            var response = new OrdenVentaSodimacSapEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdenVentaSodimacSapEntity>();

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
                            response = context.Convert<OrdenVentaSodimacSapEntity>(reader);
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


        public async Task<ResultadoTransaccionEntity<OrdersFechaEntity>> GetListOrdenVentaPreliminarPendienteByFecha(FilterRequestEntity value)
        {
            var response = new List<OrdersFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdersFechaEntity>();

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
                            response = (List<OrdersFechaEntity>)context.ConvertTo<OrdersFechaEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetListOrdenVentaPreliminarPendienteExcelByFecha(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultadoTransaccion = new ResultadoTransaccionEntity<MemoryStream>();
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

        public async Task<ResultadoTransaccionEntity<OrdersEntity>> SetCreate(OrdersCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OrdersEntity>
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

                    #region <<< CABECERA >>>
                    orders.DocDate = value.DocDate;
                    orders.DocDueDate = value.DocDueDate;
                    orders.TaxDate = value.TaxDate;

                    orders.DocObjectCode = BoObjectTypes.oOrders;
                    orders.DocType = value.DocType == "I" ? BoDocumentTypes.dDocument_Items : BoDocumentTypes.dDocument_Service;
                    orders.UserFields.Fields.Item("U_FIB_DocStPkg").Value = value.U_FIB_DocStPkg;
                    orders.UserFields.Fields.Item("U_FIB_IsPkg").Value = value.U_FIB_IsPkg;
                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    orders.CardCode = value.CardCode;
                    orders.CardName = value.CardName;
                    orders.ContactPersonCode = value.CntctCode ?? 0;
                    orders.NumAtCard = value.NumAtCard;
                    orders.DocCurrency = value.DocCur;
                    orders.DocRate = (double)value.DocRate;
                    // ===========================================================================================
                    // LOGÍSTICA
                    // ===========================================================================================
                    orders.ShipToCode = value.ShipToCode;
                    orders.Address2 = value.Address2;
                    orders.PayToCode = value.PayToCode;
                    orders.Address = value.Address;
                    // ===========================================================================================
                    // FINANZAS
                    // ===========================================================================================
                    orders.GroupNumber = value.GroupNum;
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
                    orders.UserFields.Fields.Item("U_FIB_AgencyToCode").Value = value.U_FIB_AgencyToCode;
                    // Dirección de la agencia de transporte
                    orders.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;
                    // ===========================================================================================
                    // EXPORTACIÓN
                    // ===========================================================================================
                    orders.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                    orders.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete ?? 0;
                    orders.UserFields.Fields.Item("U_FIB_TFLETE").Value = Convert.ToDouble(Math.Round(Convert.ToDecimal(value.U_FIB_TFLETE ?? 0), 6));
                    orders.UserFields.Fields.Item("U_FIB_IMPSEG").Value = Convert.ToDouble(Math.Round(Convert.ToDecimal(value.U_FIB_IMPSEG ?? 0), 6));
                    orders.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;
                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    orders.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;
                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    orders.SalesPersonCode = value.SlpCode == 0 ? -1 : value.SlpCode ?? -1;
                    orders.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                    orders.Comments = value.Comments;
                    // ===========================================================================================
                    // TOTALES
                    // ===========================================================================================
                    orders.DiscountPercent = (double)value.DiscPrcnt;
                    orders.DocTotal = (double)value.DocTotal;
                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    if (value.U_UsrCreate != null) orders.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                    #endregion

                    #region <<< DETALLE >>>

                    foreach (var line in value.Lines)
                    {
                        orders.Lines.ItemCode = line.ItemCode;
                        orders.Lines.ItemDescription = line.Dscription;
                        orders.Lines.WarehouseCode = line.WhsCode;
                        if (line.U_tipoOpT12 != null || line.U_tipoOpT12 == "") orders.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        orders.Lines.Quantity = (double)line.Quantity;
                        if (line.U_FIB_OpQtyPkg != null) orders.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = Convert.ToDouble(Math.Round(Convert.ToDecimal(line.U_FIB_OpQtyPkg ?? 0), 6));
                        orders.Lines.Currency = line.Currency;
                        orders.Lines.UnitPrice = (double)line.PriceBefDi;
                        orders.Lines.DiscountPercent = (double)line.DiscPrcnt;
                        orders.Lines.Price = (double)line.Price;
                        orders.Lines.TaxCode = line.TaxCode;
                        orders.Lines.LineTotal = (double)line.LineTotal;
                        orders.Lines.Add();
                    }

                    #endregion

                    var reg = orders.Add();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "La solictud de transferencia registrada con éxito.";
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
                    _companyProviderSap.LiberarObjetosCOM(orders);
                }

                return resultTransaccion;
            });
        }
    }
}
