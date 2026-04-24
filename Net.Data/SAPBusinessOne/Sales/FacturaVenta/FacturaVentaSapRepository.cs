using System;
using System.IO;
using System.Data;
using Net.Connection;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Data.SqlClient;
using DocumentFormat.OpenXml;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class FacturaVentaSapRepository : RepositoryBase<FacturaVentaSapEntity>, IFacturaVentaSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_VENTA_BY_FILTER = DB_ESQUEMA + "VEN_GetLitVentaByFilter";
        const string SP_GET_LIST_FACTURA_VENTA_BY_FILTER = DB_ESQUEMA + "VEN_GetLitFacturaVentaByFilter";
        const string SP_GET_LIST_VENTA_RESUMEN_BY_FECHA_GRUPO1 = DB_ESQUEMA + "VEN_GetLitVentaResumenByFechaGrupo1";
        const string SP_GET_LIST_VENTA_RESUMEN_BY_FECHA_GRUPO2 = DB_ESQUEMA + "VEN_GetLitVentaResumenByFechaGrupo2";
        const string SP_GET_LIST_VENTA_RESUMEN_BY_FECHA_GRUPO3 = DB_ESQUEMA + "VEN_GetLitVentaResumenByFechaGrupo3";

        const string SP_GET_LIST_VENTA_PROYECCION_BY_FECHA = DB_ESQUEMA + "VEN_GetListVentaProyeccionByFecha";


        public FacturaVentaSapRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(_configuration, "EntornoConnectionSap:Entorno");
        }


        public async Task<ResultadoTransaccionResponse<VentaProyeccionSapByFechaEntity>> GetListVentaProyeccionByFecha(FilterRequestEntity value)
        {
            var response = new List<VentaProyeccionSapByFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<VentaProyeccionSapByFechaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_VENTA_PROYECCION_BY_FECHA, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<VentaProyeccionSapByFechaEntity>)context.ConvertTo<VentaProyeccionSapByFechaEntity>(reader);
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


        public async Task<ResultadoTransaccionResponse<FacturaVentaSapEntity>> GetListVentaResumenByFechaGrupo(FilterRequestEntity value)
        {
            var response = new FacturaVentaSapEntity();
            var resultTransaccion = new ResultadoTransaccionResponse<FacturaVentaSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_VENTA_RESUMEN_BY_FECHA_GRUPO1, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@Grupo", value.Cod1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response.VentaResumen1 = (List<VentaResumenSapByFechaGrupoEntity>)context.ConvertTo<VentaResumenSapByFechaGrupoEntity>(reader);
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_VENTA_RESUMEN_BY_FECHA_GRUPO2, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@Grupo", value.Cod1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response.VentaResumen2 = (List<VentaResumenSapByFechaGrupoEntity>)context.ConvertTo<VentaResumenSapByFechaGrupoEntity>(reader);
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_VENTA_RESUMEN_BY_FECHA_GRUPO3, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@Grupo", value.Cod1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response.VentaResumen2 = (List<VentaResumenSapByFechaGrupoEntity>)context.ConvertTo<VentaResumenSapByFechaGrupoEntity>(reader);
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
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetVentaResumenExcelByFechaGrupo(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var objectGet = await GetListVentaResumenByFechaGrupo(value);
                ms = GetArchivoVentaResumenExcelByFechaGrupo(objectGet.data);

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
        private MemoryStream GetArchivoVentaResumenExcelByFechaGrupo(FacturaVentaSapEntity value)
        {
            var ms = new MemoryStream();

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                SetArchivoVentaResumenExcelByFechaGrupo1(document, workbookPart, sheets, value);
                SetArchivoVentaResumenExcelByFechaGrupo2(document, workbookPart, sheets, value);
                SetArchivoVentaResumenExcelByFechaGrupo3(document, workbookPart, sheets, value);

                workbookPart.Workbook.Save();
                document.Close();
            }

            return ms;
        }
        private void SetArchivoVentaResumenExcelByFechaGrupo1(SpreadsheetDocument document, WorkbookPart workbookPart, Sheets sheets, FacturaVentaSapEntity value)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Vendedor-Grupo" };
            sheets.Append(sheet);

            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

            //Cabecera
            Row row = new Row();
            row.Append(
            ExportToExcel.ConstructCell("Vendedor", CellValues.String),
            ExportToExcel.ConstructCell("Grupo", CellValues.String),
            ExportToExcel.ConstructCell("Cantidad", CellValues.String),
            ExportToExcel.ConstructCell("Total USD", CellValues.String));
            sheetData.AppendChild(row);

            //Contenido
            foreach (var item in value.VentaResumen1)
            {
                row = new Row();
                row.Append(
                ExportToExcel.ConstructCell(item.NomVendedor, CellValues.String),
                ExportToExcel.ConstructCell(item.NomGrupo, CellValues.String),
                ExportToExcel.ConstructCell(item.Cantidad.ToString(), CellValues.Number),
                ExportToExcel.ConstructCell(item.TotalItemUSD.ToString(), CellValues.Number));
                sheetData.Append(row);
            }
        }
        private void SetArchivoVentaResumenExcelByFechaGrupo2(SpreadsheetDocument document, WorkbookPart workbookPart, Sheets sheets, FacturaVentaSapEntity value)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 2, Name = "Vendedor-Grupo-UM" };
            sheets.Append(sheet);

            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

            //Cabecera
            Row row = new Row();
            row.Append(
            ExportToExcel.ConstructCell("Vendedor", CellValues.String),
            ExportToExcel.ConstructCell("Grupo", CellValues.String),
            ExportToExcel.ConstructCell("Unidad de Medida", CellValues.String),
            ExportToExcel.ConstructCell("Cantidad", CellValues.String),
            ExportToExcel.ConstructCell("Total USD", CellValues.String));
            sheetData.AppendChild(row);

            //Contenido
            foreach (var item in value.VentaResumen2)
            {
                row = new Row();
                row.Append(
                ExportToExcel.ConstructCell(item.NomVendedor, CellValues.String),
                ExportToExcel.ConstructCell(item.NomGrupo, CellValues.String),
                ExportToExcel.ConstructCell(item.UnidadMedida, CellValues.String),
                ExportToExcel.ConstructCell(item.Cantidad.ToString(), CellValues.Number),
                ExportToExcel.ConstructCell(item.TotalItemUSD.ToString(), CellValues.Number));
                sheetData.Append(row);
            }
        }
        private void SetArchivoVentaResumenExcelByFechaGrupo3(SpreadsheetDocument document, WorkbookPart workbookPart, Sheets sheets, FacturaVentaSapEntity value)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 3, Name = "Vendedor-Grupo-Artículo-UM" };
            sheets.Append(sheet);

            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

            //Cabecera
            Row row = new Row();
            row.Append(
            ExportToExcel.ConstructCell("Vendedor", CellValues.String),
            ExportToExcel.ConstructCell("Grupo", CellValues.String),
            ExportToExcel.ConstructCell("Artículo", CellValues.String),
            ExportToExcel.ConstructCell("Unidad de Medida", CellValues.String),
            ExportToExcel.ConstructCell("Cantidad", CellValues.String),
            ExportToExcel.ConstructCell("Total USD", CellValues.String));
            sheetData.AppendChild(row);

            //Contenido
            foreach (var item in value.VentaResumen3)
            {
                row = new Row();
                row.Append(
                ExportToExcel.ConstructCell(item.NomVendedor, CellValues.String),
                ExportToExcel.ConstructCell(item.NomGrupo, CellValues.String),
                ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                ExportToExcel.ConstructCell(item.UnidadMedida, CellValues.String),
                ExportToExcel.ConstructCell(item.Cantidad.ToString(), CellValues.Number),
                ExportToExcel.ConstructCell(item.TotalItemUSD.ToString(), CellValues.Number));
                sheetData.Append(row);
            }
        }

        public async Task<ResultadoTransaccionResponse<VentaSapByFilterCodeEntity>> GetListVentaByFilter(VentaSapByFilterFindEntity value)
        {
            var response = new List<VentaSapByFilterCodeEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<VentaSapByFilterCodeEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_VENTA_BY_FILTER, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@StartDate", value.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", value.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@SalesEmployee", value.SalesEmployee));
                        cmd.Parameters.Add(new SqlParameter("@Customer", value.Customer));
                        cmd.Parameters.Add(new SqlParameter("@Item", value.Item));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<VentaSapByFilterCodeEntity>)context.ConvertTo<VentaSapByFilterCodeEntity>(reader);
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
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetVentaByFilterExcel(VentaSapByFilterFindEntity value)
        {
            var ms = new MemoryStream();
            var response = new List<FilterRequestEntity>();
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
                    ExportToExcel.ConstructCell("Unidad de Negocio", CellValues.String),
                    ExportToExcel.ConstructCell("Código de Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("División", CellValues.String),
                    ExportToExcel.ConstructCell("Sector", CellValues.String),
                    ExportToExcel.ConstructCell("Pais", CellValues.String),
                    ExportToExcel.ConstructCell("Departamento", CellValues.String),
                    ExportToExcel.ConstructCell("Provincia", CellValues.String),
                    ExportToExcel.ConstructCell("Cuidad", CellValues.String),

                    ExportToExcel.ConstructCell("Tipo de Documento", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de Contabilización", CellValues.String),
                    ExportToExcel.ConstructCell("Número de Documento", CellValues.String),
                    ExportToExcel.ConstructCell("Número de Guía", CellValues.String),
                    ExportToExcel.ConstructCell("Número de Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de Pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Codicion de Pago", CellValues.String),

                    ExportToExcel.ConstructCell("Código de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Grupo", CellValues.String),
                    ExportToExcel.ConstructCell("Forcast", CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo", CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo 2", CellValues.String),
                    ExportToExcel.ConstructCell("Medida", CellValues.String),
                    ExportToExcel.ConstructCell("Color", CellValues.String),
                    ExportToExcel.ConstructCell("Porcentaje", CellValues.String),

                    ExportToExcel.ConstructCell("UM", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad", CellValues.String),
                    ExportToExcel.ConstructCell("PesoItem", CellValues.String),
                    ExportToExcel.ConstructCell("PesoP romedio Kg", CellValues.String),
                    ExportToExcel.ConstructCell("Peso VentaKg", CellValues.String),
                    ExportToExcel.ConstructCell("Peso", CellValues.String),

                    ExportToExcel.ConstructCell("Rollo Vendido", CellValues.String),
                    ExportToExcel.ConstructCell("Kg Vendido", CellValues.String),
                    ExportToExcel.ConstructCell("Tonelada Vendida", CellValues.String),

                    ExportToExcel.ConstructCell("Moneda", CellValues.String),
                    ExportToExcel.ConstructCell("TC", CellValues.String),
                    ExportToExcel.ConstructCell("Precio", CellValues.String),
                    ExportToExcel.ConstructCell("Precio Kg", CellValues.String),
                    ExportToExcel.ConstructCell("Costo SOL", CellValues.String),
                    ExportToExcel.ConstructCell("Costo USD", CellValues.String),

                    ExportToExcel.ConstructCell("Total Costo Item SOL", CellValues.String),
                    ExportToExcel.ConstructCell("Total Costo Item USD", CellValues.String),

                    ExportToExcel.ConstructCell("Total Item SOL", CellValues.String),
                    ExportToExcel.ConstructCell("Total Item USD", CellValues.String),
                    ExportToExcel.ConstructCell("Sede", CellValues.String),
                    ExportToExcel.ConstructCell("Ubigeo", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGet = await GetListVentaByFilter(value);

                    //Contenido
                    foreach (var item in objectGet.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.UnidadNegocio, CellValues.String),
                        ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                        ExportToExcel.ConstructCell(item.Division, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sector, CellValues.String),
                        ExportToExcel.ConstructCell(item.Pais, CellValues.String),
                        ExportToExcel.ConstructCell(item.Departamento, CellValues.String),
                        ExportToExcel.ConstructCell(item.Provincia, CellValues.String),
                        ExportToExcel.ConstructCell(item.Cuidad, CellValues.String),

                        ExportToExcel.ConstructCell(item.TipoDocumento, CellValues.String),
                        ExportToExcel.ConstructCell(item.FecContabilizacion.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroDocumento, CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroGuia, CellValues.String),
                        ExportToExcel.ConstructCell(item.NumeroPedido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.FechaPedido == null ? "" : Convert.ToDateTime(item.FechaPedido).ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.NomVendedor, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomCondicionPago, CellValues.String),

                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomGrupo, CellValues.String),
                        ExportToExcel.ConstructCell(item.Forcast.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NomSubGrupo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomSubGrupo2, CellValues.String),
                        ExportToExcel.ConstructCell(item.Medida, CellValues.String),
                        ExportToExcel.ConstructCell(item.Color, CellValues.String),
                        ExportToExcel.ConstructCell(item.Porcentaje, CellValues.String),

                        ExportToExcel.ConstructCell(item.UnidadMedida, CellValues.String),
                        ExportToExcel.ConstructCell(item.Cantidad.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoItem.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoPromedioKg.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoVentaKg.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Peso.ToString(), CellValues.Number),

                        ExportToExcel.ConstructCell(item.RolloVendido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.KgVendido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.ToneladaVendida.ToString(), CellValues.Number),

                        ExportToExcel.ConstructCell(item.CodMoneda, CellValues.String),
                        ExportToExcel.ConstructCell(item.TipoCambio.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Precio.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PrecioKg.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.CostoSOL.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.CostoUSD.ToString(), CellValues.Number),

                        ExportToExcel.ConstructCell(item.TotalCostoItemSOL.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalCostoItemUSD.ToString(), CellValues.Number),

                        ExportToExcel.ConstructCell(item.TotalItemSOL.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.TotalItemUSD.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Sede, CellValues.String),
                        ExportToExcel.ConstructCell(item.Ubigeo, CellValues.String));
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


        public async Task<ResultadoTransaccionResponse<FacturaVentaSapByFechaEntity>> GetListFacturaVentaByFilter(FacturaVentaSapByFilterFindEntity value)
        {
            var response = new List<FacturaVentaSapByFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<FacturaVentaSapByFechaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_FACTURA_VENTA_BY_FILTER, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@StartDate", value.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", value.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@Customer", value.Customer));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<FacturaVentaSapByFechaEntity>)context.ConvertTo<FacturaVentaSapByFechaEntity>(reader);
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
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetFacturaVentaByFilterExcel(FacturaVentaSapByFilterFindEntity value)
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
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Facturas de ventas" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Nombre de Cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de Contabilización", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de Vencimiento", CellValues.String),
                    ExportToExcel.ConstructCell("Días Vencidas", CellValues.String),
                    ExportToExcel.ConstructCell("Número de Documento", CellValues.String),
                    ExportToExcel.ConstructCell("Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Moneda", CellValues.String),
                    ExportToExcel.ConstructCell("Total", CellValues.String),
                    ExportToExcel.ConstructCell("Cobrado", CellValues.String),
                    ExportToExcel.ConstructCell("Saldo", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGet = await GetListFacturaVentaByFilter(value);

                    //Contenido
                    foreach (var item in objectGet.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                        ExportToExcel.ConstructCell(item.FecContabilizacion.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.FecContabilizacion.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.DiaVencido.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.NumeroDocumento, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomVendedor, CellValues.String),
                        ExportToExcel.ConstructCell(item.CodMoneda, CellValues.String),
                        ExportToExcel.ConstructCell(item.Total.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Cobrado.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Saldo.ToString(), CellValues.Number));
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
    }
}
