using System;
using System.IO;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Data.SqlClient;
using DocumentFormat.OpenXml;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
namespace Net.Data.Sap
{
    public class ArticuloSapRepository : RepositoryBase<ArticuloSapEntity>, IArticuloSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_ARTICULO_BY_FILTRO = DB_ESQUEMA + "INV_GetListArticuloByFiltro";
        const string SP_GET_ARTICULO_BY_CODE = DB_ESQUEMA + "INV_GetArticuloByCode";

        const string SP_GET_LIST_STOCK_GENERAL_BY_ALMACEN = DB_ESQUEMA + "INV_GetListStockGeneralByAlmacen";
        const string SP_GET_LIST_STOCK_GENERAL_DETALLADO_ALMACEN_BY_ALMACEN = DB_ESQUEMA + "INV_GetListStockGeneralDetalladoAlmacenByAlmacen";

        const string SP_GET_LIST_ARTICULO_VENTA_GRUPO_SUBGRUPO_ESTADO = DB_ESQUEMA + "INV_GetListArticuloVentaByGrupoSubGrupoEstado";
        const string SP_GET_LIST_ARTICULO_VENTA_STOCK_BY_GRUPO_SUBGRUPO = DB_ESQUEMA + "INV_GetListArticuloVentaStockByGrupoSubGrupo";
        const string SP_GET_LIST_ARTICULO_BY_GRUPO_SUBGRUPO_FILTRO = DB_ESQUEMA + "INV_GetListArticuloByGrupoSubGrupoFiltro";
        const string SP_GET_LIST_MOVIMIENTO_STOCK_BY_FECHA_SEDE = DB_ESQUEMA + "INV_GetListMovimientoStockByFechaSede";

        // Para generación de OV de SODIMAC
        const string SP_GET_FOR_ORDEN_VENTA_SODIMAC_SKU = DB_ESQUEMA + "VEN_GetArticuloForOrdenVentaSodimacBySku";
        // Para generación de OV
        const string SP_GET_ARTICULO_VENTA_BY_CODE = DB_ESQUEMA + "INV_GetArticuloVentaByCode";


