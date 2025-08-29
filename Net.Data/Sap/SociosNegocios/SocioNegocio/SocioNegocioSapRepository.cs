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
    public class SocioNegocioSapRepository : RepositoryBase<CobranzaCarteraVencidaByFilterSapEntity>, ISocioNegocioSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_BY_FILTRO = DB_ESQUEMA + "SOC_GetListSocioNegocioByFiltro";
        const string SP_GET_BY_CODE = DB_ESQUEMA + "SOC_GetSocioNegocioByCode";
        const string SP_GET_LIST_BY_SECTOR_ESTADO = DB_ESQUEMA + "SOC_GetLitClienteBySectorEstado";
        const string SP_GET_LIST_CONTACTO_BY_SECTOR_ESTADO = DB_ESQUEMA + "SOC_GetLitClienteContactoBySectorEstado";


        public SocioNegocioSapRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }


        public async Task<ResultadoTransaccionEntity<SocioNegocioSapEntity>> GetListByFiltro(FilterRequestEntity value)
        {
            var response = new List<SocioNegocioSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<SocioNegocioSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_BY_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@CardType", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@TransType", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<SocioNegocioSapEntity>)context.ConvertTo<SocioNegocioSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<SocioNegocioSapEntity>> GetByCardCode(FilterRequestEntity value)
        {
            var response = new SocioNegocioSapEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<SocioNegocioSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_BY_CODE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@CardCode", value.Cod1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<SocioNegocioSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<SocioNegocioSapEntity>> GetLitClienteBySectorEstado(FilterRequestEntity value)
        {
            var response = new List<SocioNegocioSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<SocioNegocioSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_BY_SECTOR_ESTADO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Sector", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@Estado", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<SocioNegocioSapEntity>)context.ConvertTo<SocioNegocioSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<SocioNegocioSapEntity>> GetLitClienteContactoBySectorEstado(FilterRequestEntity value)
        {
            var response = new List<SocioNegocioSapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<SocioNegocioSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_CONTACTO_BY_SECTOR_ESTADO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Sector", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@Estado", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<SocioNegocioSapEntity>)context.ConvertTo<SocioNegocioSapEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetClienteExcelBySectorEstado(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var objectGetListCliente = await GetLitClienteBySectorEstado(value);
                var objectGetListClienteContacto = await GetLitClienteContactoBySectorEstado(value);
                ms = GetArchivoClienteExcelBySectorEstado(objectGetListCliente.dataList.ToList(), objectGetListClienteContacto.dataList.ToList());

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

        private MemoryStream GetArchivoClienteExcelBySectorEstado(List<SocioNegocioSapEntity> value1, List<SocioNegocioSapEntity> value2)
        {
            var ms = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                SetArchivoClienteExcelBySectorEstado(document, workbookPart, sheets, value1);
                SetArchivoClienteContactoExcelBySectorEstado(document, workbookPart, sheets, value2);

                workbookPart.Workbook.Save();
                document.Close();
            }

            return ms;
        }

        private void SetArchivoClienteExcelBySectorEstado(SpreadsheetDocument document, WorkbookPart workbookPart, Sheets sheets, List<SocioNegocioSapEntity> value)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Cliente" };
            sheets.Append(sheet);

            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

            //Cabecera
            Row row = new Row();
            row.Append(
            ExportToExcel.ConstructCell("Código de Cliente", CellValues.String),
            ExportToExcel.ConstructCell("RUC", CellValues.String),
            ExportToExcel.ConstructCell("Nombre de Cliente", CellValues.String),
            ExportToExcel.ConstructCell("Estado", CellValues.String),
            ExportToExcel.ConstructCell("Vendedor", CellValues.String),
            ExportToExcel.ConstructCell("Direccion", CellValues.String),
            ExportToExcel.ConstructCell("Sector", CellValues.String),
            ExportToExcel.ConstructCell("División", CellValues.String),
            ExportToExcel.ConstructCell("País", CellValues.String),
            ExportToExcel.ConstructCell("Departamento", CellValues.String),
            ExportToExcel.ConstructCell("Provincia", CellValues.String),
            ExportToExcel.ConstructCell("Distrito", CellValues.String),
            ExportToExcel.ConstructCell("Ubigeo", CellValues.String),
            ExportToExcel.ConstructCell("Teléfono 1", CellValues.String),
            ExportToExcel.ConstructCell("Teléfono 2", CellValues.String),
            ExportToExcel.ConstructCell("Móvil", CellValues.String),
            ExportToExcel.ConstructCell("Correo", CellValues.String),
            ExportToExcel.ConstructCell("Fecha de Alta", CellValues.String),
            ExportToExcel.ConstructCell("Fecha de Baja", CellValues.String),
            ExportToExcel.ConstructCell("Fecha de Última Venta", CellValues.String)
            );
            sheetData.AppendChild(row);

            //Contenido
            foreach (var item in value)
            {
                row = new Row();
                row.Append(
                ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                ExportToExcel.ConstructCell(item.LicTradNum, CellValues.String),
                ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                ExportToExcel.ConstructCell(item.NomStatus, CellValues.String),
                ExportToExcel.ConstructCell(item.SlpName, CellValues.String),
                ExportToExcel.ConstructCell(item.Address, CellValues.String),
                ExportToExcel.ConstructCell(item.NomSector, CellValues.String),
                ExportToExcel.ConstructCell(item.NomDivision, CellValues.String),
                ExportToExcel.ConstructCell(item.Pais, CellValues.String),
                ExportToExcel.ConstructCell(item.NomDepartamento, CellValues.String),
                ExportToExcel.ConstructCell(item.NomProvincia, CellValues.String),
                ExportToExcel.ConstructCell(item.NomDistrito, CellValues.String),
                ExportToExcel.ConstructCell(item.Ubigeo, CellValues.String),
                ExportToExcel.ConstructCell(item.Tel1, CellValues.String),
                ExportToExcel.ConstructCell(item.Tel2, CellValues.String),
                ExportToExcel.ConstructCell(item.Movil, CellValues.String),
                ExportToExcel.ConstructCell(item.Email, CellValues.String),
                ExportToExcel.ConstructCell(item.CreateDate.ToString("dd/MM/yyyy"), CellValues.String),
                ExportToExcel.ConstructCell(item.LowDate == null ? null : Convert.ToDateTime(item.LowDate).ToString("dd/MM/yyyy"), CellValues.String),
                ExportToExcel.ConstructCell(item.FechaUltimaVenta == null ? null : Convert.ToDateTime(item.FechaUltimaVenta).ToString("dd/MM/yyyy"), CellValues.String)
                );
                sheetData.Append(row);
            }
        }

        private void SetArchivoClienteContactoExcelBySectorEstado(SpreadsheetDocument document, WorkbookPart workbookPart, Sheets sheets, List<SocioNegocioSapEntity> value)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 2, Name = "Cliente-Contacto" };
            sheets.Append(sheet);

            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

            //Cabecera
            Row row = new Row();
            row.Append(
            ExportToExcel.ConstructCell("Código de Cliente", CellValues.String),
            ExportToExcel.ConstructCell("RUC", CellValues.String),
            ExportToExcel.ConstructCell("Nombre de Cliente", CellValues.String),
            ExportToExcel.ConstructCell("Estado", CellValues.String),
            ExportToExcel.ConstructCell("Vendedor", CellValues.String),
            ExportToExcel.ConstructCell("Direccion", CellValues.String),
            ExportToExcel.ConstructCell("Sector", CellValues.String),
            ExportToExcel.ConstructCell("División", CellValues.String),
            ExportToExcel.ConstructCell("País", CellValues.String),
            ExportToExcel.ConstructCell("Departamento", CellValues.String),
            ExportToExcel.ConstructCell("Provincia", CellValues.String),
            ExportToExcel.ConstructCell("Distrito", CellValues.String),
            ExportToExcel.ConstructCell("Ubigeo", CellValues.String),
            ExportToExcel.ConstructCell("Contacto", CellValues.String),
            ExportToExcel.ConstructCell("Teléfono 1", CellValues.String),
            ExportToExcel.ConstructCell("Teléfono 2", CellValues.String),
            ExportToExcel.ConstructCell("Móvil", CellValues.String),
            ExportToExcel.ConstructCell("Correo", CellValues.String)
            );
            sheetData.AppendChild(row);

            //Contenido
            foreach (var item in value)
            {
                row = new Row();
                row.Append(
                ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                ExportToExcel.ConstructCell(item.LicTradNum, CellValues.String),
                ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                ExportToExcel.ConstructCell(item.NomStatus, CellValues.String),
                ExportToExcel.ConstructCell(item.SlpName, CellValues.String),
                ExportToExcel.ConstructCell(item.Address, CellValues.String),
                ExportToExcel.ConstructCell(item.NomSector, CellValues.String),
                ExportToExcel.ConstructCell(item.NomDivision, CellValues.String),
                ExportToExcel.ConstructCell(item.Pais, CellValues.String),
                ExportToExcel.ConstructCell(item.NomDepartamento, CellValues.String),
                ExportToExcel.ConstructCell(item.NomProvincia, CellValues.String),
                ExportToExcel.ConstructCell(item.NomDistrito, CellValues.String),
                ExportToExcel.ConstructCell(item.Ubigeo, CellValues.String),
                ExportToExcel.ConstructCell(item.NomContacto, CellValues.String),
                ExportToExcel.ConstructCell(item.TelContacto1, CellValues.String),
                ExportToExcel.ConstructCell(item.TelContacto2, CellValues.String),
                ExportToExcel.ConstructCell(item.MovilContacto, CellValues.String),
                ExportToExcel.ConstructCell(item.EmailContacto, CellValues.String)
                );
                sheetData.Append(row);
            }
        }



        //public async Task<ResultadoTransaccionEntity<MemoryStream>> GetClienteExcelBySectorEstado(FilterRequestEntity value)
        //{
        //    var ms = new MemoryStream();
        //    var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    resultTransaccion.NombreMetodo = _metodoName;
        //    resultTransaccion.NombreAplicacion = _aplicacionName;

        //    try
        //    {
        //        using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
        //        {
        //            WorkbookPart workbookPart = document.AddWorkbookPart();
        //            workbookPart.Workbook = new Workbook();

        //            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        //            worksheetPart.Worksheet = new Worksheet();

        //            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
        //            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Socio de negocios" };
        //            sheets.Append(sheet);

        //            workbookPart.Workbook.Save();

        //            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

        //            //Cabecera
        //            Row row = new Row();
        //            row.Append(
        //            ExportToExcel.ConstructCell("Código de Cliente", CellValues.String),
        //            ExportToExcel.ConstructCell("RUC", CellValues.String),
        //            ExportToExcel.ConstructCell("Nombre de Cliente", CellValues.String),
        //            ExportToExcel.ConstructCell("Estado", CellValues.String),
        //            ExportToExcel.ConstructCell("Vendedor", CellValues.String),
        //            ExportToExcel.ConstructCell("Direccion", CellValues.String),
        //            ExportToExcel.ConstructCell("Sector", CellValues.String),
        //            ExportToExcel.ConstructCell("División", CellValues.String),
        //            ExportToExcel.ConstructCell("País", CellValues.String),
        //            ExportToExcel.ConstructCell("Departamento", CellValues.String),
        //            ExportToExcel.ConstructCell("Provincia", CellValues.String),
        //            ExportToExcel.ConstructCell("Distrito", CellValues.String),
        //            ExportToExcel.ConstructCell("Ubigeo", CellValues.String),
        //            ExportToExcel.ConstructCell("Contacto", CellValues.String),
        //            ExportToExcel.ConstructCell("Teléfono de Contacto", CellValues.String),
        //            ExportToExcel.ConstructCell("Fecha de Alta", CellValues.String),
        //            ExportToExcel.ConstructCell("Fecha de Baja", CellValues.String),
        //            ExportToExcel.ConstructCell("Fecha de Última Venta", CellValues.String));
        //            sheetData.AppendChild(row);

        //            var objectGetList = await GetLitClienteBySectorEstado(value);

        //            //Contenido
        //            foreach (var item in objectGetList.dataList)
        //            {
        //                row = new Row();
        //                row.Append(
        //                ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
        //                ExportToExcel.ConstructCell(item.LicTradNum, CellValues.String),
        //                ExportToExcel.ConstructCell(item.CardName, CellValues.String),
        //                ExportToExcel.ConstructCell(item.NomStatus, CellValues.String),
        //                ExportToExcel.ConstructCell(item.SlpName, CellValues.String),
        //                ExportToExcel.ConstructCell(item.Address, CellValues.String),
        //                ExportToExcel.ConstructCell(item.NomSector, CellValues.String),
        //                ExportToExcel.ConstructCell(item.NomDivision, CellValues.String),
        //                ExportToExcel.ConstructCell(item.Pais, CellValues.String),
        //                ExportToExcel.ConstructCell(item.NomDepartamento, CellValues.String),
        //                ExportToExcel.ConstructCell(item.NomProvincia, CellValues.String),
        //                ExportToExcel.ConstructCell(item.NomDistrito, CellValues.String),
        //                ExportToExcel.ConstructCell(item.Ubigeo, CellValues.String),
        //                ExportToExcel.ConstructCell(item.NomContacto, CellValues.String),
        //                ExportToExcel.ConstructCell(item.TelContacto, CellValues.String),
        //                ExportToExcel.ConstructCell(item.CreateDate.ToString("dd/MM/yyyy"), CellValues.String),
        //                ExportToExcel.ConstructCell(item.LowDate == null ? null : Convert.ToDateTime(item.LowDate).ToString("dd/MM/yyyy"), CellValues.String),
        //                ExportToExcel.ConstructCell(item.FechaUltimaVenta == null ? null : Convert.ToDateTime(item.FechaUltimaVenta).ToString("dd/MM/yyyy"), CellValues.String));
        //                sheetData.Append(row);
        //            }

        //            worksheetPart.Worksheet.Save();
        //            document.Close();
        //        }

        //        resultTransaccion.IdRegistro = 0;
        //        resultTransaccion.ResultadoCodigo = 0;
        //        resultTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
        //        resultTransaccion.data = ms;
        //    }
        //    catch (Exception ex)
        //    {
        //        resultTransaccion.IdRegistro = -1;
        //        resultTransaccion.ResultadoCodigo = -1;
        //        resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }

        //    return resultTransaccion;
        //}
    }
}
