using System;
using System.IO;
using SAPbobsCOM;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
namespace Net.Data.Sap
{
    public class ItemsRepository : RepositoryBase<ItemsEntity>, IItemsRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _conSap;
        private readonly DataContextSap _db;
        private readonly CompanyProviderSap _companyProviderSap;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";

        const string SP_GET_LIST_ARTICULO_VENTA_GRUPO_SUBGRUPO_ESTADO = DB_ESQUEMA + "INV_GetListArticuloVentaByGrupoSubGrupoEstado";
        const string SP_GET_LIST_ARTICULO_VENTA_STOCK_BY_GRUPO_SUBGRUPO = DB_ESQUEMA + "INV_GetListArticuloVentaStockByGrupoSubGrupo";
        const string SP_GET_LIST_ARTICULO_BY_GRUPO_SUBGRUPO_FILTRO = DB_ESQUEMA + "INV_GetListArticuloByGrupoSubGrupoFiltro";
        const string SP_GET_LIST_MOVIMIENTO_STOCK_BY_FECHA_SEDE = DB_ESQUEMA + "INV_GetListMovimientoStockByFechaSede";

        // Para generación de OV de SODIMAC
        const string SP_GET_FOR_ORDEN_VENTA_SODIMAC_SKU = DB_ESQUEMA + "VEN_GetArticuloForOrdenVentaSodimacBySku";
        // Para generación de OV
        const string SP_GET_ARTICULO_VENTA_BY_CODE = DB_ESQUEMA + "INV_GetArticuloVentaByCode";


        public ItemsRepository(IConnectionSQL context, IConfiguration configuration, DataContextSap db, CompanyProviderSap companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
            _conSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }

        
        public async Task<ResultadoTransaccionEntity<ItemsEntity>> GetListByFilter(ItemsFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ItemsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                IQueryable<ItemsEntity> query = _db.Items.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(value.InvntItem))
                {
                    query = query.Where(n => n.InvntItem == value.InvntItem);
                }

                if (!string.IsNullOrWhiteSpace(value.SellItem))
                {
                    query = query.Where(n => n.SellItem == value.SellItem);
                }

                if (!string.IsNullOrWhiteSpace(value.PrchseItem))
                {
                    query = query.Where(n => n.PrchseItem == value.PrchseItem);
                }

                if (!string.IsNullOrWhiteSpace(value.Item))
                {
                    var bPartner = value.Item.Trim();
                    query = query.Where(n =>
                    n.ItemCode.Contains(bPartner) ||
                    n.ItemName.Contains(bPartner));
                }


                var list = await query
                .Select(n => new ItemsEntity
                {
                    ItemCode = n.ItemCode,
                    ItemName = n.ItemName,
                    OnHand = n.OnHand,
                    frozenFor = n.frozenFor,
                }).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.dataList = list;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<ItemsQueryEntity>> GetListByCode(ItemsByDocumentsFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ItemsQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            var sumDec = (short)0;
            var listNum = (short)0;
            var vatPrcnt = 0m;
            var quantity = 1m;
            var taxCode = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(value.ItemCode)) throw new Exception("El código de artículo no es válido.");

                // 🔹 Limpia y prepara códigos
                var itemCodes = value.ItemCode
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();

                if(!string.IsNullOrWhiteSpace(value.CardCode))
                {
                    // 🔹 Para decimales
                    sumDec = await _db.AdminInfo
                    .Select(n => n.SumDec)
                    .FirstOrDefaultAsync();

                    // 🔹 Precio List
                    listNum = await _db.BusinessPartners
                    .Where(n => n.CardCode == value.CardCode)
                    .Select(n => n.ListNum)
                    .FirstOrDefaultAsync();

                    var exonerados = new HashSet<int> { 5, 24, 36 };
                    taxCode = exonerados.Contains(value.SlpCode) ? "EXO" : "IGV18";

                    // 🔹 Impuesto
                    vatPrcnt = await _db.TaxGroups
                    .Where(n => n.Code == taxCode)
                    .Select(n => n.Rate)
                    .FirstOrDefaultAsync();
                }


                // 🔹 Diccionario para orden personalizado
                var orderMap = itemCodes
                .Select((code, index) => new { code, index })
                .ToDictionary(x => x.code, x => x.index);


