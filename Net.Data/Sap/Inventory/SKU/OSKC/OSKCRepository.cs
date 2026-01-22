using System;
using System.IO;
using AutoMapper;
using SAPbobsCOM;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using System.Reflection;
using Net.Data.AppContext;
using Net.Business.Entities;
using DocumentFormat.OpenXml;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;

namespace Net.Data.Sap
{
    /// <summary>
    /// Repositorio para interactuar con la tabla de usuario @FIB_OSKC de SAP.
    /// </summary>
    public class OSKCRepository : RepositoryBase<OSKCEntity>, IOSKCRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IMapper _am;
        private readonly DataContextSap _dc;
        private readonly CompanyProviderSap _companyProviderSap;

        // --- Constantes para evitar "magic strings" ---
        private const string SapGeneralServiceName = "FIB_OSKC";

        public OSKCRepository(IConnectionSQL context, IConfiguration configuration, DataContextSap db, IMapper am, CompanyProviderSap companyProviderSap)
            : base(context)
        {
            _am = am;
            _dc = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }

        private void SetGeneralDataProperties(GeneralData oGeneralData, OSKCEntity value)
        {
            oGeneralData.SetProperty("Name", value.U_Number);
            oGeneralData.SetProperty("U_Number", value.U_Number);
            oGeneralData.SetProperty("U_SlpCode", value.U_SlpCode);
            oGeneralData.SetProperty("U_Status", value.U_Status);
            oGeneralData.SetProperty("U_DocDate", value.U_DocDate);
            oGeneralData.SetProperty("U_ItemCodeBase", value.U_ItemCodeBase);
            oGeneralData.SetProperty("U_ItmsGrpCod", value.U_ItmsGrpCod);
            oGeneralData.SetProperty("U_ItmsSGrpCod", value.U_ItmsSGrpCod);
            oGeneralData.SetProperty("U_ItemName", value.U_ItemName);
            oGeneralData.SetProperty("U_CardCode", value.U_CardCode);
            oGeneralData.SetProperty("U_Quantity", Convert.ToDouble(Math.Round(value.U_Quantity, 6)));
            oGeneralData.SetProperty("U_UnitMsrCode", value.U_UnitMsrCode);
            oGeneralData.SetProperty("U_Wide", Convert.ToDouble(Math.Round(value.U_Wide, 6)));
            oGeneralData.SetProperty("U_UnitCode", value.U_UnitCode);
            oGeneralData.SetProperty("U_Long", Convert.ToDouble(Math.Round(value.U_Long, 6)));
            oGeneralData.SetProperty("U_GrMtSq", Convert.ToDouble(Math.Round(value.U_GrMtSq, 6)));
            oGeneralData.SetProperty("U_ItemWeight", Convert.ToDouble(Math.Round(value.U_ItemWeight, 6)));
            oGeneralData.SetProperty("U_ColorCode", value.U_ColorCode);
            oGeneralData.SetProperty("U_Laminate", value.U_Laminate);
            if (value.U_LamTypCode != null) oGeneralData.SetProperty("U_LamTypCode", value.U_LamTypCode);
            oGeneralData.SetProperty("U_Linner", value.U_Linner);
            oGeneralData.SetProperty("U_LinnWeight", Convert.ToDouble(Math.Round(value.U_LinnWeight, 6)));
            oGeneralData.SetProperty("U_Print", value.U_Print);
            if (value.U_PrintColCode != null) oGeneralData.SetProperty("U_PrintColCode", value.U_PrintColCode);
            oGeneralData.SetProperty("U_Fuelle", value.U_Fuelle);
            oGeneralData.SetProperty("U_UvByMonCode", value.U_UvByMonCode);
            oGeneralData.SetProperty("U_PrjMonVol", Convert.ToDouble(Math.Round(value.U_PrjMonVol, 6)));
            oGeneralData.SetProperty("U_Price", Convert.ToDouble(Math.Round(value.U_Price, 6)));
            oGeneralData.SetProperty("U_Observations", value.U_Observations);
        }

        /// <summary>
        /// Crea un nuevo registro de configuración de SKU (@FIB_OSKC).
        /// </summary>
        public Task<ResultadoTransaccionEntity<OSKCEntity>> SetCreate(OSKCEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKCEntity>
            {
                NombreMetodo = MethodBase.GetCurrentMethod().Name,
                NombreAplicacion = _aplicacionName
            };

            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            GeneralService oGeneralService = null;

            try
            {
                // Conexión a SAP
                var company = _companyProviderSap.GetCompany();

                oCompService = company.GetCompanyService();
                oGeneralService = oCompService.GetGeneralService(SapGeneralServiceName);
                oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralData.SetProperty("Code", value.U_Number);
                SetGeneralDataProperties(oGeneralData, value);
                oGeneralService.Add(oGeneralData);

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Registro procesado con éxito.";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(oGeneralData, oGeneralService, oCompService);
            }

            return Task.FromResult(resultTransaccion);
        }

