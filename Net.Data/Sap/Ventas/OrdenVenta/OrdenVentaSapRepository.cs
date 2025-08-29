using System;
using System.IO;
using System.Data;
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
    public class OrdenVentaSapRepository : RepositoryBase<OrdenVentaSapEntity>, IOrdenVentaSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;

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


        public OrdenVentaSapRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }


        public async Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListSeguimientoByFilter(OrdenVentaSeguimientoFindEntity value)
        {
            var response = new List<OrdenVentaSapByFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>();

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
                            response = (List<OrdenVentaSapByFechaEntity>)context.ConvertTo<OrdenVentaSapByFechaEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoByFilterExcel(OrdenVentaSeguimientoFindEntity value)
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
        public async Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListSeguimientoDetalladoDireccionFiscalByFilter(OrdenVentaSeguimientoFindEntity value)
        {
            var response = new List<OrdenVentaSapByFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>();

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

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdenVentaSapByFechaEntity>)context.ConvertTo<OrdenVentaSapByFechaEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoDetalladoDireccionFiscalByFilterExcel(OrdenVentaSeguimientoFindEntity value)
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

        public async Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListSeguimientoDetalladoDireccionDespachoByFilter(OrdenVentaSeguimientoFindEntity value)
        {
            var response = new List<OrdenVentaSapByFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>();

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

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdenVentaSapByFechaEntity>)context.ConvertTo<OrdenVentaSapByFechaEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetSeguimientoDetalladoDireccionDespachoByFilterExcel(OrdenVentaSeguimientoFindEntity value)
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


        public async Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListOrdenVentaPendienteStockAlmacenProduccionByFecha(FilterRequestEntity value)
        {
            var response = new List<OrdenVentaSapByFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>();

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
                            response = (List<OrdenVentaSapByFechaEntity>)context.ConvertTo<OrdenVentaSapByFechaEntity>(reader);
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


        public async Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListOrdenVentaProgramacionByFecha(FilterRequestEntity value)
        {
            var response = new List<OrdenVentaSapByFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>();

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
                            response = (List<OrdenVentaSapByFechaEntity>)context.ConvertTo<OrdenVentaSapByFechaEntity>(reader);
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


        public async Task<ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>> GetListOrdenVentaPreliminarPendienteByFecha(FilterRequestEntity value)
        {
            var response = new List<OrdenVentaSapByFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<OrdenVentaSapByFechaEntity>();

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
                            response = (List<OrdenVentaSapByFechaEntity>)context.ConvertTo<OrdenVentaSapByFechaEntity>(reader);
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
    }
}
