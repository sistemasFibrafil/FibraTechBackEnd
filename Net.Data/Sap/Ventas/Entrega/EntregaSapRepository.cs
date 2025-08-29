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
    public class EntregaSapRepository : RepositoryBase<EntregaSapEntity>, IEntregaSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST = DB_ESQUEMA + "VEN_GetListGuiaByFecha";


        public EntregaSapRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }


        public async Task<ResultadoTransaccionEntity<EntregaSapByFechaEntity>> GetListGuiaByFecha(FilterRequestEntity value)
        {
            var response = new List<EntregaSapByFechaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<EntregaSapByFechaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<EntregaSapByFechaEntity>)context.ConvertTo<EntregaSapByFechaEntity>(reader);
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

        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetGuiaExcelByFecha(FilterRequestEntity value)
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
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Guías" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("FECHA EMISIÓN", CellValues.String),
                    ExportToExcel.ConstructCell("TIPO", CellValues.String),
                    ExportToExcel.ConstructCell("SERIE", CellValues.String),
                    ExportToExcel.ConstructCell("NÚMERO", CellValues.String),
                    ExportToExcel.ConstructCell("DOC ENTIDAD", CellValues.String),
                    ExportToExcel.ConstructCell("RUC", CellValues.String),
                    ExportToExcel.ConstructCell("DENOMINACIÓN", CellValues.String),
                    ExportToExcel.ConstructCell("DETALLE (LINEAS)", CellValues.String),
                    ExportToExcel.ConstructCell("PESO NETO", CellValues.String),
                    ExportToExcel.ConstructCell("PESO UNIDAD DE MEDIDA", CellValues.String),
                    ExportToExcel.ConstructCell("FECHA DE TRASLADO", CellValues.String),
                    ExportToExcel.ConstructCell("TRANSPORTISTA DOCUMENTO TIPO", CellValues.String),
                    ExportToExcel.ConstructCell("TRANSPORTISTA DOCUMENTO NUMERO", CellValues.String),
                    ExportToExcel.ConstructCell("TRANSPORTISTA DENOMINACION", CellValues.String),
                    ExportToExcel.ConstructCell("PLACA", CellValues.String),
                    ExportToExcel.ConstructCell("CONDUCTOR DOCUMENTO TIPO", CellValues.String),
                    ExportToExcel.ConstructCell("CONDUCTOR DOCUMENTO NUMERO", CellValues.String),
                    ExportToExcel.ConstructCell("CONDUCTOR NOMBRE", CellValues.String),
                    ExportToExcel.ConstructCell("CONDUCTOR APELLIDOS", CellValues.String),
                    ExportToExcel.ConstructCell("CONDUCTOR LICENCIA", CellValues.String),
                    ExportToExcel.ConstructCell("PARTIDA UBIGEO", CellValues.String),
                    ExportToExcel.ConstructCell("PARTIDA DIRECCIÓN", CellValues.String),
                    ExportToExcel.ConstructCell("LLEGADA UBIGEO", CellValues.String),
                    ExportToExcel.ConstructCell("LLEGADA DIRECCIÓN", CellValues.String),
                    ExportToExcel.ConstructCell("OBSERVACIÓN", CellValues.String),
                    ExportToExcel.ConstructCell("ESTADO SUNAT", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGet = await GetListGuiaByFecha(value);

                    //Contenido
                    foreach (var item in objectGet.dataList)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.FechaEmision.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.Tipo, CellValues.String),
                        ExportToExcel.ConstructCell(item.Serie, CellValues.String),
                        ExportToExcel.ConstructCell(item.Numero, CellValues.String),

                        ExportToExcel.ConstructCell(item.ClienteTipoDocumento, CellValues.String),
                        ExportToExcel.ConstructCell(item.ClienteNumeroDocumento, CellValues.String),
                        ExportToExcel.ConstructCell(item.ClienteDenominacion, CellValues.String),

                        ExportToExcel.ConstructCell(item.Detalle, CellValues.String),
                        ExportToExcel.ConstructCell(item.PesoBruto.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.PesoUnidadMedida, CellValues.String),
                        ExportToExcel.ConstructCell(item.FechaTraslado.ToString("dd/MM/yyyy"), CellValues.String),

                        ExportToExcel.ConstructCell(item.TransportistaDocumentoTipo, CellValues.String),
                        ExportToExcel.ConstructCell(item.TransportistaDocumentoNumero, CellValues.String),
                        ExportToExcel.ConstructCell(item.TransportistaDenominacion, CellValues.String),
                        ExportToExcel.ConstructCell(item.TransportistaPlacaNumero, CellValues.String),

                        ExportToExcel.ConstructCell(item.ConductorDocumentoTipo, CellValues.String),
                        ExportToExcel.ConstructCell(item.ConductorDocumentoNumero, CellValues.String),
                        ExportToExcel.ConstructCell(item.ConductorNombre, CellValues.String),
                        ExportToExcel.ConstructCell(item.ConductorApellidos, CellValues.String),
                        ExportToExcel.ConstructCell(item.ConductorLicenciaNumero, CellValues.String),

                        ExportToExcel.ConstructCell(item.PuntoPartidaDireccion, CellValues.String),
                        ExportToExcel.ConstructCell(item.PuntoPartidaDireccion, CellValues.String),
                        ExportToExcel.ConstructCell(item.PuntoLlegadaUbigeo, CellValues.String),
                        ExportToExcel.ConstructCell(item.PuntoPartidaUbigeo, CellValues.String),

                        ExportToExcel.ConstructCell(item.Observaciones, CellValues.String),
                        ExportToExcel.ConstructCell(item.EstadoSunat, CellValues.String));
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

