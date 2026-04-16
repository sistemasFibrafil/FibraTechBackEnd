using System;
using System.IO;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Transactions;
using System.Data.SqlClient;
using Net.Business.Entities;
using DocumentFormat.OpenXml;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class TakeInventoryFinishedProductsRepository : RepositoryBase<TakeInventoryFinishedProductsEntity>, ITakeInventoryFinishedProductsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "INV_GetListTomaInventario";
        const string SP_INSERT = DB_ESQUEMA + "INV_SetTomaInventarioCreate";

        private readonly DataContextSeguridad _dbSeg;
        private readonly DataContextSAPBusinessOne _dbSap;

        public TakeInventoryFinishedProductsRepository(IConnectionSQL context, IConfiguration configuration, DataContextSeguridad dbSeg, DataContextSAPBusinessOne dbSap, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _dbSeg = dbSeg;
            _dbSap = dbSap;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>> GetListByFilter(TakeInventoryFinishedProductsFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query =
                from n in _dbSeg.TakeInventoryFinishedProducts
                join d in _dbSeg.TakeInventoryFinishedProducts1 on n.DocEntry equals d.DocEntry
                where d.IsDelete == "N" && d.CreateDate >= value.StartDate && d.CreateDate <= value.EndDate
                select new
                {
                    n.DocEntry,
                    n.ItemCode,
                    n.Dscription,
                    n.WhsCode,
                    n.UnitMsr,
                    n.OnHandSys,
                    d.Quantity,
                    d.WeightKg,
                    d.UsrCreate
                };

                
                // FILTRO POR USUARIO
                if (!string.IsNullOrWhiteSpace(value.Usuario))
                {
                    var usuarios = value.Usuario.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    query = query.Where(x => usuarios.Contains(x.UsrCreate));
                }


                // FILTRO POR ALMACÉN
                if (!string.IsNullOrWhiteSpace(value.WhsCode))
                {
                    var warehouses = value.WhsCode.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(x => warehouses.Contains(x.WhsCode));
                }


                // FILTRO POR ITEM
                if (!string.IsNullOrWhiteSpace(value.Item))
                {
                    var filter = value.Item.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(EF.Functions.Collate(x.ItemCode!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.Dscription!, GlobalVariables.CI), $"%{filter}%")
                    );
                }


                var list = await query
                .GroupBy(x => new
                {
                    x.DocEntry,
                    x.ItemCode,
                    x.Dscription,
                    x.WhsCode,
                    x.UnitMsr,
                    x.OnHandSys
                })
                .Select(g => new
                {
                    g.Key,
                    SumQty = g.Sum(x => x.Quantity),
                    SumKg = g.Sum(x => x.WeightKg)
                })
                .Select(x => new TakeInventoryFinishedProductsQueryEntity
                {
                    DocEntry = x.Key.DocEntry,
                    ItemCode = x.Key.ItemCode,
                    Dscription = x.Key.Dscription,
                    WhsCode = x.Key.WhsCode,
                    UnitMsr = x.Key.UnitMsr,
                    OnHandPhy = (x.Key.UnitMsr ?? "").ToUpper() == "KG" ? x.SumKg : x.SumQty,
                    OnHandSys = x.Key.OnHandSys,
                    Difference = ((x.Key.UnitMsr ?? "").ToUpper() == "KG" ? x.SumKg : x.SumQty) - x.Key.OnHandSys,
                    Quantity = x.SumQty,
                    WeightKg = x.SumKg
                })
                .OrderBy(x => x.ItemCode)
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

        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetSummaryItemExcelByFilter(TakeInventoryFinishedProductsFilterEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Resumido" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append
                    (
                        ExportToExcel.ConstructCell("Código", CellValues.String),
                        ExportToExcel.ConstructCell("Descripción", CellValues.String),
                        ExportToExcel.ConstructCell("Almacén", CellValues.String),
                        ExportToExcel.ConstructCell("UM", CellValues.String),
                        ExportToExcel.ConstructCell("Stock físico", CellValues.String),
                        ExportToExcel.ConstructCell("Stock sistema", CellValues.String),
                        ExportToExcel.ConstructCell("Diferencia", CellValues.String),
                        ExportToExcel.ConstructCell("Cantidad", CellValues.String),
                        ExportToExcel.ConstructCell("Peso (Kg)", CellValues.String)
                    );
                    sheetData.AppendChild(row);

                    var query =
                    from n in _dbSeg.TakeInventoryFinishedProducts
                    join d in _dbSeg.TakeInventoryFinishedProducts1 on n.DocEntry equals d.DocEntry
                    where n.IsDelete == "N" && n.CreateDate >= value.StartDate && n.CreateDate <= value.EndDate
                    select new
                    {
                        n.ItemCode,
                        n.Dscription,
                        n.WhsCode,
                        n.UnitMsr,
                        n.OnHandSys,
                        d.Quantity,
                        d.WeightKg,
                        d.UsrCreate
                    };


                    // FILTRO POR USUARIO
                    if (!string.IsNullOrWhiteSpace(value.Usuario))
                    {
                        var usuarios = value.Usuario.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                        query = query.Where(x => usuarios.Contains(x.UsrCreate));
                    }


                    // FILTRO POR ALMACÉN
                    if (!string.IsNullOrWhiteSpace(value.WhsCode))
                    {
                        var warehouses = value.WhsCode.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                        query = query.Where(x => warehouses.Contains(x.WhsCode));
                    }


                    // FILTRO POR ITEM
                    if (!string.IsNullOrWhiteSpace(value.Item))
                    {
                        var filter = value.Item.Trim();

                        query = query.Where(x =>
                            EF.Functions.Like(EF.Functions.Collate(x.ItemCode!, GlobalVariables.CI), $"%{filter}%") ||
                            EF.Functions.Like(EF.Functions.Collate(x.Dscription!, GlobalVariables.CI), $"%{filter}%")
                        );
                    }


                    var list = await query
                    .GroupBy(x => new
                    {
                        x.ItemCode,
                        x.Dscription,
                        x.WhsCode,
                        x.UnitMsr,
                        x.OnHandSys
                    })
                    .Select(g => new
                    {
                        g.Key,
                        SumQty = g.Sum(x => x.Quantity),
                        SumKg = g.Sum(x => x.WeightKg)
                    })
                    .Select(x => new TakeInventoryFinishedProductsQueryEntity
                    {
                        ItemCode = x.Key.ItemCode,
                        Dscription = x.Key.Dscription,
                        WhsCode = x.Key.WhsCode,
                        UnitMsr = x.Key.UnitMsr,
                        OnHandPhy = (x.Key.UnitMsr ?? "").ToUpper() == "KG" ? x.SumKg : x.SumQty,
                        OnHandSys = x.Key.OnHandSys,
                        Difference = ((x.Key.UnitMsr ?? "").ToUpper() == "KG" ? x.SumKg : x.SumQty) - x.Key.OnHandSys,
                        Quantity = x.SumQty,
                        WeightKg = x.SumKg
                    })
                    .OrderBy(x => x.ItemCode)
                    .ToListAsync();


                    //Contenido
                    foreach (var item in list)
                    {
                        row = new Row();
                        row.Append
                        (
                            ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                            ExportToExcel.ConstructCell(item.Dscription, CellValues.String),
                            ExportToExcel.ConstructCell(item.WhsCode, CellValues.String),
                            ExportToExcel.ConstructCell(item.UnitMsr, CellValues.String),
                            ExportToExcel.ConstructCell(item.OnHandPhy.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.OnHandSys.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.Difference.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.Quantity.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.WeightKg.ToString(), CellValues.Number)
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

        public async Task<ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>> GetSummaryUserByFilter(TakeInventoryFinishedProductsFilterEntity value)
        {
            var list = new List<TakeInventoryFinishedProductsQueryEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@StartDate", value.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", value.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@Usuario", value.Usuario));
                        cmd.Parameters.Add(new SqlParameter("@WhsCode", value.WhsCode));
                        cmd.Parameters.Add(new SqlParameter("@Item", value.Item));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new TakeInventoryFinishedProductsQueryEntity
                                {
                                    ItemCode = reader["ItemCode"]?.ToString(),
                                    Dscription = reader["Dscription"]?.ToString(),
                                    WhsCode = reader["WhsCode"]?.ToString(),
                                    UnitMsr = reader["UnitMsr"]?.ToString(),

                                    OnHandSys = reader["OnHandSys"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["OnHandSys"]),
                                    OnHandPhy = reader["OnHandPhy"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["OnHandPhy"]),
                                    Difference = reader["Difference"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Difference"]),
                                    Quantity = reader["Quantity"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Quantity"]),
                                    WeightKg = reader["Quantity"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WeightKg"]),
                                };

                                // detectar columnas dinámicas
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string col = reader.GetName(i);

                                    // columnas fijas → ignorar
                                    if (col is "ItemCode" or "Dscription" or "WhsCode" or "UnitMsr" or "OnHandSys" or "OnHandPhy" or "Difference" or "Quantity" or "WeightKg")
                                        continue;

                                    row.DynamicColumns[col] = reader[i] == DBNull.Value ? "" : reader[i];
                                }

                                list.Add(row);
                            }
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
                    resultTransaccion.dataList = list;
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

        private Cell CreateCell(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue("")
                };
            }

            // Detectar numérico
            if (decimal.TryParse(
                    value.ToString(),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal num))
            {
                return new Cell
                {
                    DataType = CellValues.Number,
                    CellValue = new CellValue(num.ToString(System.Globalization.CultureInfo.InvariantCulture))
                };
            }

            // Texto normal
            return new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(value.ToString())
            };
        }

        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetSummaryUserExcelByFilter(TakeInventoryFinishedProductsFilterEntity value)
        {
            var ms = new MemoryStream(); // ❗ YA NO usar "using"
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook);

                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1,Name = "Resumido - usuario" };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();


                // Obtener datos
                var data = await GetSummaryUserByFilter(value);

                //------------------------------------------------------
                //  ENCABEZADOS ESTÁTICOS RENOMBRADOS
                //------------------------------------------------------
                Dictionary<string, string> staticHeaderNames = new Dictionary<string, string>
                {
                    { "ItemCode", "Código" },
                    { "Dscription", "Descripción" },
                    { "WhsCode", "Almacén" },
                    { "UnitMsr", "UM" },
                    { "OnHandPhy", "Stock Físico" },
                    { "OnHandSys", "Stock Sistema" },
                    { "Difference", "Diferencia" },
                    { "Quantity", "Cantidad" },
                    { "WeightKg", "Peso (Kg)" }
                };

                string[] staticCols =
                {
                    "ItemCode","Dscription","WhsCode","UnitMsr",
                    "OnHandPhy","OnHandSys","Difference","Quantity","WeightKg"
                };

                var header = new Row();

                foreach (var col in staticCols)
                    header.Append(CreateCell(staticHeaderNames[col]));

                //------------------------------------------------------
                //  COLUMNAS DINÁMICAS (sin cambiar su nombre)
                //------------------------------------------------------
                var dynamicCols = data.dataList
                    .SelectMany(r => r.DynamicColumns.Keys)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                foreach (var col in dynamicCols)
                    header.Append(CreateCell(col));

                sheetData.Append(header);


                //------------------------------------------------------
                //  CONTENIDO
                //------------------------------------------------------
                foreach (var item in data.dataList)
                {
                    var row = new Row();

                    row.Append(CreateCell(item.ItemCode));
                    row.Append(CreateCell(item.Dscription));
                    row.Append(CreateCell(item.WhsCode));
                    row.Append(CreateCell(item.UnitMsr));
                    row.Append(CreateCell(item.OnHandPhy));
                    row.Append(CreateCell(item.OnHandSys));
                    row.Append(CreateCell(item.Difference));
                    row.Append(CreateCell(item.Quantity));
                    row.Append(CreateCell(item.WeightKg));

                    foreach (var col in dynamicCols)
                    {
                        item.DynamicColumns.TryGetValue(col, out object valor);
                        row.Append(CreateCell(valor ?? 0));
                    }

                    sheetData.Append(row);
                }

                worksheetPart.Worksheet.Save();

                document.Dispose();   // cerrar OpenXML correctamente

                ms.Position = 0;      // Poner al inicio

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                resultTransaccion.data = ms;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }        

        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetDetailedExcelByFilter(TakeInventoryFinishedProductsFilterEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                using (var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Detallado" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append
                    (
                        ExportToExcel.ConstructCell("Código", CellValues.String),
                        ExportToExcel.ConstructCell("Descripción", CellValues.String),
                        ExportToExcel.ConstructCell("Almacén", CellValues.String),
                        ExportToExcel.ConstructCell("Código de barras", CellValues.String),
                        ExportToExcel.ConstructCell("Fecha de produccipon", CellValues.String),
                        ExportToExcel.ConstructCell("UM", CellValues.String),
                        ExportToExcel.ConstructCell("Peso (Kg)", CellValues.String),
                        ExportToExcel.ConstructCell("Usuario", CellValues.String),
                        ExportToExcel.ConstructCell("Fecha de registro", CellValues.String),
                        ExportToExcel.ConstructCell("Hora de registro", CellValues.String)
                    );
                    sheetData.AppendChild(row);


                    var query =
                    from n in _dbSeg.TakeInventoryFinishedProducts
                    join d in _dbSeg.TakeInventoryFinishedProducts1 on n.DocEntry equals d.DocEntry
                    join e in _dbSeg.Usuario on d.UsrCreate equals e.IdUsuario
                    where n.IsDelete == "N" && n.CreateDate >= value.StartDate && n.CreateDate <= value.EndDate
                    select new
                    {
                        n.ItemCode,
                        n.Dscription,
                        n.WhsCode,
                        d.CodeBar,
                        d.ProductionDate,
                        n.UnitMsr,
                        d.WeightKg,
                        d.UsrCreate,
                        UsrNameCreate = e.Nombre + " " + e.ApellidoPaterno,
                        d.CreateDate,
                        d.CreateTime,
                    };


                    // FILTRO POR USUARIO
                    if (!string.IsNullOrWhiteSpace(value.Usuario))
                    {
                        var usuarios = value.Usuario.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                        query = query.Where(x => usuarios.Contains(x.UsrCreate));
                    }


                    // FILTRO POR ALMACÉN
                    if (!string.IsNullOrWhiteSpace(value.WhsCode))
                    {
                        var warehouses = value.WhsCode.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                        query = query.Where(x => warehouses.Contains(x.WhsCode));
                    }


                    // FILTRO POR ITEM
                    if (!string.IsNullOrWhiteSpace(value.Item))
                    {
                        var filter = value.Item.Trim();

                        query = query.Where(x =>
                            EF.Functions.Like(EF.Functions.Collate(x.ItemCode!, GlobalVariables.CI), $"%{filter}%") ||
                            EF.Functions.Like(EF.Functions.Collate(x.Dscription!, GlobalVariables.CI), $"%{filter}%")
                        );
                    }

                    var list = await query
                    .Select(x => new TakeInventoryFinishedProductsQueryEntity
                    {
                        ItemCode = x.ItemCode,
                        Dscription = x.Dscription,
                        WhsCode = x.WhsCode,
                        CodeBar = x.CodeBar,
                        ProductionDate = x.ProductionDate,
                        UnitMsr = x.UnitMsr,
                        WeightKg = x.WeightKg,
                        UsrNameCreate = x.UsrNameCreate,
                        CreateDate = x.CreateDate,
                        CreateTime = x.CreateTime
                    })
                    .OrderBy(x => x.ItemCode)
                    .ThenBy(x => x.CreateDate)
                    .ThenBy(x => x.CreateTime)
                    .ToListAsync();


                    //Contenido
                    foreach (var item in list)
                    {
                        row = new Row();
                        row.Append
                        (
                            ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                            ExportToExcel.ConstructCell(item.Dscription, CellValues.String),
                            ExportToExcel.ConstructCell(item.WhsCode, CellValues.String),
                            ExportToExcel.ConstructCell(item.CodeBar, CellValues.String),
                            ExportToExcel.ConstructCell(item.ProductionDate == null? "" : Convert.ToDateTime(item.ProductionDate).ToString("dd/MM/yyyy"), CellValues.String),
                            ExportToExcel.ConstructCell(item.UnitMsr, CellValues.String),
                            ExportToExcel.ConstructCell(item.WeightKg.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.UsrNameCreate, CellValues.String),
                            ExportToExcel.ConstructCell(item.CreateDate.ToString("dd/MM/yyyy"), CellValues.String),
                            ExportToExcel.ConstructCell(item.CreateTime.ToString("D4").Substring(0, 2) + ":" + item.CreateTime.ToString("D4").Substring(2, 2), CellValues.String)
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

        public async Task<ResultadoTransaccionEntity<TakeInventoryFinishedProducts1Entity>> GetListByItemCode(TakeInventoryFinishedProductsModalFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TakeInventoryFinishedProducts1Entity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _dbSeg.TakeInventoryFinishedProducts1.Where(n => n.IsDelete == "N" && n.DocEntry == value.DocEntry);

                if (!string.IsNullOrWhiteSpace(value.CodeBar))
                {
                    query = query.Where(n => n.CodeBar.Contains(value.CodeBar));
                }


                var list = await query
                .Select(n => new TakeInventoryFinishedProducts1Entity
                {
                    DocEntry = n.DocEntry,
                    LineId = n.LineId,
                    CodeBar = n.CodeBar,
                    ProductionDate = n.ProductionDate,
                    WeightKg = n.WeightKg
                })
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

        public async Task<ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>> GetListCurrentDate(TakeInventoryFinishedProductsFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _dbSeg.TakeInventoryFinishedProducts
                .Join(
                    _dbSeg.TakeInventoryFinishedProducts1,
                    n => n.DocEntry,
                    m => m.DocEntry,
                    (n, m) => new { n, m }
                )
                .Where(x => x.m.IsDelete == "N" && x.n.WhsCode == value.WhsCode && x.m.UsrCreate == value.UsrCreate && x.m.CreateDate == value.CreateDate)
                .GroupBy(x => new
                {
                    x.n.ItemCode,
                    x.n.Dscription,
                    x.n.WhsCode,
                    x.n.UnitMsr
                })
                .Select(g => new TakeInventoryFinishedProductsQueryEntity
                {
                    ItemCode = g.Key.ItemCode,
                    Dscription = g.Key.Dscription,
                    WhsCode = g.Key.WhsCode,
                    UnitMsr = g.Key.UnitMsr,
                    Quantity = g.Sum(p => p.m.Quantity),
                    WeightKg = g.Sum(p => p.m.WeightKg),
                })
                .OrderBy(x => x.ItemCode)
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

        public async Task<ResultadoTransaccionEntity<InventoryTransferRequestQueryEntity>> GetToCopy(TakeInventoryFinishedProductsToCopyFindEntity value)
        {
            var data = new InventoryTransferRequestQueryEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<InventoryTransferRequestQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query =
                from n in _dbSeg.TakeInventoryFinishedProducts
                join d in _dbSeg.TakeInventoryFinishedProducts1 on n.DocEntry equals d.DocEntry
                where d.IsDelete == "N" && d.CreateDate >= value.StartDate && d.CreateDate <= value.EndDate
                select new
                {
                    n.DocEntry,
                    n.ItemCode,
                    n.Dscription,
                    d.CodeBar,
                    n.WhsCode,
                    n.UnitMsr,
                    d.Quantity,
                    d.WeightKg,
                    d.UsrCreate
                };


                // FILTRO POR USUARIO
                if (!string.IsNullOrWhiteSpace(value.Usuario))
                {
                    var usuarios = value.Usuario.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    query = query.Where(x => usuarios.Contains(x.UsrCreate));
                }


                // FILTRO POR ALMACÉN
                if (!string.IsNullOrWhiteSpace(value.WhsCode))
                {
                    var warehouses = value.WhsCode.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(x => warehouses.Contains(x.WhsCode));
                }


                // FILTRO POR ITEM
                if (!string.IsNullOrWhiteSpace(value.Item))
                {
                    var filter = value.Item.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(EF.Functions.Collate(x.ItemCode!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.Dscription!, GlobalVariables.CI), $"%{filter}%")
                    );
                }

                // FILTRO POR CÓDIGO DE ARTÍCULO
                if (!string.IsNullOrWhiteSpace(value.ItemCode))
                {
                    var itemCode = value.ItemCode.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(n => itemCode.Contains(n.ItemCode));
                }


                // Se obtiene el tipo de operación por defecto "11"
                var tipoOperacion = await _dbSap.OperationType.Where(t => t.Code == "11").FirstOrDefaultAsync();


                var list = await query
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.Dscription,
                    x.WhsCode,
                    x.UnitMsr
                })
                .Select(g => new
                {
                    g.Key,
                    SumQty = g.Sum(x => x.Quantity),
                    SumKg = g.Sum(x => x.WeightKg)
                })
                .Select(x => new InventoryTransferRequest1QueryEntity
                {
                    ItemCode = x.Key.ItemCode,
                    Dscription = x.Key.Dscription,
                    FromWhsCod = x.Key.WhsCode,
                    WhsCode = x.Key.WhsCode,
                    U_tipoOpT12 = tipoOperacion.Code ?? "",
                    U_tipoOpT12Nam = tipoOperacion.U_descrp ?? "",
                    UnitMsr = x.Key.UnitMsr,
                    Quantity = (x.Key.UnitMsr ?? "KG").ToUpper() == "KG" ? x.SumKg : x.SumQty,
                    OpenQty = (x.Key.UnitMsr ?? "KG").ToUpper() == "KG" ? x.SumKg : x.SumQty,
                    U_FIB_OpQtyPkg = (x.Key.UnitMsr ?? "KG").ToUpper() == "KG" ? x.SumKg : x.SumQty,
                    U_FIB_LinStPkg = "C"
                })
                .OrderBy(x => x.ItemCode)
                .ToListAsync();


                var PickingList = await query
                .Select(n => new PickingEntity
                {
                    U_ItemCode = n.ItemCode,
                    U_Dscription = n.Dscription,
                    U_CodeBar = n.CodeBar,
                    U_FromWhsCod = n.WhsCode,
                    U_WhsCode = n.WhsCode,
                    U_UnitMsr = n.UnitMsr,
                    U_Quantity = n.Quantity,
                    U_WeightKg = n.WeightKg,
                    U_Status = "O"
                })
                .OrderBy(x => x.U_ItemCode)
                .ToListAsync();

                data.Lines = list;
                data.PickingLines = PickingList;

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
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

        public async Task<ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>> SetCreate(TakeInventoryFinishedProductsCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TakeInventoryFinishedProductsQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                using (SqlConnection conn = new(context.GetConnectionSQL()))
                {
                    using (CommittableTransaction transaction = new CommittableTransaction())
                    {
                        await conn.OpenAsync();
                        conn.EnlistTransaction(transaction);

                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@WhsCode", value.WhsCode));
                                cmd.Parameters.Add(new SqlParameter("@CodeBar", value.CodeBar));
                                cmd.Parameters.Add(new SqlParameter("@UsrCreate", value.UsrCreate));
                                cmd.Parameters.Add(new SqlParameter("@CreateDate", value.CreateDate));
                                cmd.Parameters.Add(new SqlParameter("@CreateTime", value.CreateTime));

                                await cmd.ExecuteNonQueryAsync();
                            }                            

                            transaction.Commit();


                            var list = await _dbSeg.TakeInventoryFinishedProducts
                            .Join(
                                _dbSeg.TakeInventoryFinishedProducts1,
                                n => n.DocEntry,
                                m => m.DocEntry,
                                (n, m) => new { n, m }
                            )
                            .Where(x => x.m.IsDelete == "N" && x.n.WhsCode == value.WhsCode && x.m.UsrCreate == value.UsrCreate && x.m.CreateDate == value.CreateDate)
                            .GroupBy(x => new
                            {
                                x.n.ItemCode,
                                x.n.Dscription,
                                x.n.WhsCode,
                                x.n.UnitMsr
                            })
                            .Select(g => new TakeInventoryFinishedProductsQueryEntity
                            {
                                ItemCode = g.Key.ItemCode,
                                Dscription = g.Key.Dscription,
                                WhsCode = g.Key.WhsCode,
                                UnitMsr = g.Key.UnitMsr,
                                Quantity = g.Sum(p => p.m.Quantity),
                                WeightKg = g.Sum(p => p.m.WeightKg),
                            })
                            .OrderBy(x => x.ItemCode)
                            .ToListAsync();

                            resultTransaccion.IdRegistro = 0;
                            resultTransaccion.ResultadoCodigo = 0;
                            resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
                            resultTransaccion.dataList = list;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.Split('\n')[0].Trim().ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<TakeInventoryFinishedProducts1Entity>> SetDeleteLine(TakeInventoryFinishedProducts1DeleteEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TakeInventoryFinishedProducts1Entity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            await using var transaction = await _dbSeg.Database.BeginTransactionAsync();

            try
            {
                var entityLine = await _dbSeg.TakeInventoryFinishedProducts1.FindAsync(value.DocEntry, value.LineId);

                _dbSeg.Entry(entityLine).CurrentValues.SetValues(value);
                _dbSeg.Entry(entityLine).Property(e => e.CodeBar).IsModified = false;
                _dbSeg.Entry(entityLine).Property(e => e.ProductionDate).IsModified = false;
                _dbSeg.Entry(entityLine).Property(e => e.Quantity).IsModified = false;
                _dbSeg.Entry(entityLine).Property(e => e.WeightKg).IsModified = false;
                _dbSeg.Entry(entityLine).Property(e => e.UsrCreate).IsModified = false;
                _dbSeg.Entry(entityLine).Property(e => e.CreateDate).IsModified = false;
                _dbSeg.Entry(entityLine).Property(e => e.CreateTime).IsModified = false;

                await _dbSeg.SaveChangesAsync();

                var line = await _dbSeg.TakeInventoryFinishedProducts1.Where(n => n.IsDelete == "N" &&  n.DocEntry == value.DocEntry).FirstOrDefaultAsync();

                if (line == null)
                {
                    var entidad = new TakeInventoryFinishedProductsDeleteEntity
                    {
                        DocEntry = value.DocEntry,
                        IsDelete = value.IsDelete,
                        UsrDelete = value.UsrDelete,
                        DeleteDate = value.DeleteDate,
                        DeleteTime = value.DeleteTime
                    };

                    var entity = await _dbSeg.TakeInventoryFinishedProducts.FindAsync(value.DocEntry);

                    _dbSeg.Entry(entity).CurrentValues.SetValues(entidad);
                    _dbSeg.Entry(entity).Property(e => e.ItemCode).IsModified = false;
                    _dbSeg.Entry(entity).Property(e => e.Dscription).IsModified = false;
                    _dbSeg.Entry(entity).Property(e => e.OnHandSys).IsModified = false;
                    _dbSeg.Entry(entity).Property(e => e.WhsCode).IsModified = false;
                    _dbSeg.Entry(entity).Property(e => e.UnitMsr).IsModified = false;
                    _dbSeg.Entry(entity).Property(e => e.UsrCreate).IsModified = false;
                    _dbSeg.Entry(entity).Property(e => e.CreateDate).IsModified = false;
                    _dbSeg.Entry(entity).Property(e => e.CreateTime).IsModified = false;

                    await _dbSeg.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Se realizo correctamente.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<TakeInventoryFinishedProductsEntity>> SetDelete(TakeInventoryFinishedProductsDeleteEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TakeInventoryFinishedProductsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            await using var transaction = await _dbSeg.Database.BeginTransactionAsync();

            try
            {
                var entity = await _dbSeg.TakeInventoryFinishedProducts.FindAsync(value.DocEntry);

                _dbSeg.Entry(entity).CurrentValues.SetValues(value);
                _dbSeg.Entry(entity).Property(e => e.ItemCode).IsModified = false;
                _dbSeg.Entry(entity).Property(e => e.Dscription).IsModified = false;
                _dbSeg.Entry(entity).Property(e => e.OnHandSys).IsModified = false;
                _dbSeg.Entry(entity).Property(e => e.WhsCode).IsModified = false;
                _dbSeg.Entry(entity).Property(e => e.UnitMsr).IsModified = false;
                _dbSeg.Entry(entity).Property(e => e.UsrCreate).IsModified = false;
                _dbSeg.Entry(entity).Property(e => e.CreateDate).IsModified = false;
                _dbSeg.Entry(entity).Property(e => e.CreateTime).IsModified = false;

                await _dbSeg.SaveChangesAsync();


                var list = await _dbSeg.TakeInventoryFinishedProducts1.Where(n => n.IsDelete == "N" && n.DocEntry == value.DocEntry).ToListAsync();

                foreach (var item in list)
                {
                    var entityLine = await _dbSeg.TakeInventoryFinishedProducts1.FindAsync(item.DocEntry, item.LineId);

                    _dbSeg.Entry(entityLine).CurrentValues.SetValues(new TakeInventoryFinishedProducts1DeleteEntity
                    {
                        DocEntry = item.DocEntry,
                        LineId = item.LineId,
                        IsDelete = "Y",
                        UsrDelete = value.UsrDelete,
                        DeleteDate = value.DeleteDate,
                        DeleteTime = value.DeleteTime
                    });
                    _dbSeg.Entry(entityLine).Property(e => e.CodeBar).IsModified = false;
                    _dbSeg.Entry(entityLine).Property(e => e.ProductionDate).IsModified = false;
                    _dbSeg.Entry(entityLine).Property(e => e.Quantity).IsModified = false;
                    _dbSeg.Entry(entityLine).Property(e => e.WeightKg).IsModified = false;
                    _dbSeg.Entry(entityLine).Property(e => e.UsrCreate).IsModified = false;
                    _dbSeg.Entry(entityLine).Property(e => e.CreateDate).IsModified = false;
                    _dbSeg.Entry(entityLine).Property(e => e.CreateTime).IsModified = false;

                    await _dbSeg.SaveChangesAsync();
                }                                

                await transaction.CommitAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Se realizo correctamente.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
    }
}
