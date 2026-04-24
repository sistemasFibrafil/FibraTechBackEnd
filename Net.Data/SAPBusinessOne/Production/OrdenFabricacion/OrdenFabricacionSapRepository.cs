using System;
using System.IO;
using System.Data;
using Net.Connection;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class OrdenFabricacionSapRepository : RepositoryBase<OrdenFabricacionSapEntity>, IOrdenFabricacionSapRepository
    {
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_OF_BY_SEDE = DB_ESQUEMA + "PRO_GetListOrdenFabricacionBySede";
        const string SP_GET_LIST_OF_GENERAL_BY_SEDE = DB_ESQUEMA + "PRO_GetListOrdenFabricacionGeneralBySede";


        public OrdenFabricacionSapRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }
        

        public async Task<ResultadoTransaccionResponse<OrdenFabricacionSapEntity>> GetListOrdenFabricacionBySede(FilterRequestEntity value)
        {
            var response = new List<OrdenFabricacionSapEntity>();
            var ResultTransaccion = new ResultadoTransaccionResponse<OrdenFabricacionSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            ResultTransaccion.NombreMetodo = _metodoName;
            ResultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_OF_BY_SEDE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@Location", value.Cod1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdenFabricacionSapEntity>)context.ConvertTo<OrdenFabricacionSapEntity>(reader);
                        }
                    }

                    ResultTransaccion.IdRegistro = 0;
                    ResultTransaccion.ResultadoCodigo = 0;
                    ResultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    ResultTransaccion.dataList = response;
                }
            }
            catch (Exception ex)
            {
                ResultTransaccion.IdRegistro = -1;
                ResultTransaccion.ResultadoCodigo = -1;
                ResultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return ResultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetOrdenFabricacionExcelBySede(FilterRequestEntity value)
        {
            var ResultTransaccion = new ResultadoTransaccionResponse<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            ResultTransaccion.NombreMetodo = _metodoName;
            ResultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                MemoryStream ms = new MemoryStream();

                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Orden Fabicacion" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();

                    row.Append(
                    ExportToExcel.ConstructCell("N° Producción", CellValues.String),
                    ExportToExcel.ConstructCell("Código Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Código de Barra", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad Planificada", CellValues.String),
                    ExportToExcel.ConstructCell("Unidad Medida", CellValues.String),
                    ExportToExcel.ConstructCell("Bultos Procesados", CellValues.String),
                    ExportToExcel.ConstructCell("Pesos Procesados", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha", CellValues.String),
                    ExportToExcel.ConstructCell("Máquina", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListOrdenFabricacionBySede(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();

                        row.Append(
                        ExportToExcel.ConstructCell(item.IdProducion.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.CodeBar, CellValues.String),
                        ExportToExcel.ConstructCell(item.CantidadPlanificada, CellValues.String),
                        ExportToExcel.ConstructCell(item.UnidadMedida, CellValues.String),
                        ExportToExcel.ConstructCell(item.BultoProcesado.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoProcesado.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Fecha.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.Maquina, CellValues.String));

                        sheetData.Append(row);
                    }

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

                ResultTransaccion.IdRegistro = 0;
                ResultTransaccion.ResultadoCodigo = 0;
                ResultTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                ResultTransaccion.data = ms;
            }
            catch (Exception ex)
            {
                ResultTransaccion.IdRegistro = -1;
                ResultTransaccion.ResultadoCodigo = -1;
                ResultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return ResultTransaccion;
        }


        public async Task<ResultadoTransaccionResponse<OrdenFabricacionGeneralSapBySedeEntity>> GetListOrdenFabricacionGeneralBySede(FilterRequestEntity value)
        {
            var response = new List<OrdenFabricacionGeneralSapBySedeEntity>();
            var ResultTransaccion = new ResultadoTransaccionResponse<OrdenFabricacionGeneralSapBySedeEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            ResultTransaccion.NombreMetodo = _metodoName;
            ResultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_OF_GENERAL_BY_SEDE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@Location", value.Cod1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<OrdenFabricacionGeneralSapBySedeEntity>)context.ConvertTo<OrdenFabricacionGeneralSapBySedeEntity>(reader);
                        }
                    }

                    ResultTransaccion.IdRegistro = 0;
                    ResultTransaccion.ResultadoCodigo = 0;
                    ResultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    ResultTransaccion.dataList = response;
                }
            }
            catch (Exception ex)
            {
                ResultTransaccion.IdRegistro = -1;
                ResultTransaccion.ResultadoCodigo = -1;
                ResultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return ResultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetOrdenFabricacionGeneralExcelBySede(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var ResultTransaccion = new ResultadoTransaccionResponse<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            ResultTransaccion.NombreMetodo = _metodoName;
            ResultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Órden de Fabricación" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("DocEntry", CellValues.String),
                    ExportToExcel.ConstructCell("Número SAP", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Órden Fabricación", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Fin", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha Sistema", CellValues.String),
                    ExportToExcel.ConstructCell("Tipo", CellValues.String),
                    ExportToExcel.ConstructCell("Estado", CellValues.String),
                    ExportToExcel.ConstructCell("Norma Reparto", CellValues.String),
                    ExportToExcel.ConstructCell("Item Prod", CellValues.String),
                    ExportToExcel.ConstructCell("Dsc Item Prod", CellValues.String),
                    ExportToExcel.ConstructCell("Almacén", CellValues.String),
                    ExportToExcel.ConstructCell("Unidad Medida", CellValues.String),
                    ExportToExcel.ConstructCell("QProd", CellValues.String),
                    ExportToExcel.ConstructCell("Peso Prod", CellValues.String),
                    ExportToExcel.ConstructCell("Grupo", CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo", CellValues.String),
                    ExportToExcel.ConstructCell("SubGrupo2", CellValues.String),
                    ExportToExcel.ConstructCell("Item Base", CellValues.String),
                    ExportToExcel.ConstructCell("Dsc Item Base", CellValues.String),
                    ExportToExcel.ConstructCell("UM Compra", CellValues.String),
                    ExportToExcel.ConstructCell("QBase", CellValues.String),
                    ExportToExcel.ConstructCell("Planificado", CellValues.String),
                    ExportToExcel.ConstructCell("IssuedQty", CellValues.String),
                    ExportToExcel.ConstructCell("UM Inventario", CellValues.String),
                    ExportToExcel.ConstructCell("Precio", CellValues.String),
                    ExportToExcel.ConstructCell("WareHouse", CellValues.String),
                    ExportToExcel.ConstructCell("CantMillar", CellValues.String),
                    ExportToExcel.ConstructCell("Situacion", CellValues.String),
                    ExportToExcel.ConstructCell("Destino Prod", CellValues.String),
                    ExportToExcel.ConstructCell("Máquina", CellValues.String),
                    ExportToExcel.ConstructCell("Cento Costo", CellValues.String),
                    ExportToExcel.ConstructCell("Comentarios", CellValues.String),
                    ExportToExcel.ConstructCell("Sede", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetListOrdenFabricacionGeneralBySede(value);

                    //Contenido
                    foreach (var item in objectGetList.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.DocEntry.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocNum.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.FechaOrdenFabricacion.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.FechaFin.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.FechaSistema.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.Tipo, CellValues.String),
                        ExportToExcel.ConstructCell(item.Estado, CellValues.String),
                        ExportToExcel.ConstructCell(item.NormaReparto, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemProd, CellValues.String),
                        ExportToExcel.ConstructCell(item.DscItemProd, CellValues.String),
                        ExportToExcel.ConstructCell(item.Almacen, CellValues.String),
                        ExportToExcel.ConstructCell(item.UnidadMedida, CellValues.String),
                        ExportToExcel.ConstructCell(item.QProd.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoProd.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Grupo, CellValues.String),
                        ExportToExcel.ConstructCell(item.SubGrupo, CellValues.String),
                        ExportToExcel.ConstructCell(item.SubGrupo2, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemBase, CellValues.String),
                        ExportToExcel.ConstructCell(item.DscItemBase, CellValues.String),
                        ExportToExcel.ConstructCell(item.QBase.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Planificado.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.IssuedQty.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.UnidadMedidaInventario, CellValues.String),
                        ExportToExcel.ConstructCell(item.Precio.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.WareHouse, CellValues.String),
                        ExportToExcel.ConstructCell(item.CantMillar.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Situacion, CellValues.String),
                        ExportToExcel.ConstructCell(item.DestinoProd, CellValues.String),
                        ExportToExcel.ConstructCell(item.Maquina, CellValues.String),
                        ExportToExcel.ConstructCell(item.Usuario, CellValues.String),
                        ExportToExcel.ConstructCell(item.CentoCosto, CellValues.String),
                        ExportToExcel.ConstructCell(item.Comentarios, CellValues.String),
                        ExportToExcel.ConstructCell(item.Sede, CellValues.String));
                        sheetData.Append(row);
                    }

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

                ResultTransaccion.IdRegistro = 0;
                ResultTransaccion.ResultadoCodigo = 0;
                ResultTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                ResultTransaccion.data = ms;
            }
            catch (Exception ex)
            {
                ResultTransaccion.IdRegistro = -1;
                ResultTransaccion.ResultadoCodigo = -1;
                ResultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return ResultTransaccion;
        }
    }
}