        /// <summary>
        /// Actualiza un registro de configuración de SKU (@FIB_OSKC).
        /// </summary>
        public Task<ResultadoTransaccionEntity<OSKCEntity>> SetUpdate(OSKCEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKCEntity>
            {
                NombreMetodo = MethodBase.GetCurrentMethod().Name,
                NombreAplicacion = _aplicacionName
            };

            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralDataParams = null;

            try
            {
                // Conexión a SAP
                var company = _companyProviderSap.GetCompany();

                oCompService = company.GetCompanyService();
                oGeneralService = oCompService.GetGeneralService(SapGeneralServiceName);
                oGeneralDataParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGeneralDataParams.SetProperty("Code", value.Code);
                oGeneralData = oGeneralService.GetByParams(oGeneralDataParams);

                SetGeneralDataProperties(oGeneralData, value);
                oGeneralService.Update(oGeneralData);

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Registro actualizado con éxito.";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(oGeneralData, oGeneralDataParams, oGeneralService, oCompService);
            }

            return Task.FromResult(resultTransaccion);
        }

        /// <summary>
        /// Elimina un registro de configuración de SKU (@FIB_OSKC).
        /// </summary>
        public Task<ResultadoTransaccionEntity<OSKCEntity>> SetDelete(OSKCEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKCEntity>
            {
                NombreMetodo = MethodBase.GetCurrentMethod().Name,
                NombreAplicacion = _aplicacionName
            };

            CompanyService oCompService = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralParams = null;

            try
            {
                // Conexión a SAP
                var company = _companyProviderSap.GetCompany();

                oCompService = company.GetCompanyService();
                oGeneralService = oCompService.GetGeneralService(SapGeneralServiceName);
                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGeneralParams.SetProperty("Code", value.Code);
                oGeneralService.Delete(oGeneralParams);

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Registro eliminado con éxito.";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(oGeneralParams, oGeneralService, oCompService);
            }

            return Task.FromResult(resultTransaccion);
        }

