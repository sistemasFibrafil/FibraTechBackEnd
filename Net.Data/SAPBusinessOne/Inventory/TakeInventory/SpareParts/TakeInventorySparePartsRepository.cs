using System;
using System.IO;
using SAPbobsCOM;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class TakeInventorySparePartsRepository : RepositoryBase<TakeInventorySparePartsEntity>, ITakeInventorySparePartsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSeguridad _dbSeg;
        private readonly DataContextSAPBusinessOne _dbSap;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;
        
        public TakeInventorySparePartsRepository(IConnectionSQL context, IConfiguration configuration, DataContextSeguridad dbSeg, DataContextSAPBusinessOne dbSap, CompanyProviderSAPBusinessOne companyProviderSap)
           : base(context)
        {
            _dbSeg = dbSeg;
            _dbSap = dbSap;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }


        public async Task<ResultadoTransaccionResponse<TakeInventorySparePartsEntity>> GetListByFilter(TakeInventorySparePartsFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<TakeInventorySparePartsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query =_dbSap.TakeInventorySpareParts
                .AsNoTracking()
                .Where(n => n.U_IsDelete == "N" && n.U_CreateDate >= value.StartDate && n.U_CreateDate <= value.EndDate);


                // FILTRO POR USUARIO
                if (!string.IsNullOrWhiteSpace(value.Usuario))
                {
                    var usuarios = value.Usuario.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    query = query.Where(n => usuarios.Contains(n.U_UsrCreate));
                }


                // FILTRO POR ALMACÉN
                if (!string.IsNullOrWhiteSpace(value.WhsCode))
                {
                    var warehouses = value.WhsCode.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(n => warehouses.Contains(n.U_WhsCode));
                }


                // FILTRO POR ITEM
                if (!string.IsNullOrWhiteSpace(value.Item))
                {
                    var filter = value.Item.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(EF.Functions.Collate(n.U_ItemCode!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(n.U_Dscription!, GlobalVariables.CI), $"%{filter}%")
                    );
                }


                // AGRUPACIÓN (igual a tu consulta original)
                var list = await query
                .Select(g => new TakeInventorySparePartsEntity
                {
                    DocEntry = g.DocEntry,
                    U_ItemCode = g.U_ItemCode,
                    U_Dscription = g.U_Dscription,
                    U_WhsCode = g.U_WhsCode,
                    U_UnitMsr = g.U_UnitMsr,
                    U_OnHandPhy = g.U_OnHandPhy,
                    U_OnHandSys = g.U_OnHandSys,
                    U_Difference = g.U_Difference
                })
                .OrderBy(x => x.U_ItemCode)
                .ThenByDescending(x => x.DocEntry)
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

        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetExcelByFilter(TakeInventorySparePartsFilterEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>
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
                        ExportToExcel.ConstructCell("Usuario", CellValues.String),
                        ExportToExcel.ConstructCell("Fecha de registro", CellValues.String),
                        ExportToExcel.ConstructCell("Hora de registro", CellValues.String)
                    );
                    sheetData.AppendChild(row);

                    // =======================================================
                    // 1. CARGAR USUARIOS (CON PERSONA) EN MEMORIA
                    // =======================================================

                    var usuarioDictionary = await _dbSeg.Usuario
                    .Select( x => new
                        {
                            x.IdUsuario,
                            U_UsrNameCreate = x.Nombre + " " + x.ApellidoPaterno
                        })
                    .ToDictionaryAsync(x => x.IdUsuario, x => x.U_UsrNameCreate);

                    var query =
                    from n in _dbSap.TakeInventorySpareParts
                    where n.U_IsDelete == "N" && n.U_CreateDate >= value.StartDate && n.U_CreateDate <= value.EndDate
                    select new
                    {
                        n.U_ItemCode,
                        n.U_Dscription,
                        n.U_WhsCode,
                        n.U_UnitMsr,
                        n.U_OnHandPhy,
                        n.U_OnHandSys,
                        n.U_Difference,
                        n.U_UsrCreate,
                        n.U_CreateDate,
                        n.U_CreateTime
                    };

                    // FILTRO POR USUARIO
                    if (!string.IsNullOrWhiteSpace(value.Usuario))
                    {
                        var usuarios = value.Usuario.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                        query = query.Where(x => usuarios.Contains(x.U_UsrCreate));
                    }


                    // FILTRO POR ALMACÉN
                    if (!string.IsNullOrWhiteSpace(value.WhsCode))
                    {
                        var warehouses = value.WhsCode.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                        query = query.Where(x => warehouses.Contains(x.U_WhsCode));
                    }


                    // FILTRO POR ITEM
                    if (!string.IsNullOrWhiteSpace(value.Item))
                    {
                        var filter = value.Item.Trim();

                        query = query.Where(x =>
                            EF.Functions.Like(EF.Functions.Collate(x.U_ItemCode!, GlobalVariables.CI), $"%{filter}%") ||
                            EF.Functions.Like(EF.Functions.Collate(x.U_Dscription!, GlobalVariables.CI), $"%{filter}%")
                        );
                    }


                    var list = await query
                    .AsNoTracking()
                    .ToListAsync();

                    var result = list
                    .Select(x => new TakeInventorySparePartsQueryEntity
                    {
                        U_ItemCode = x.U_ItemCode,
                        U_Dscription = x.U_Dscription,
                        U_WhsCode = x.U_WhsCode,
                        U_UnitMsr = x.U_UnitMsr,
                        U_OnHandPhy = x.U_OnHandPhy,
                        U_OnHandSys = x.U_OnHandSys,
                        U_Difference = x.U_Difference,
                        U_UsrNameCreate = usuarioDictionary[x.U_UsrCreate],
                        U_CreateDate = x.U_CreateDate,
                        U_CreateTime = x.U_CreateTime
                    })
                    .OrderBy(x => x.U_ItemCode)
                    .ThenByDescending(x => x.U_CreateDate)
                    .ThenByDescending(x => x.U_CreateTime)
                    .ToList();

                    //Contenido
                    foreach (var item in result)
                    {
                        row = new Row();
                        row.Append
                        (
                            ExportToExcel.ConstructCell(item.U_ItemCode, CellValues.String),
                            ExportToExcel.ConstructCell(item.U_Dscription, CellValues.String),
                            ExportToExcel.ConstructCell(item.U_WhsCode, CellValues.String),
                            ExportToExcel.ConstructCell(item.U_UnitMsr, CellValues.String),
                            ExportToExcel.ConstructCell(item.U_OnHandPhy.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.U_OnHandSys.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.U_Difference.ToString(), CellValues.Number),
                            ExportToExcel.ConstructCell(item.U_UsrNameCreate.ToString(), CellValues.String),
                            ExportToExcel.ConstructCell(item.U_CreateDate.ToString("dd/MM/yyyy"), CellValues.String),
                            ExportToExcel.ConstructCell(item.U_CreateTime.ToString("D4").Substring(0, 2) + ":" + item.U_CreateTime.ToString("D4").Substring(2, 2), CellValues.String)
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

        public async Task<ResultadoTransaccionResponse<TakeInventorySparePartsEntity>> GetListCurrentDate(TakeInventorySparePartsFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<TakeInventorySparePartsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _dbSap.TakeInventorySpareParts
                .AsNoTracking()
                .Where(n => n.U_IsDelete == "N" && n.U_WhsCode == value.U_WhsCode && n.U_UsrCreate == value.U_UsrCreate && n.U_CreateDate == value.U_CreateDate);

                var list = await query
                .Select(g => new TakeInventorySparePartsEntity
                {
                    DocEntry = g.DocEntry,
                    U_ItemCode = g.U_ItemCode,
                    U_Dscription = g.U_Dscription,
                    U_WhsCode = g.U_WhsCode,
                    U_UnitMsr = g.U_UnitMsr,
                    U_OnHandPhy = g.U_OnHandPhy,
                    U_OnHandSys = g.U_OnHandSys,
                    U_Difference = g.U_Difference,
                })
                .OrderBy(x => x.U_ItemCode)
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

        public async Task<ResultadoTransaccionResponse<TakeInventorySparePartsEntity>> SetCreate(TakeInventorySparePartsCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<TakeInventorySparePartsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            GeneralService oGeneralService = null;

            try
            {
                // Se valida que el código de barras no se encuentre registrado en el picking
                var cantidadPicking = await _dbSap.TakeInventorySpareParts.CountAsync(n => n.U_IsDelete == "N" && n.U_ItemCode == value.U_CodeBar && n.U_WhsCode == value.U_WhsCode && n.U_UsrCreate == value.U_UsrCreate && n.U_CreateDate == value.U_CreateDate);

                if (cantidadPicking > 0)
                {
                    throw new Exception("Ya existe en esta fecha..");
                }


                // Se obtiene los datos de articulo relacionados al código de barras ingresado
                var item = await _dbSap.Items
                .Where(n => n.ItemCode == value.U_CodeBar)
                .Select(x => new
                {
                    x.ItemCode,
                    x.ItemName,
                    x.InvntryUom
                })
                .FirstOrDefaultAsync() ?? throw new Exception("Código de barras no existe en SAP B1.");


                // Se obtiene los datos de almacén relacionados al código de barras ingresado
                var itemWarehouseInfo = await _dbSap.ItemWarehouseInfo
                .Where(n => n.ItemCode == value.U_CodeBar && n.WhsCode == value.U_WhsCode)
                .Select(x => new
                {
                    x.WhsCode,
                    x.OnHand
                })
                .FirstOrDefaultAsync() ?? throw new Exception("El alamcén no existe en SAP B1.");


                // Conexión a SAP
                company = _companyProviderSap.GetCompany();


                // Se instancia el servicio de objetos generales
                oCompService = company.GetCompanyService();
                // se instancia el objeto de datos generales
                oGeneralService = oCompService.GetGeneralService("FIB_OTIS");
                // se instancia el objeto de datos generales
                oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);


                // Se asignan los valores a las propiedades del objeto de datos generales
                oGeneralData.SetProperty("U_ItemCode", item.ItemCode);
                oGeneralData.SetProperty("U_Dscription", item.ItemName);
                oGeneralData.SetProperty("U_WhsCode", itemWarehouseInfo.WhsCode);
                oGeneralData.SetProperty("U_UnitMsr", item.InvntryUom);
                oGeneralData.SetProperty("U_OnHandSys", Convert.ToDouble(Math.Round(Convert.ToDouble(itemWarehouseInfo.OnHand), 6)));
                oGeneralData.SetProperty("U_UsrCreate", value.U_UsrCreate);
                oGeneralData.SetProperty("U_CreateDate", value.U_CreateDate);
                oGeneralData.SetProperty("U_CreateTime", value.U_CreateTime);
                // Se crea el registro
                oGeneralService.Add(oGeneralData);


                var list = await _dbSap.TakeInventorySpareParts
                .Where(n => n.U_IsDelete == "N" && n.U_WhsCode == value.U_WhsCode && n.U_UsrCreate == value.U_UsrCreate && n.U_CreateDate == value.U_CreateDate)
                .Select(n => new TakeInventorySparePartsEntity
                {
                    DocEntry = n.DocEntry,
                    U_ItemCode = n.U_ItemCode,
                    U_Dscription = n.U_Dscription,
                    U_WhsCode = n.U_WhsCode,
                    U_UnitMsr = n.U_UnitMsr,
                    U_OnHandPhy = n.U_OnHandPhy,
                    U_OnHandSys = n.U_OnHandSys,
                    U_Difference = n.U_Difference
                }).ToListAsync();


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
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(oGeneralData, oCompService, oGeneralService);
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<TakeInventorySparePartsEntity>> SetUpdate(TakeInventorySparePartsUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<TakeInventorySparePartsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralDataParams = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Se instancia el servicio de objetos generales
                    oCompService = company.GetCompanyService();

                    // Se instancia el objeto de datos generales
                    oGeneralService = oCompService.GetGeneralService("FIB_OTIS");

                    // Se instancia el objeto de parámetros de datos generales
                    oGeneralDataParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);

                    // Se asigna el DocEntry del registro a actualizar
                    oGeneralDataParams.SetProperty("DocEntry", value.DocEntry);

                    // Se obtiene el registro a actualizar
                    oGeneralData = oGeneralService.GetByParams(oGeneralDataParams);

                    // Se asignan los valores a las propiedades del objeto de datos generales
                    oGeneralData.SetProperty("U_OnHandPhy", Convert.ToDouble(Math.Round(Convert.ToDouble(value.U_OnHandPhy), 6)));
                    oGeneralData.SetProperty("U_Difference", Convert.ToDouble(Math.Round(Convert.ToDouble(value.U_Difference), 6)));
                    oGeneralData.SetProperty("U_UsrUpdate", value.U_UsrUpdate);
                    oGeneralData.SetProperty("U_UpdateDate", value.U_UpdateDate);
                    oGeneralData.SetProperty("U_UpdateTime", value.U_UpdateTime);
                    // Se actualizar el registro
                    oGeneralService.Update(oGeneralData);


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Realizado con éxito.";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(oGeneralData, oCompService, oGeneralService, oGeneralDataParams);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionResponse<TakeInventorySparePartsEntity>> SetDelete(TakeInventorySparePartsDeleteEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<TakeInventorySparePartsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralDataParams = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Se instancia el servicio de objetos generales
                    oCompService = company.GetCompanyService();

                    // Se instancia el objeto de datos generales
                    oGeneralService = oCompService.GetGeneralService("FIB_OTIS");

                    // Se instancia el objeto de parámetros de datos generales
                    oGeneralDataParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);

                    // Se asigna el DocEntry del registro a actualizar
                    oGeneralDataParams.SetProperty("DocEntry", value.DocEntry);

                    // Se obtiene el registro a actualizar
                    oGeneralData = oGeneralService.GetByParams(oGeneralDataParams);

                    // Se asignan los valores a las propiedades del objeto de datos generales
                    oGeneralData.SetProperty("U_IsDelete", value.U_IsDelete);
                    oGeneralData.SetProperty("U_UsrDelete", value.U_UsrDelete);
                    oGeneralData.SetProperty("U_DeleteDate", value.U_DeleteDate);
                    oGeneralData.SetProperty("U_DeleteTime", value.U_DeleteTime);
                    // Se actualizar el registro
                    oGeneralService.Update(oGeneralData);


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Realizado con éxito.";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(oGeneralData, oCompService, oGeneralService, oGeneralDataParams);
                }

                return resultTransaccion;
            });
        }
    }
}