                // 🔹 Tipo de operación (puede ser null)
                var tipoOperacion = await _db.TipoOperacion.AsNoTracking().FirstOrDefaultAsync(t => t.Code == value.CodTipoOperacion);


                var list = await _db.Items
                .AsNoTracking()
                .Where(i => itemCodes.Contains(i.ItemCode))
                .Select(i => new
                {
                    Item = i,
                    Price = i.PriceLists
                            .Where(p => p.PriceList == listNum && p.Currency == value.Currency)
                            .Select(p => (decimal?)p.Price ?? 0)
                            .FirstOrDefault()
                })
                .Select(x => new ItemsQueryEntity
                {
                    ItemCode = x.Item.ItemCode,
                    ItemName = x.Item.ItemName,

                    // 🔹 Precio
                    PriceBefDi = x.Price,
                    Price = x.Price,

                    // 🔹 Cantidad
                    Quantity = quantity,

                    // 🔹 Impuestos
                    TaxCode = taxCode,
                    VatPrcnt = vatPrcnt,

                    // 🔹 Cálculos
                    LineTotal = Math.Round(quantity * (x.Price), sumDec),
                    VatSum = Math.Round((Math.Round(quantity * (x.Price), sumDec) * vatPrcnt) / 100, sumDec),

                    // 🔹 Datos ya existentes
                    DfltWH = x.Item.DfltWH,
                    AcctCode = x.Item.DefaultWarehouse != null ? x.Item.DefaultWarehouse.BalInvntAc : "",
                    FormatCode = x.Item.DefaultWarehouse != null &&
                                 x.Item.DefaultWarehouse.ChartOfAccounts != null
                                 ? string.Concat
                                 (
                                     x.Item.DefaultWarehouse.ChartOfAccounts.Segment_0, "-",
                                     x.Item.DefaultWarehouse.ChartOfAccounts.Segment_1, "-",
                                     x.Item.DefaultWarehouse.ChartOfAccounts.Segment_2
                                 )
                                 : "",
                    AcctName = x.Item.DefaultWarehouse != null &&
                               x.Item.DefaultWarehouse.ChartOfAccounts != null
                               ? x.Item.DefaultWarehouse.ChartOfAccounts.AcctName
                               : "",

                    BuyUnitMsr = x.Item.BuyUnitMsr,
                    SalUnitMsr = x.Item.SalUnitMsr,
                    InvntryUom = x.Item.InvntryUom,
                    OnHand = x.Item.OnHand,

                    U_tipoOpT12 = tipoOperacion != null ? tipoOperacion.Code : "",
                    U_tipoOpT12Nam = tipoOperacion != null ? tipoOperacion.U_descrp : ""
                })
                .ToListAsync();