        public async Task<ResultadoTransaccionEntity<OSKCEntity>> GetListByDateRange(OSKCEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKCEntity>
            {
                NombreMetodo = regex.Match(MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var lista = await _dc.OSKCView.Where(x => x.U_DocDate >= value.StrDate && x.U_DocDate <= value.EndDate).ToListAsync();

                if (!lista.Any())
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = "No se encontraron registros con el filtro especificado.";
                    return resultTransaccion;
                }
                
                
                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {lista.Count}";
                resultTransaccion.dataList = _am.Map<List<OSKCEntity>>(lista);
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        /// <summary>
        /// Obtiene una configuración de SKU por su código único.
        /// </summary>
        public async Task<ResultadoTransaccionEntity<OSKCEntity>> GetByCode(OSKCEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKCEntity>
            {
                NombreMetodo = MethodBase.GetCurrentMethod().Name,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _dc.OSKCView.FirstOrDefaultAsync(x => x.Code == value.Code);

                if (data == null)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = "No se encontró un registro con el código proporcionado.";
                    return resultTransaccion;
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = _am.Map<OSKCEntity>(data);
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        /// <summary>
        /// Obtiene una lista de configuraciones de SKU según un filtro de texto.
        /// </summary>
        public async Task<ResultadoTransaccionEntity<OSKCEntity>> GetListByFiltro(OSKCEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKCEntity>
            {
                NombreMetodo = MethodBase.GetCurrentMethod().Name,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var filtro = value.Filtro ?? "";
                var lista = await _dc.OSKCView.Where(x => x.U_Number.Contains(filtro) || x.U_ItemName.Contains(filtro) || x.U_CardName.Contains(filtro)).ToListAsync();

                if (!lista.Any())
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = "No se encontraron registros con el filtro especificado.";
                    return resultTransaccion;
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {lista.Count}";
                resultTransaccion.dataList = _am.Map<List<OSKCEntity>>(lista);
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetOSKCExcel(OSKCEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>
            {
                NombreMetodo = regex.Match(MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var result = (await GetListByDateRange(value));
                if (result.ResultadoCodigo == -1)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = "No se encontraron registros para generar el reporte";
                    return resultTransaccion;
                }

                var lista = result.dataList.ToList();

                using (var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook, true))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = ExcelHelper.CreateStylesheet();
                    stylesPart.Stylesheet.Save();

                    var sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Reporte" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    var columns = new Columns(
                        new Column() { Min = 1, Max = 1, Width = 20, CustomWidth = true },
                        new Column() { Min = 2, Max = 2, Width = 35, CustomWidth = true },
                        new Column() { Min = 3, Max = 3, Width = 15, CustomWidth = true },
                        new Column() { Min = 4, Max = 4, Width = 15, CustomWidth = true },
                        new Column() { Min = 5, Max = 5, Width = 20, CustomWidth = true },
                        new Column() { Min = 6, Max = 6, Width = 30, CustomWidth = true },
                        new Column() { Min = 7, Max = 7, Width = 70, CustomWidth = true },
                        new Column() { Min = 8, Max = 8, Width = 70, CustomWidth = true },
                        new Column() { Min = 9, Max = 9, Width = 15, CustomWidth = true },
                        new Column() { Min = 10, Max = 10, Width = 15, CustomWidth = true },
                        new Column() { Min = 11, Max = 11, Width = 15, CustomWidth = true },
                        new Column() { Min = 12, Max = 12, Width = 15, CustomWidth = true },
                        new Column() { Min = 13, Max = 13, Width = 15, CustomWidth = true },
                        new Column() { Min = 14, Max = 14, Width = 15, CustomWidth = true },
                        new Column() { Min = 15, Max = 15, Width = 20, CustomWidth = true },
                        new Column() { Min = 16, Max = 16, Width = 15, CustomWidth = true },
                        new Column() { Min = 17, Max = 17, Width = 20, CustomWidth = true },
                        new Column() { Min = 18, Max = 18, Width = 10, CustomWidth = true },
                        new Column() { Min = 19, Max = 19, Width = 15, CustomWidth = true },
                        new Column() { Min = 20, Max = 20, Width = 15, CustomWidth = true },
                        new Column() { Min = 21, Max = 21, Width = 15, CustomWidth = true },
                        new Column() { Min = 22, Max = 22, Width = 30, CustomWidth = true },
                        new Column() { Min = 23, Max = 23, Width = 30, CustomWidth = true },
                        new Column() { Min = 24, Max = 24, Width = 45, CustomWidth = true },
                        new Column() { Min = 25, Max = 25, Width = 50, CustomWidth = true }
                    );
                    worksheetPart.Worksheet.InsertBefore(columns, worksheetPart.Worksheet.GetFirstChild<SheetData>());

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    Row row = new Row { RowIndex = 1, Height = 40, CustomHeight = true };
                    row.Append
                    (
                        ExcelHelper.CreateTextCell("A", 1, "N° CORRELATIVO", 5),
                        ExcelHelper.CreateTextCell("B", 1, "NOMBRE DEL EJECUTIVO COMERCIAL", 5),
                        ExcelHelper.CreateTextCell("C", 1, "STATUS", 5),
                        ExcelHelper.CreateTextCell("D", 1, "FECHA", 5),
                        ExcelHelper.CreateTextCell("E", 1, "GRUPO DE ARTÍCULO", 5),
                        ExcelHelper.CreateTextCell("F", 1, "SUBGRUPO DE ARTÍCULO", 5),
                        ExcelHelper.CreateTextCell("G", 1, "DESCRIPCIÓN DEL SKU", 5),
                        ExcelHelper.CreateTextCell("H", 1, "CLIENTE", 5),
                        ExcelHelper.CreateTextCell("I", 1, "UME", 5),
                        ExcelHelper.CreateTextCell("J", 1, "CANTIDAD", 5),
                        ExcelHelper.CreateTextCell("K", 1, "ANCHO (CM)", 5),
                        ExcelHelper.CreateTextCell("L", 1, "LARGO (MTS)", 5),
                        ExcelHelper.CreateTextCell("M", 1, "GR/M2", 5),
                        ExcelHelper.CreateTextCell("N", 1, "PESO ITEM  (GR)", 5),
                        ExcelHelper.CreateTextCell("O", 1, "COLOR", 5),
                        ExcelHelper.CreateTextCell("P", 1, "LAMINADO", 5),
                        ExcelHelper.CreateTextCell("Q", 1, "TIPO LAMINADO", 5),
                        ExcelHelper.CreateTextCell("R", 1, "LINNER", 5),
                        ExcelHelper.CreateTextCell("S", 1, "PESO LINNER", 5),
                        ExcelHelper.CreateTextCell("T", 1, "IMPRESIÓN", 5),
                        ExcelHelper.CreateTextCell("U", 1, "# COLORES IMP", 5),
                        ExcelHelper.CreateTextCell("V", 1, "UV X MES (TIEMPO DE VIDA)", 5),
                        ExcelHelper.CreateTextCell("W", 1, "VOLÚMENES PROYECTADOS MENSUALES", 5),
                        ExcelHelper.CreateTextCell("X", 1, "PRECIO VENTA X KG $ (NO INCLUYE IGV) PRECIO VENTA X ROLLOS $ (NO INCLUYE IGV)", 5),
                        ExcelHelper.CreateTextCell("Y", 1, "OBSERVACIÓN", 5)
                    );
                    sheetData.Append(row);

                    for (int i = 0; i < lista.Count(); i++)
                    {
                        row = new Row { RowIndex = Convert.ToUInt32(i + 2) };
                        row.Append
                        (
                            ExcelHelper.CreateTextCell("A", Convert.ToUInt32(i + 2), lista[i].U_Number, 1),
                            ExcelHelper.CreateTextCell("B", Convert.ToUInt32(i + 2), lista[i].U_SlpName, 1),
                            ExcelHelper.CreateTextCell("C", Convert.ToUInt32(i + 2), lista[i].U_StatusName, 1),
                            ExcelHelper.CreateDateCell("D", Convert.ToUInt32(i + 2), lista[i].U_DocDate, 3),
                            ExcelHelper.CreateTextCell("E", Convert.ToUInt32(i + 2), lista[i].U_ItmsGrpNam, 1),
                            ExcelHelper.CreateTextCell("F", Convert.ToUInt32(i + 2), lista[i].U_ItmsSGrpNam, 1),
                            ExcelHelper.CreateTextCell("G", Convert.ToUInt32(i + 2), lista[i].U_ItemName, 1),
                            ExcelHelper.CreateTextCell("H", Convert.ToUInt32(i + 2), lista[i].U_CardName, 1),
                            ExcelHelper.CreateTextCell("I", Convert.ToUInt32(i + 2), lista[i].U_UnitMsrName, 1),
                            ExcelHelper.CreateNumberCell("J", Convert.ToUInt32(i + 2), lista[i].U_Quantity, 4),
                            ExcelHelper.CreateNumberCell("K", Convert.ToUInt32(i + 2), lista[i].U_Wide, 4),
                            ExcelHelper.CreateNumberCell("L", Convert.ToUInt32(i + 2), lista[i].U_Long, 4),
                            ExcelHelper.CreateNumberCell("M", Convert.ToUInt32(i + 2), lista[i].U_GrMtSq, 4),
                            ExcelHelper.CreateNumberCell("N", Convert.ToUInt32(i + 2), lista[i].U_ItemWeight, 4),
                            ExcelHelper.CreateTextCell("O", Convert.ToUInt32(i + 2), lista[i].U_ColorName, 1),
                            ExcelHelper.CreateTextCell("P", Convert.ToUInt32(i + 2), lista[i].U_LaminateName, 1),
                            ExcelHelper.CreateTextCell("Q", Convert.ToUInt32(i + 2), lista[i].U_LamTypName, 1),
                            ExcelHelper.CreateTextCell("R", Convert.ToUInt32(i + 2), lista[i].U_LinnerName, 1),
                            ExcelHelper.CreateNumberCell("S", Convert.ToUInt32(i + 2), lista[i].U_LinnWeight, 4),
                            ExcelHelper.CreateTextCell("T", Convert.ToUInt32(i + 2), lista[i].U_PrintName, 1),
                            ExcelHelper.CreateTextCell("U", Convert.ToUInt32(i + 2), lista[i].U_PrintColName, 1),
                            ExcelHelper.CreateTextCell("V", Convert.ToUInt32(i + 2), lista[i].U_UvByMonName, 1),
                            ExcelHelper.CreateNumberCell("W", Convert.ToUInt32(i + 2), lista[i].U_PrjMonVol, 4),
                            ExcelHelper.CreateNumberCell("X", Convert.ToUInt32(i + 2), lista[i].U_Price, 4),
                            ExcelHelper.CreateTextCell("Y", Convert.ToUInt32(i + 2), lista[i].U_Observations, 1)
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
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
    }
}