        public ArticuloSapRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }


        public async Task<ResultadoTransaccionEntity<ArticuloSapEntity>> GetListByFiltro(FilterRequestEntity value)
        {
            var response = new List<ArticuloSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_ARTICULO_BY_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@ArtInv", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@ArtVen", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@ArtCom", value.Cod3));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<ArticuloSapEntity>)context.ConvertTo<ArticuloSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<ArticuloSapEntity>> GetByCode(FilterRequestEntity value)
        {
            var response = new ArticuloSapEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_ARTICULO_BY_CODE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@ItemCode", value.Cod1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<ArticuloSapEntity>(reader);
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


        public async Task<ResultadoTransaccionEntity<ArticuloSapEntity>> GetListStockGeneralByAlmacen(FilterRequestEntity value)
        {
            var response = new List<ArticuloSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_STOCK_GENERAL_BY_ALMACEN, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@ExcluirInactivo", value.Val1));
                        cmd.Parameters.Add(new SqlParameter("@ExcluirSinStock", value.Val2));
                        cmd.Parameters.Add(new SqlParameter("@Almacen", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<ArticuloSapEntity>)context.ConvertTo<ArticuloSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetStockGeneralByAlmacenExcel(FilterRequestEntity value)
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
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Stock General" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append
                    (
                        ExportToExcel.ConstructCell("Código de Artículo", CellValues.String),
                        ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                        ExportToExcel.ConstructCell("UM", CellValues.String),
                        ExportToExcel.ConstructCell("Stock", CellValues.String),
                        ExportToExcel.ConstructCell("Comprometido", CellValues.String),
                        ExportToExcel.ConstructCell("Solicitado", CellValues.String),
                        ExportToExcel.ConstructCell("Disponible", CellValues.String),
                        ExportToExcel.ConstructCell("Peso Promedio Kg", CellValues.String),
                        ExportToExcel.ConstructCell("Peso Kg", CellValues.String),
                        ExportToExcel.ConstructCell("Fecha de Producción", CellValues.String)
                    );
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListStockGeneralByAlmacen(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append
                        (
                            ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                            ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                            ExportToExcel.ConstructCell(item.InvntryUom, CellValues.String),
                            ExportToExcel.ConstructCell(item.OnHand.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.IsCommited.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.OnOrder.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.Available.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.PesoKg.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.FecProduccion == null ? "" : Convert.ToDateTime(item.FecProduccion).ToString("dd/MM/yyyy"), CellValues.String)
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


        public async Task<ResultadoTransaccionEntity<ArticuloSapEntity>> GetListStockGeneralDetalladoAlmacenByAlmacen(FilterRequestEntity value)
        {
            var response = new List<ArticuloSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_STOCK_GENERAL_DETALLADO_ALMACEN_BY_ALMACEN, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@ExcluirInactivo", value.Val1));
                        cmd.Parameters.Add(new SqlParameter("@ExcluirSinStock", value.Val2));
                        cmd.Parameters.Add(new SqlParameter("@Almacen", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<ArticuloSapEntity>)context.ConvertTo<ArticuloSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetStockGeneralDetalladoAlmacenByAlmacenExcel(FilterRequestEntity value)
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
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Stock General Detallado" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append
                    (
                        ExportToExcel.ConstructCell("Código de Artículo", CellValues.String),
                        ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                        ExportToExcel.ConstructCell("Código de Almacén", CellValues.String),
                        ExportToExcel.ConstructCell("Nombre de Almacén", CellValues.String),
                        ExportToExcel.ConstructCell("UM", CellValues.String),
                        ExportToExcel.ConstructCell("Stock", CellValues.String),
                        ExportToExcel.ConstructCell("Comprometido", CellValues.String),
                        ExportToExcel.ConstructCell("Solicitado", CellValues.String),
                        ExportToExcel.ConstructCell("Disponible", CellValues.String),
                        ExportToExcel.ConstructCell("Peso Promedio Kg", CellValues.String),
                        ExportToExcel.ConstructCell("Peso Kg", CellValues.String),
                        ExportToExcel.ConstructCell("Fecha de Producción", CellValues.String)
                    );
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListStockGeneralDetalladoAlmacenByAlmacen(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append
                        (
                            ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                            ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                            ExportToExcel.ConstructCell(item.WhsCode, CellValues.String),
                            ExportToExcel.ConstructCell(item.WhsName, CellValues.String),
                            ExportToExcel.ConstructCell(item.InvntryUom, CellValues.String),
                            ExportToExcel.ConstructCell(item.OnOrder.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.IsCommited.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.OnOrder.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.Available.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.PesoKg.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.FecProduccion == null ? "" : Convert.ToDateTime(item.FecProduccion).ToString("dd/MM/yyyy"), CellValues.String)
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
                using (SqlConnection conn = new SqlConnection(_cnxSap))
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
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Stock de Articulo de Venta" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Grupo", CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo", CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo 2", CellValues.String),
                    ExportToExcel.ConstructCell("Estado", CellValues.String),
                    ExportToExcel.ConstructCell("UM", CellValues.String),
                    ExportToExcel.ConstructCell("Peso Item", CellValues.String),
                    ExportToExcel.ConstructCell("Peso Promedio Kg", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListArticuloVentaByGrupoSubGrupoEstado(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomGrupo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo2, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomEstado, CellValues.String),
                        ExportToExcel.ConstructCell(item.UnidadVenta, CellValues.String),
                        ExportToExcel.ConstructCell(item.PesoItem.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), CellValues.Number));
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
                using (SqlConnection conn = new SqlConnection(_cnxSap))
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
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Artículos de venta - Stock" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Grupo", CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo", CellValues.String),
                    ExportToExcel.ConstructCell("NomGrupo 2", CellValues.String),
                    ExportToExcel.ConstructCell("UM", CellValues.String),
                    ExportToExcel.ConstructCell("Stock", CellValues.String),
                    ExportToExcel.ConstructCell("Comprometido", CellValues.String),
                    ExportToExcel.ConstructCell("Solicitado", CellValues.String),
                    ExportToExcel.ConstructCell("Disponible", CellValues.String),
                    ExportToExcel.ConstructCell("Peso Promedio Kg", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListArticuloVentaStockByGrupoSubGrupo(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomGrupo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo2, CellValues.String),
                        ExportToExcel.ConstructCell(item.UnidadVenta, CellValues.String),
                        ExportToExcel.ConstructCell(item.Stock.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Comprometido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Solicitado.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Disponible.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), CellValues.Number));
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


        public async Task<ResultadoTransaccionEntity<ArticuloSapEntity>> GetListArticuloByGrupoSubGrupoFiltro(FilterRequestEntity value)
        {
            var response = new List<ArticuloSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
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
                            response = (List<ArticuloSapEntity>)context.ConvertTo<ArticuloSapEntity>(reader);
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
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Artículos por grupo - subgrupo" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código", CellValues.String),
                    ExportToExcel.ConstructCell("Descripción", CellValues.String),
                    ExportToExcel.ConstructCell("Estado", CellValues.String),
                    ExportToExcel.ConstructCell("Grupo", CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo", CellValues.String),
                    ExportToExcel.ConstructCell("NomGrupo 2", CellValues.String),
                    ExportToExcel.ConstructCell("UM", CellValues.String),
                    ExportToExcel.ConstructCell("Stock", CellValues.String),
                    ExportToExcel.ConstructCell("Comprometido", CellValues.String),
                    ExportToExcel.ConstructCell("Solicitado", CellValues.String),
                    ExportToExcel.ConstructCell("Disponible", CellValues.String),
                    ExportToExcel.ConstructCell("Peso", CellValues.String),
                    ExportToExcel.ConstructCell("Peso Promedio Kg", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListArticuloByGrupoSubGrupoFiltro(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.StatusName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomGrupo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo2, CellValues.String),
                        ExportToExcel.ConstructCell(item.SalUnitMsr, CellValues.String),
                        ExportToExcel.ConstructCell(item.OnHand.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.IsCommited.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.OnOrder.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Available.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoArticulo.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), CellValues.Number));
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


        public async Task<ResultadoTransaccionEntity<MovimientoStockSapByFechaSedeEntity>> GetListMovimientoStockByFechaSede(MovimientoStockSapByFechaSedeFindEntity value)
        {
            var response = new List<MovimientoStockSapByFechaSedeEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<MovimientoStockSapByFechaSedeEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
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
                            response = (List<MovimientoStockSapByFechaSedeEntity>)context.ConvertTo<MovimientoStockSapByFechaSedeEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetMovimientoStockExcelByFechaSede(MovimientoStockSapByFechaSedeFindEntity value)
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
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Movimiento de Stock" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Tipo de Movimiento", CellValues.String),
                    ExportToExcel.ConstructCell("Número de Guía SAP", CellValues.String),
                    ExportToExcel.ConstructCell("Número de Guía SUNAT", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de Guía", CellValues.String),
                    ExportToExcel.ConstructCell("Código de Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Usuario", CellValues.String),
                    ExportToExcel.ConstructCell("Código de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Sede", CellValues.String),
                    ExportToExcel.ConstructCell("Centro de Costo", CellValues.String),
                    ExportToExcel.ConstructCell("Almacén de Origen", CellValues.String),
                    ExportToExcel.ConstructCell("Almacén de Destino", CellValues.String),
                    ExportToExcel.ConstructCell("Bultos", CellValues.String),
                    ExportToExcel.ConstructCell("Total Kg", CellValues.String),
                    ExportToExcel.ConstructCell("UM", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Número de Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Número de Fctura SAP", CellValues.String),
                    ExportToExcel.ConstructCell("Número de Fctura SUNAT", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Transportista", CellValues.String),
                    ExportToExcel.ConstructCell("RUC de Transportista", CellValues.String),
                    ExportToExcel.ConstructCell("Placa de Transportista", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Conductor", CellValues.String),
                    ExportToExcel.ConstructCell("Lincencia de Conductor", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListMovimientoStockByFechaSede(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.NomTipoMovimiento, CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroGuiaSAP.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroGuiaSUNAT, CellValues.String),
                        ExportToExcel.ConstructCell(item.DocDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                        ExportToExcel.ConstructCell(item.Usuario, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sede, CellValues.String),
                        ExportToExcel.ConstructCell(item.CentroCosto, CellValues.String),
                        ExportToExcel.ConstructCell(item.AlmacenOrigen, CellValues.String),
                        ExportToExcel.ConstructCell(item.AlmacenDestino, CellValues.String),
                        ExportToExcel.ConstructCell(item.Bulto.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalKg.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.UnidadMedida, CellValues.String),
                        ExportToExcel.ConstructCell(item.Quantity.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroPedido == null ? null : item.NumeroPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.FechaPedido == null ? null : Convert.ToDateTime(item.FechaPedido).ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroFcturaSAP == null ? null : item.NumeroFcturaSAP.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroFcturaSUNAT, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomTransportista, CellValues.String),
                        ExportToExcel.ConstructCell(item.RucTransportista, CellValues.String),
                        ExportToExcel.ConstructCell(item.PlacaTransportista, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomConductor, CellValues.String),
                        ExportToExcel.ConstructCell(item.LincenciaConductor, CellValues.String));
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
        

        public async Task<ResultadoTransaccionEntity<ArticuloSapForSodimacBySkuItemEntity>> GetArticuloForOrdenVentaSodimacBySku(ArticuloSapForSodimacBySkuEntity value)
        {
            var linea = 1;
            var articulo = new ArticuloSapEntity();
            var response = new List<ArticuloSapForSodimacBySkuItemEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloSapForSodimacBySkuItemEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
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
                                articulo = context.Convert<ArticuloSapEntity>(reader);
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


        public async Task<ResultadoTransaccionEntity<ArticuloDocumentoSapEntity>> GetArticuloVentaByCode(FilterRequestEntity value)
        {
            var response = new ArticuloDocumentoSapEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<ArticuloDocumentoSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
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
                            response = context.Convert<ArticuloDocumentoSapEntity>(reader);
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
    }
}