                // 🔹 Ordena respetando el orden de entrada
                list = list
                .OrderBy(x => orderMap.TryGetValue(x.ItemCode, out var idx) ? idx : int.MaxValue)
                .ToList();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.dataList = list;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }


        public async Task<ResultadoTransaccionEntity<ItemsStockGeneralViewEntity>> GetListStockGeneralSummary(ItemsStockGeneralViewFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ItemsStockGeneralViewEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.ItemsStockGeneralView.AsNoTracking();

                if (value.ExcluirInactivo)
                {
                    query = query.Where(n => n.frozenFor == "N");
                }

                if (value.ExcluirSinStock)
                {
                    query = query.Where(n => n.OnHand - n.IsCommited + n.OnOrder != 0);
                }

                // FILTRO POR ALMACÉN
                if (!string.IsNullOrWhiteSpace(value.WhsCode))
                {
                    var whsCode = value.WhsCode.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(x => whsCode.Contains(x.WhsCode));
                }

                //// FILTRO POR ITEM
                //if (!string.IsNullOrWhiteSpace(value.Item))
                //{
                //    // Deja un solo espacio y reemplaza por %
                //    var filter = Regex.Replace(value.Item.Trim(), @"\s+", " ").Replace(" ", "%");

                //    query = query.Where(x =>
                //        EF.Functions.Like(EF.Functions.Collate(x.ItemCode!, GlobalVariables.CI), $"%{filter}%") ||
                //        EF.Functions.Like(EF.Functions.Collate(x.ItemName!, GlobalVariables.CI), $"%{filter}%")
                //    );
                //}


                var list = await query
                .GroupBy(n => new
                {
                    n.ItemCode,
                    n.ItemName,
                    n.InvntryUom,
                    n.PesoPromedioKg,
                    n.FecProduccion
                })
                .Select(g => new ItemsStockGeneralViewEntity
                {
                    ItemCode = g.Key.ItemCode,
                    ItemName = g.Key.ItemName,
                    InvntryUom = g.Key.InvntryUom,

                    OnHand = g.Sum(x => x.OnHand),
                    IsCommited = g.Sum(x => x.IsCommited),
                    OnOrder = g.Sum(x => x.OnOrder),
                    Available = g.Sum(x => x.Available),
                    PesoKg = g.Sum(x => x.PesoKg),
                    PesoPromedioKg = g.Key.PesoPromedioKg,
                    FecProduccion = g.Key.FecProduccion
                })
                .OrderBy(n => n.ItemCode)
                .ThenBy(n => n.ItemName)
                .ThenBy(n => n.InvntryUom)
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.dataList = list;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetStockGeneralSummaryExcel(ItemsStockGeneralViewFilterEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    DocumentFormat.OpenXml.Packaging.WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                    DocumentFormat.OpenXml.Packaging.WorksheetPart worksheetPart = workbookPart.AddNewPart<DocumentFormat.OpenXml.Packaging.WorksheetPart>();
                    worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());
                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Stock General" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData = worksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

                    //Cabecera
                    DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    row.Append
                    (
                        ExportToExcel.ConstructCell("Código de Artículo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Nombre de Artículo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("UM", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Stock", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Comprometido", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Solicitado", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Disponible", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Peso Kg", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Peso Promedio Kg", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Fecha de Producción", DocumentFormat.OpenXml.Spreadsheet.CellValues.String)
                    );
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListStockGeneralSummary(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        row.Append
                        (
                            ExportToExcel.ConstructCell(item.ItemCode, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                            ExportToExcel.ConstructCell(item.ItemName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                            ExportToExcel.ConstructCell(item.InvntryUom, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                            ExportToExcel.ConstructCell(item.OnHand.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.IsCommited.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.OnOrder.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.Available.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.PesoKg.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.FecProduccion == null ? "" : Convert.ToDateTime(item.FecProduccion).ToString("dd/MM/yyyy"), DocumentFormat.OpenXml.Spreadsheet.CellValues.String)
                        );
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


        public async Task<ResultadoTransaccionEntity<ItemsStockGeneralViewEntity>> GetListStockGeneralDetailed(ItemsStockGeneralViewFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ItemsStockGeneralViewEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.ItemsStockGeneralView.AsNoTracking();

                if (value.ExcluirInactivo)
                {
                    query = query.Where(n => n.frozenFor == "N");
                }

                if (value.ExcluirSinStock)
                {
                    query = query.Where(n => n.OnHand - n.IsCommited + n.OnOrder != 0);
                }

                // FILTRO POR ALMACÉN
                if (!string.IsNullOrWhiteSpace(value.WhsCode))
                {
                    var whsCode = value.WhsCode.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(x => whsCode.Contains(x.WhsCode));
                }

                //// FILTRO POR ITEM
                //if (!string.IsNullOrWhiteSpace(value.Item))
                //{
                //    // 1️.- deja un solo espacio
                //    var filter = Regex.Replace(value.Item.Trim(), @"\s+", " ");
                //    // 2️.- reemplaza por %
                //    filter = filter.Replace(" ", "%");

                //    query = query.Where(x =>
                //        EF.Functions.Like(EF.Functions.Collate(x.ItemCode!, GlobalVariables.CI), $"%{filter}%") ||
                //        EF.Functions.Like(EF.Functions.Collate(x.ItemName!, GlobalVariables.CI), $"%{filter}%")
                //    );
                //}

                var list = await query
                .OrderBy(n => n.ItemCode)
                .ThenBy(n => n.ItemName)
                .ThenBy(n => n.WhsName)
                .ThenBy(n => n.InvntryUom)
                .Select(n => new ItemsStockGeneralViewEntity
                {
                    ItemCode = n.ItemCode,
                    ItemName = n.ItemName,
                    WhsName = n.WhsName,
                    InvntryUom = n.InvntryUom,
                    OnHand = n.OnHand,
                    IsCommited = n.IsCommited,
                    OnOrder = n.OnOrder,
                    Available = n.Available,
                    PesoPromedioKg = n.PesoPromedioKg,
                    PesoKg = n.PesoKg,
                    FecProduccion = n.FecProduccion,
                })
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.dataList = list;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetStockGeneralDetailedExcel(ItemsStockGeneralViewFilterEntity value)
        {
            var ms = new MemoryStream();
            var resultadoTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    DocumentFormat.OpenXml.Packaging.WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                    DocumentFormat.OpenXml.Packaging.WorksheetPart worksheetPart = workbookPart.AddNewPart<DocumentFormat.OpenXml.Packaging.WorksheetPart>();
                    worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());
                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Stock General Detallado" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData = worksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

                    //Cabecera
                    DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    row.Append
                    (
                        ExportToExcel.ConstructCell("Código de Artículo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Nombre de Artículo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Código de Almacén", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Nombre de Almacén", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("UM", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Stock", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Comprometido", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Solicitado", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Disponible", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Peso Kg", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Peso Promedio Kg", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell("Fecha de Producción", DocumentFormat.OpenXml.Spreadsheet.CellValues.String)
                    );
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListStockGeneralDetailed(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        row.Append
                        (
                            ExportToExcel.ConstructCell(item.ItemCode, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                            ExportToExcel.ConstructCell(item.ItemName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                            ExportToExcel.ConstructCell(item.WhsCode, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                            ExportToExcel.ConstructCell(item.WhsName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                            ExportToExcel.ConstructCell(item.InvntryUom, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                            ExportToExcel.ConstructCell(item.OnHand.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.IsCommited.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.OnOrder.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.Available.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.PesoKg.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                            ExportToExcel.ConstructCell(item.FecProduccion == null ? "" : Convert.ToDateTime(item.FecProduccion).ToString("dd/MM/yyyy"), DocumentFormat.OpenXml.Spreadsheet.CellValues.String)
                        );
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


        public async Task<ResultadoTransaccionEntity<ArticuloVentaByGrupoSubGrupoEstado>> GetListArticuloVentaByGrupoSubGrupoEstado(FilterRequestEntity value)
        {
            var response = new List<ArticuloVentaByGrupoSubGrupoEstado>();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloVentaByGrupoSubGrupoEstado>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_conSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_ARTICULO_VENTA_GRUPO_SUBGRUPO_ESTADO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Grupo", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@SubGrupo", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@SubGrupo2", value.Cod3));
                        cmd.Parameters.Add(new SqlParameter("@Estado", value.Cod4));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<ArticuloVentaByGrupoSubGrupoEstado>)context.ConvertTo<ArticuloVentaByGrupoSubGrupoEstado>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetArticuloVentaExcelByGrupoSubGrupoEstado(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    DocumentFormat.OpenXml.Packaging.WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                    DocumentFormat.OpenXml.Packaging.WorksheetPart worksheetPart = workbookPart.AddNewPart<DocumentFormat.OpenXml.Packaging.WorksheetPart>();
                    worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());
                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Stock de Articulo de Venta" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData = worksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

                    //Cabecera
                    DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código de Artículo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Grupo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo 2", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Estado", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("UM", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Peso Item", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Peso Promedio Kg", DocumentFormat.OpenXml.Spreadsheet.CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListArticuloVentaByGrupoSubGrupoEstado(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.ItemCode, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomGrupo, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo2, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomEstado, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.UnidadVenta, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.PesoItem.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number));
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


        public async Task<ResultadoTransaccionEntity<ArticuloVentaStockByGrupoSubGrupo>> GetListArticuloVentaStockByGrupoSubGrupo(FilterRequestEntity value)
        {
            var response = new List<ArticuloVentaStockByGrupoSubGrupo>();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloVentaStockByGrupoSubGrupo>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_conSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_ARTICULO_VENTA_STOCK_BY_GRUPO_SUBGRUPO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Grupo", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@SubGrupo", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@SubGrupo2", value.Cod3));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<ArticuloVentaStockByGrupoSubGrupo>)context.ConvertTo<ArticuloVentaStockByGrupoSubGrupo>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetArticuloVentaStockExcelByGrupoSubGrupo(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    DocumentFormat.OpenXml.Packaging.WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                    DocumentFormat.OpenXml.Packaging.WorksheetPart worksheetPart = workbookPart.AddNewPart<DocumentFormat.OpenXml.Packaging.WorksheetPart>();
                    worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());
                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Artículos de venta - Stock" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData = worksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

                    //Cabecera
                    DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código de Artículo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Grupo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("NomGrupo 2", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("UM", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Stock", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Comprometido", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Solicitado", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Disponible", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Peso Promedio Kg", DocumentFormat.OpenXml.Spreadsheet.CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListArticuloVentaStockByGrupoSubGrupo(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.ItemCode, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomGrupo, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo2, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.UnidadVenta, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.Stock.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.Comprometido.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.Solicitado.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.Disponible.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number));
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


        public async Task<ResultadoTransaccionEntity<ArticuloReporteEntity>> GetListArticuloByGrupoSubGrupoFiltro(FilterRequestEntity value)
        {
            var response = new List<ArticuloReporteEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloReporteEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_conSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_ARTICULO_BY_GRUPO_SUBGRUPO_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@ExcluirInactivo", value.Val1));
                        cmd.Parameters.Add(new SqlParameter("@ExcluirSinStock", value.Val2));
                        cmd.Parameters.Add(new SqlParameter("@InvntItem", value.Val4));
                        cmd.Parameters.Add(new SqlParameter("@SellItem", value.Val3));
                        cmd.Parameters.Add(new SqlParameter("@PrchseItem", value.Val5));
                        cmd.Parameters.Add(new SqlParameter("@Grupo", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@SubGrupo", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@SubGrupo2", value.Cod3));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<ArticuloReporteEntity>)context.ConvertTo<ArticuloReporteEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetListArticuloExcelByGrupoSubGrupoFiltro(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    DocumentFormat.OpenXml.Packaging.WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                    DocumentFormat.OpenXml.Packaging.WorksheetPart worksheetPart = workbookPart.AddNewPart<DocumentFormat.OpenXml.Packaging.WorksheetPart>();
                    worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());
                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Artículos por grupo - subgrupo" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData = worksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

                    //Cabecera
                    DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Descripción", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Estado", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Grupo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("NomGrupo 2", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("UM", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Stock", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Comprometido", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Solicitado", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Disponible", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Peso", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Peso Promedio Kg", DocumentFormat.OpenXml.Spreadsheet.CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListArticuloByGrupoSubGrupoFiltro(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.ItemCode, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.StatusName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomGrupo, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo2, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.SalUnitMsr, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.OnHand.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.IsCommited.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.OnOrder.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.Available.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoArticulo.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number));
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


        public async Task<ResultadoTransaccionEntity<MovimientoStockByFechaSedeEntity>> GetListMovimientoStockByFechaSede(ArticuloMovimientoStockFindEntity value)
        {
            var response = new List<MovimientoStockByFechaSedeEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<MovimientoStockByFechaSedeEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_conSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_MOVIMIENTO_STOCK_BY_FECHA_SEDE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@StartDate", value.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", value.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@Location", value.Location));
                        cmd.Parameters.Add(new SqlParameter("@TypeMovement", value.TypeMovement));
                        cmd.Parameters.Add(new SqlParameter("@Customer", value.Customer));
                        cmd.Parameters.Add(new SqlParameter("@Item", value.Item));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<MovimientoStockByFechaSedeEntity>)context.ConvertTo<MovimientoStockByFechaSedeEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetMovimientoStockExcelByFechaSede(ArticuloMovimientoStockFindEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    DocumentFormat.OpenXml.Packaging.WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                    DocumentFormat.OpenXml.Packaging.WorksheetPart worksheetPart = workbookPart.AddNewPart<DocumentFormat.OpenXml.Packaging.WorksheetPart>();
                    worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());
                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Movimiento de Stock" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData = worksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

                    //Cabecera
                    DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Tipo de Movimiento", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Número de Guía SAP", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Número de Guía SUNAT", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de Guía", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Código de Cliente", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Cliente", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Usuario", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Código de Artículo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Sede", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Centro de Costo", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Almacén de Origen", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Almacén de Destino", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Bultos", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Total Kg", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("UM", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de Pedido", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Número de Pedido", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Número de Fctura SAP", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Número de Fctura SUNAT", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Transportista", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("RUC de Transportista", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Placa de Transportista", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Conductor", DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                    ExportToExcel.ConstructCell("Lincencia de Conductor", DocumentFormat.OpenXml.Spreadsheet.CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListMovimientoStockByFechaSede(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.NomTipoMovimiento, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroGuiaSAP.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroGuiaSUNAT, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.DocDate.ToString("dd/MM/yyyy"), DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.CardCode, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.CardName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.Usuario, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemCode, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.Sede, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.CentroCosto, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.AlmacenOrigen, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.AlmacenDestino, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.Bulto.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalKg.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.UnidadMedida, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.Quantity.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroPedido == null ? null : item.NumeroPedido.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.FechaPedido == null ? null : Convert.ToDateTime(item.FechaPedido).ToString("dd/MM/yyyy"), DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroFcturaSAP == null ? null : item.NumeroFcturaSAP.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroFcturaSUNAT, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomTransportista, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.RucTransportista, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.PlacaTransportista, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.NomConductor, DocumentFormat.OpenXml.Spreadsheet.CellValues.String),
                        ExportToExcel.ConstructCell(item.LincenciaConductor, DocumentFormat.OpenXml.Spreadsheet.CellValues.String));
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
        

        public async Task<ResultadoTransaccionEntity<ArticuloForSodimacBySkuItemEntity>> GetArticuloForOrdenVentaSodimacBySku(ArticuloSodimacBySkuEntity value)
        {
            var linea = 1;
            var articulo = new ItemsEntity();
            var response = new List<ArticuloForSodimacBySkuItemEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloForSodimacBySkuItemEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_conSap))
                {
                    conn.Open();

                    var listSku = value.Linea.Select(x => x.Sku).Distinct().ToList();

                    foreach (var sku in listSku)
                    {
                        using (SqlCommand cmd = new SqlCommand(SP_GET_FOR_ORDEN_VENTA_SODIMAC_SKU, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add(new SqlParameter("@Sku", sku));

                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                articulo = context.Convert<ItemsEntity>(reader);
                            }
                        }

                        if (articulo == null)
                        {
                            resultTransaccion.IdRegistro = -1;
                            resultTransaccion.ResultadoCodigo = -1;
                            resultTransaccion.ResultadoDescripcion = "El SKU " + sku + " no existe en SAP Business One.";
                            return resultTransaccion;
                        }

                        foreach (var item in value.Linea)
                        {
                            if (item.Sku == sku)
                            {
                                item.ItemCode = articulo.ItemCode;
                                item.Dscription = articulo.ItemName;
                            }
                        }
                    }

                    _= value.Linea.OrderBy(static x => x.Line2).ToList();

                    foreach (var item in value.Linea)
                    {
                        item.Line1 = linea;
                        linea++;
                    }

                    _= value.Linea.OrderBy(static x => x.Line1).ToList();

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", value.Linea.Count);
                    resultTransaccion.dataList = value.Linea;
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


        public async Task<ResultadoTransaccionEntity<ArticuloDocumentoEntity>> GetArticuloVentaByCode(FilterRequestEntity value)
        {
            var response = new ArticuloDocumentoEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloDocumentoEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_conSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_ARTICULO_VENTA_BY_CODE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@CardCode", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@Currency", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@ItemCode", value.Cod3));
                        cmd.Parameters.Add(new SqlParameter("@SlpCode", value.Id1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<ArticuloDocumentoEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
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


        public async Task<ResultadoTransaccionEntity<ItemsEntity>> SetCreateMassive(ItemsCreateMassiveEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ItemsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Items items = null;
            Company company = null;
            Documents entrada = null;
            Documents salida = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Empieza la transacción en SAP
                    if (!company.InTransaction) company.StartTransaction();


                    #region <<< SALIDA DE MERCANCIAS PARA AJUSTE DE INVENTARIO >>>

                    if(value.IsSalida)
                    {
                        var listaItemsOrigen = value.Line
                    .GroupBy(n => new { n.U_FIB_ItemCode, n.U_FIB_ItemName, n.DfltWH })
                    .Select(g => new ItemsEntity
                    {
                        U_FIB_ItemCode = g.Key.U_FIB_ItemCode,
                        U_FIB_ItemName = g.Key.U_FIB_ItemName,
                        DfltWH = g.Key.DfltWH,
                        OnHand = g.Sum(x => x.OnHand ?? 0)
                    })
                    .ToList();

                        //Se crea el objeto de salida de mercancías
                        salida = company.GetBusinessObject(BoObjectTypes.oInventoryGenExit);

                        salida.DocDate = DateTime.Now;
                        salida.TaxDate = DateTime.Now;

                        foreach (var articulo in listaItemsOrigen)
                        {
                            salida.Lines.ItemCode = articulo.U_FIB_ItemCode;
                            salida.Lines.ItemDescription = articulo.U_FIB_ItemName;
                            salida.Lines.WarehouseCode = articulo.DfltWH;
                            salida.Lines.CostingCode = "CH-TLCIR";
                            salida.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = "28";
                            salida.Lines.Quantity = Convert.ToDouble(articulo.OnHand ?? 0);
                            salida.Lines.Add();
                        }

                        var regSal = salida.Add();

                        if (regSal == 0)
                        {
                            resultTransaccion.IdRegistro = 0;
                            resultTransaccion.ResultadoCodigo = 0;
                            resultTransaccion.ResultadoDescripcion = "Entrada de mercancías registrada con éxito.";
                        }
                        else
                        {
                            company.GetLastError(out int errorCode, out string errorMessage);
                            throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                        }
                    }

                    #endregion



                    #region <<< REGISTRO DE ARTÍCULOS >>>

                    
                    // Se crea el objeto de la items
                    items = company.GetBusinessObject(BoObjectTypes.oItems);

                    foreach (var articulo in value.Line)
                    {
                        items.ItemCode = articulo.ItemCode;
                        items.ItemName = articulo.ItemName;
                        items.ItemsGroupCode = articulo.ItmsGrpCod;

                        if (articulo.InvntItem == "Y")
                            items.InventoryItem = BoYesNoEnum.tYES;
                        else
                            items.InventoryItem = BoYesNoEnum.tNO;
                        if (articulo.SellItem == "Y")
                            items.SalesItem = BoYesNoEnum.tYES;
                        else
                            items.SalesItem = BoYesNoEnum.tNO;
                        if (articulo.PrchseItem == "Y")
                            items.PurchaseItem = BoYesNoEnum.tYES;
                        else
                            items.PurchaseItem = BoYesNoEnum.tNO;
                        if (articulo.WTLiable == "Y")
                            items.WTLiable = BoYesNoEnum.tYES;
                        else
                            items.WTLiable = BoYesNoEnum.tNO;
                        if (articulo.VatLiable == "Y")
                            items.VatLiable = BoYesNoEnum.tYES;
                        else
                            items.VatLiable = BoYesNoEnum.tNO;
                        if (articulo.IndirctTax == "Y")
                            items.IndirectTax = BoYesNoEnum.tYES;
                        else
                            items.IndirectTax = BoYesNoEnum.tNO;

                        items.SalesUnit = articulo.SalUnitMsr;
                        items.PurchaseUnit = articulo.BuyUnitMsr;
                        items.InventoryUOM = articulo.InvntryUom;

                        items.DefaultWarehouse = articulo.DfltWH;

                        items.ArTaxCode = articulo.TaxCodeAR;

                        if (articulo.U_FIB_ItemCode != null) items.UserFields.Fields.Item("U_FIB_ItemCode").Value = articulo.U_FIB_ItemCode;
                        if (articulo.U_FIB_ItemName != null) items.UserFields.Fields.Item("U_FIB_ItemName").Value = articulo.U_FIB_ItemName;
                        if (articulo.U_BPP_TIPEXIST != null) items.UserFields.Fields.Item("U_BPP_TIPEXIST").Value = articulo.U_BPP_TIPEXIST;
                        if (articulo.U_BPP_TIPUNMED != null) items.UserFields.Fields.Item("U_BPP_TIPUNMED").Value = articulo.U_BPP_TIPUNMED;
                        if (articulo.U_S_PartAranc1 != null) items.UserFields.Fields.Item("U_S_PartAranc1").Value = articulo.U_S_PartAranc1;
                        if (articulo.U_S_PartAranc2 != null) items.UserFields.Fields.Item("U_S_PartAranc2").Value = articulo.U_S_PartAranc2;
                        if (articulo.U_FIB_ECU != null) items.UserFields.Fields.Item("U_FIB_ECU").Value = articulo.U_FIB_ECU;
                        if (articulo.U_S_CCosto != null) items.UserFields.Fields.Item("U_S_CCosto").Value = articulo.U_S_CCosto;
                        if (articulo.U_FIB_PESO != null) items.UserFields.Fields.Item("U_FIB_PESO").Value = Convert.ToDouble(articulo.U_FIB_PESO ?? 0m);
                        if (articulo.U_FIB_SGRUP != null) items.UserFields.Fields.Item("U_FIB_SGRUP").Value = articulo.U_FIB_SGRUP;
                        if (articulo.U_FIB_SGRUPO2 != null) items.UserFields.Fields.Item("U_FIB_SGRUPO2").Value = articulo.U_FIB_SGRUPO2;
                        if (articulo.U_FIB_LINNEG != null) items.UserFields.Fields.Item("U_FIB_LINNEG").Value = articulo.U_FIB_LINNEG;
                        if (articulo.U_UsrCreate != null) items.UserFields.Fields.Item("U_UsrCreate").Value = articulo.U_UsrCreate;

                        var regItem = items.Add();

                        if (regItem == 0)
                        {
                            resultTransaccion.IdRegistro = 0;
                            resultTransaccion.ResultadoCodigo = 0;
                            resultTransaccion.ResultadoDescripcion = "Artículo registrado con éxito.";
                        }
                        else
                        {
                            company.GetLastError(out int errorCode, out string errorMessage);
                            throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                        }
                    }

                    #endregion



                    #region <<< ENTRADA DE MERCANCÍAS POR ARTÍCULO >>>

                    if (value.IsEntrada)
                    {
                        var listGroup = value.Line
                   .GroupBy(n => n.U_FIB_ItemCode)
                   .Select(g => g.First())
                   .Select(n => new ItemsEntity { U_FIB_ItemCode = n.U_FIB_ItemCode })
                   .ToList();

                        foreach (var articuloGroup in listGroup)
                        {
                            var lista = value.Line.Where(x => x.U_FIB_ItemCode == articuloGroup.U_FIB_ItemCode).ToList();

                            // Se crea el objeto de la entrada de mercancías
                            entrada = company.GetBusinessObject(BoObjectTypes.oInventoryGenEntry);

                            entrada.DocDate = DateTime.Now;
                            entrada.TaxDate = DateTime.Now;

                            foreach (var articulo in lista)
                            {
                                entrada.Lines.ItemCode = articulo.ItemCode;
                                entrada.Lines.ItemDescription = articulo.ItemName;
                                entrada.Lines.WarehouseCode = articulo.DfltWH;
                                salida.Lines.CostingCode = "CH-TLCIR";
                                entrada.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = "28";
                                entrada.Lines.Quantity = Convert.ToDouble(articulo.OnHand ?? 0);
                                entrada.Lines.Add();
                            }

                            var regEnt = entrada.Add();

                            if (regEnt == 0)
                            {
                                resultTransaccion.IdRegistro = 0;
                                resultTransaccion.ResultadoCodigo = 0;
                                resultTransaccion.ResultadoDescripcion = "Entrada de mercancías registrada con éxito.";
                            }
                            else
                            {
                                company.GetLastError(out int errorCode, out string errorMessage);
                                throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                            }
                        }
                    }

                    #endregion


                    // Se finaliza la transacción en SAP
                    if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_Commit);
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
                    _companyProviderSap.LiberarObjetosCOM(items, entrada, salida);
                }

                return resultTransaccion;
            });
        }
    }
}
