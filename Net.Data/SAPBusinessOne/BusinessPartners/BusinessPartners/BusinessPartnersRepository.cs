using System;
using System.IO;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
using Microsoft.Extensions.Options;
using Net.Business.Entities.Web;
using Net.Connection.ConnectionSAPBusinessOne;
using SAPbobsCOM;
namespace Net.Data.SAPBusinessOne
{
    public class BusinessPartnersRepository : RepositoryBase<BusinessPartnersEntity>, IBusinessPartnersRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        public BusinessPartnersRepository(IConnectionSQL context, IOptions<ParametrosTokenConfig> tokenConfig, IConfiguration configuration, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _companyProviderSap = companyProviderSap;
            _aplicacionName = GetType().Name;
            _metodoName = "";
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }



        public async Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetListByFilter(BusinessPartnersFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.BusinessPartners
                .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(value.CardType))
                {
                    var cardType = value.CardType.Trim();
                    if (cardType.Length > 0)
                        query = query.Where(n => n.CardType == cardType);
                }

                
                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var bPartner = value.SearchText.Trim();
                    query = query.Where(n =>
                    n.CardCode.Contains(bPartner) ||
                    n.LicTradNum.Contains(bPartner) ||
                    n.CardName.Contains(bPartner));
                }


                var list = await query
                .Select(n => new BusinessPartnersQueryEntity
                {
                    CardCode = n.CardCode,
                    LicTradNum = n.LicTradNum,
                    CardName = n.CardName,
                    GroupName = n.BusinessPartnerGroups.GroupName,
                    SlpName = n.SalesPersons.SlpName,
                    U_BPP_BPAT = n.U_BPP_BPAT,
                })
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
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
        public async Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetListModalByFilter(BusinessPartnersModalFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.BusinessPartners
                .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(value.CardType))
                {
                    var cardType = value.CardType.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (cardType.Length > 0)
                        query = query.Where(n => cardType.Contains(n.CardType));
                }


                if (!string.IsNullOrWhiteSpace(value.TransType))
                {
                    var transType = value.TransType.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (transType.Length > 0)
                        query = query.Where(n => transType.Contains(n.U_BPP_BPAT ?? "N"));
                }


                if (!string.IsNullOrWhiteSpace(value.BusinessPartner))
                {
                    var bPartner = value.BusinessPartner.Trim();
                    query = query.Where(n => 
                    n.CardCode.Contains(bPartner) ||
                    n.LicTradNum.Contains(bPartner) ||
                    n.CardName.Contains(bPartner));
                }


                var list = await query
                .Select(n => new BusinessPartnersQueryEntity
                {
                    CardCode = n.CardCode,
                    LicTradNum = n.LicTradNum,
                    CardName = n.CardName,
                    // LEFT JOIN por CardCode + Name (OCRD.CntctPrsn guarda el nombre)
                    CntctCode = _db.ContactEmployees
                                .Where(c => c.CardCode == n.CardCode && c.Name == n.CntctPrsn)
                                .Select(c => (int?)c.CntctCode)
                                .FirstOrDefault() ?? 0,

                    Address2 = n.MailAddres + " " +
                               n.State.Name + " " +
                               n.MailCounty + " - " +
                               n.MailCity,
                    U_BPP_BPTD = n.U_BPP_BPTD,
                })
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
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
        public async Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetByCode(string cardCode)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.BusinessPartners
                .AsNoTracking()
                .Where(p => p.CardCode == cardCode)
                .Select(p => new BusinessPartnersQueryEntity
                {
                    CardCode = p.CardCode,
                    LicTradNum = p.LicTradNum,
                    CardName = p.CardName,
                    CardType = p.CardType,
                    U_BPP_BPTD = p.U_BPP_BPTD,
                    Currency = p.Currency,

                    LinesCurrency = _db.CurrencyCodes
                                    .Where(c => p.Currency == "##" || c.CurrCode == p.Currency)
                                    .Select(c => new CurrencyCodesEntity
                                    {
                                        CurrCode = c.CurrCode,
                                        CurrName = c.CurrName
                                    })
                                    .ToList(),

                    SlpCode = p.SlpCode,
                    SlpName = p.SalesPersons.SlpName,
                    BillToDef = p.BillToDef,

                    LinesPayAddress = _db.Addresses
                                        .Where(a => a.CardCode == p.CardCode && a.AdresType == "B")
                                        .OrderBy(a => a.LineNum)
                                        .Select(a => new AddressesEntity
                                        {
                                            CardCode = a.CardCode,
                                            AdresType = a.AdresType,
                                            Address = a.Address,
                                            Street = a.Street,
                                            Country  = a.Country,
                                            GlblLocNum = a.GlblLocNum,
                                            City = a.City,
                                            County = a.County,
                                            State = a.State,
                                            LineNum = a.LineNum,
                                            TaxCode = a.TaxCode
                                        })
                                        .ToList(),

                    ShipToDef = p.ShipToDef,

                    LinesShipAddress = _db.Addresses
                                        .Where(a => a.CardCode == p.CardCode && a.AdresType == "S")
                                        .OrderBy(a => a.LineNum)
                                        .Select(a => new AddressesEntity
                                        {
                                            CardCode = a.CardCode,
                                            AdresType = a.AdresType,
                                            Address = a.Address,
                                            Street = a.Street,
                                            Country = a.Country,
                                            GlblLocNum = a.GlblLocNum,
                                            City = a.City,
                                            County = a.County,
                                            State = a.State,
                                            LineNum = a.LineNum,
                                            TaxCode = a.TaxCode
                                        })
                                        .ToList(),

                    GroupNum = p.GroupNum,
                    GroupCode = p.GroupCode,
                    GroupName = p.BusinessPartnerGroups.GroupName,
                    ListNum = p.ListNum,

                    Phone1 = p.Phone1,
                    EmailAddress = p.E_Mail,
                    Cellular = p.Cellular,
                    CreditLine = p.CreditLine ?? 0,

                    // Propiedad primaria de contacto
                    CntctCode = _db.ContactEmployees
                                .Where(c => c.CardCode == p.CardCode && c.Name == p.CntctPrsn)
                                .Select(c => (int?)c.CntctCode)
                                .FirstOrDefault() ?? 0,

                    CntctPrsn = p.CntctPrsn,

                    // Lista completa de personas de contacto
                    ContactEmployees = _db.ContactEmployees
                                        .Where(c => c.CardCode == p.CardCode)
                                        .Select(c => new ContactEmployeesQueryEntity
                                        {
                                            CntctCode = c.CntctCode,
                                            CardCode = c.CardCode,
                                            Name = c.Name,
                                            FullName = (c.FirstName + " " + (c.MiddleName ?? "") + " " + (c.LastName ?? "")).Trim(),
                                            FirstName = c.FirstName,
                                            MiddleName = c.MiddleName,
                                            LastName = c.LastName,
                                            Tel1 = c.Tel1,
                                            Cellolar = c.Cellolar,
                                            E_MailL = c.E_MailL,
                                            Position = c.Position
                                        })
                                        .ToList()
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetVehicleByCode(string cardCode)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.BusinessPartners
                .AsNoTracking()
                .Where(p => p.CardCode == cardCode)
                .Select(p => new BusinessPartnersQueryEntity
                {
                    CardCode = p.CardCode,
                    LicTradNum = p.LicTradNum,
                    CardName = p.CardName,
                    LinesVehicles = p.LinesVehicles.Select(s => new VehiclesQueryEntity
                    {
                        Code = s.Code,
                        Name = s.Name,
                        U_BPP_VEPL = s.U_BPP_VEPL,
                        U_BPP_VEMA = s.U_BPP_VEMA,
                        U_BPP_VEMO = s.U_BPP_VEMO,
                        U_BPP_VEAN = s.U_BPP_VEAN,
                        U_BPP_VECO = s.U_BPP_VECO,
                        U_BPP_VESE = s.U_BPP_VESE,
                        U_BPP_VEPM = s.U_BPP_VEPM ?? 0,
                        U_FIB_COTR = s.U_FIB_COTR
                    }).ToList()
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetDriverByCode(string cardCode)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.BusinessPartners
                .AsNoTracking()
                .Where(p => p.CardCode == cardCode)
                .Select(p => new BusinessPartnersQueryEntity
                {
                    CardCode = p.CardCode,
                    LicTradNum = p.LicTradNum,
                    CardName = p.CardName,
                    LinesDrivers = p.LinesDrivers.Select(s => new DriversQueryEntity
                    {
                        Code = s.Code,
                        Name = s.Name,
                        U_BPP_CHNO = s.U_BPP_CHNO,
                        U_FIB_CHAP = s.U_FIB_CHAP,
                        U_FIB_CHTD = s.U_FIB_CHTD,
                        U_FIB_CHND = s.U_FIB_CHND,
                        U_BPP_CHLI = s.U_BPP_CHLI,
                        U_FIB_COTR = s.U_FIB_COTR
                    }).ToList()
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<BusinessPartnersViewEntity>> GetListClienteBySectorStatus(BusinessPartnersSectorStatusFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersViewEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.BusinessPartnersView
                .AsNoTracking()
                .Where(n => n.CardType == "C"); // Solo clientes

                if (!string.IsNullOrWhiteSpace(value.Sector))
                {
                    var sectores = value.Sector.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (sectores.Length > 0)
                        query = query.Where(n => sectores.Contains(n.CodSector));
                }

                if (!string.IsNullOrWhiteSpace(value.Status))
                {
                    var status = value.Status.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (status.Length > 0)
                        query = query.Where(n => status.Contains(n.CodStatus));
                }

                //if (!string.IsNullOrWhiteSpace(value.BusinessPartner))
                //{
                //    // Deja un solo espacio y reemplaza por %
                //    var filter = Regex.Replace(value.BusinessPartner.Trim(), @"\s+", " ").Replace(" ", "%");

                //    query = query.Where(x =>
                //        EF.Functions.Like(EF.Functions.Collate(x.CardCode!, GlobalVariables.CI), $"%{filter}%") ||
                //        EF.Functions.Like(EF.Functions.Collate(x.CardName!, GlobalVariables.CI), $"%{filter}%")
                //    );
                //}


                var data = await query
                .Select(n => new
                {
                    n.CardCode,
                    n.LicTradNum,
                    n.CardName,
                    n.UnidadNegocio,
                    n.DocType,
                    n.CreditLine,
                    n.CodStatus,
                    n.NomStatus,
                    n.SlpName,
                    n.Address,
                    n.NomSector,
                    n.NomDivision,
                    n.Pais,
                    n.NomDepartamento,
                    n.NomProvincia,
                    n.NomDistrito,
                    n.Ubigeo,
                    n.Tel1,
                    n.Tel2,
                    n.Movil,
                    n.Email,
                    n.CreateDate,
                    n.LowDate,
                    n.FechaUltimaVenta
                })
                .ToListAsync();


                var list = data
                .GroupBy(n => n.CardCode)
                .Select(g => g.First())
                .Select(n => new BusinessPartnersViewEntity
                {
                    CardCode = n.CardCode,
                    LicTradNum = n.LicTradNum,
                    CardName = n.CardName,
                    UnidadNegocio = n.UnidadNegocio,
                    DocType = n.DocType,
                    CreditLine = n.CreditLine,
                    CodStatus = n.CodStatus,
                    NomStatus = n.NomStatus,
                    SlpName = n.SlpName,
                    Address = n.Address,
                    NomSector = n.NomSector,
                    NomDivision = n.NomDivision,
                    Pais = n.Pais,
                    NomDepartamento = n.NomDepartamento,
                    NomProvincia = n.NomProvincia,
                    NomDistrito = n.NomDistrito,
                    Ubigeo = n.Ubigeo,
                    Tel1 = n.Tel1,
                    Tel2 = n.Tel2,
                    Movil = n.Movil,
                    Email = n.Email,
                    CreateDate = n.CreateDate,
                    LowDate = n.LowDate,
                    FechaUltimaVenta = n.FechaUltimaVenta
                })
                .OrderBy(x => x.CardCode)
                .ToList();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
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
        public async Task<ResultadoTransaccionEntity<BusinessPartnersViewEntity>> GetLitClienteContactoBySectorStatus(BusinessPartnersSectorStatusFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersViewEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.BusinessPartnersView
                .AsNoTracking()
                .Where(n => n.CardType == "C"); // Solo clientes

                if (!string.IsNullOrWhiteSpace(value.Sector))
                {
                    var sectores = value.Sector.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (sectores.Length > 0)
                        query = query.Where(n => sectores.Contains(n.CodSector));
                }

                if (!string.IsNullOrWhiteSpace(value.Status))
                {
                    var status = value.Status.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (status.Length > 0)
                        query = query.Where(n => status.Contains(n.CodStatus));
                }

                //if (!string.IsNullOrWhiteSpace(value.BusinessPartner))
                //{
                //    // Deja un solo espacio y reemplaza por %
                //    var filter = Regex.Replace(value.BusinessPartner.Trim(), @"\s+", " ").Replace(" ", "%");

                //    query = query.Where(x =>
                //        EF.Functions.Like(EF.Functions.Collate(x.CardCode!, GlobalVariables.CI), $"%{filter}%") ||
                //        EF.Functions.Like(EF.Functions.Collate(x.CardName!, GlobalVariables.CI), $"%{filter}%")
                //    );
                //}

                var list = await query
                .Select(n => new BusinessPartnersViewEntity
                {
                    CardCode = n.CardCode,
                    LicTradNum = n.LicTradNum,
                    CardName = n.CardName,
                    CodStatus = n.CodStatus,
                    NomStatus = n.NomStatus,
                    SlpName = n.SlpName,
                    Address = n.Address,
                    NomSector = n.NomSector,
                    NomDivision = n.NomDivision,
                    Pais = n.Pais,
                    NomDepartamento = n.NomDepartamento,
                    NomProvincia = n.NomProvincia,
                    NomDistrito = n.NomDistrito,
                    Ubigeo = n.Ubigeo,
                    NomContacto = n.NomContacto,
                    TelContacto1 = n.TelContacto1,
                    TelContacto2 = n.TelContacto2,
                    MovilContacto = n.MovilContacto,
                })
                .OrderBy(x => x.CardCode)
                .ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
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
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetClienteBySectorStatusExcel(BusinessPartnersSectorStatusFilterEntity value)
        {
            var ms = new MemoryStream();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var objectGetCliente = await GetListClienteBySectorStatus(value);
                var objectGetClienteContacto = await GetLitClienteContactoBySectorStatus(value);
                ms = GetArchivoClienteBySectorStatusExcel(objectGetCliente.dataList.ToList(), objectGetClienteContacto.dataList.ToList());

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
        private MemoryStream GetArchivoClienteBySectorStatusExcel(List<BusinessPartnersViewEntity> value1, List<BusinessPartnersViewEntity> value2)
        {
            var ms = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                SetArchivoClienteBySectorStatusExcel(document, workbookPart, sheets, value1);
                SetArchivoClienteContactoBySectorStatusExcel(document, workbookPart, sheets, value2);

                workbookPart.Workbook.Save();
                document.Close();
            }

            return ms;
        }
        private void SetArchivoClienteBySectorStatusExcel(SpreadsheetDocument document, WorkbookPart workbookPart, Sheets sheets, List<BusinessPartnersViewEntity> value)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Cliente" };
            sheets.Append(sheet);

            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

            //Cabecera
            Row row = new Row();
            row.Append(
            ExportToExcel.ConstructCell("Código", CellValues.String),
            ExportToExcel.ConstructCell("Documento", CellValues.String),
            ExportToExcel.ConstructCell("Tipo de documento", CellValues.String),
            ExportToExcel.ConstructCell("Nombre", CellValues.String),
            ExportToExcel.ConstructCell("Unidad de Negocio", CellValues.String),
            ExportToExcel.ConstructCell("Línea de crédito", CellValues.String),
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
                ExportToExcel.ConstructCell(item.DocType, CellValues.String),
                ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                ExportToExcel.ConstructCell(item.UnidadNegocio, CellValues.String),
                ExportToExcel.ConstructCell(item.CreditLine.ToString(), CellValues.Number),
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
        private void SetArchivoClienteContactoBySectorStatusExcel(SpreadsheetDocument document, WorkbookPart workbookPart, Sheets sheets, List<BusinessPartnersViewEntity> value)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 2, Name = "Cliente-Contacto" };
            sheets.Append(sheet);

            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

            //Cabecera
            Row row = new Row();
            row.Append(
            ExportToExcel.ConstructCell("Código", CellValues.String),
            ExportToExcel.ConstructCell("RUC", CellValues.String),
            ExportToExcel.ConstructCell("Nombre", CellValues.String),
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

        public async Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> SetCreate(BusinessPartnersCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            BusinessPartners bp = null;

            return await Task.Run(() =>
            {
                try
                {
                    var company = _companyProviderSap.GetCompany();
                    bp = company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                    if (!string.IsNullOrEmpty(value.CardCode)) bp.CardCode = value.CardCode;
                    bp.CardName = value.CardName;

                    if (!string.IsNullOrEmpty(value.CardType))
                    {
                        bp.CardType = value.CardType == "S" ? BoCardTypes.cSupplier : value.CardType == "L" ? BoCardTypes.cLid : BoCardTypes.cCustomer;
                    }
                    if (value.GroupCode.HasValue) bp.GroupCode = value.GroupCode.Value;
                    if (!string.IsNullOrEmpty(value.LicTradNum)) bp.FederalTaxID = value.LicTradNum;
                    if (!string.IsNullOrEmpty(value.Phone1)) bp.Phone1 = value.Phone1;
                    if (!string.IsNullOrEmpty(value.Cellular)) bp.Cellular = value.Cellular;
                    if (!string.IsNullOrEmpty(value.EmailAddress)) bp.EmailAddress = value.EmailAddress;
                    if (!string.IsNullOrEmpty(value.Address)) bp.Address = value.Address;
                    if (value.SlpCode.HasValue && value.SlpCode > 0) bp.SalesPersonCode = value.SlpCode.Value;
                    if (value.GroupNum.HasValue) bp.PayTermsGrpCode = value.GroupNum.Value;
                    if (value.CreditLine.HasValue) bp.CreditLimit = (double)value.CreditLine.Value;
                    if (value.DebitLine.HasValue) bp.MaxCommitment = (double)value.DebitLine.Value;
                    if (!string.IsNullOrEmpty(value.CntctPrsn)) bp.ContactPerson = value.CntctPrsn;
                    if (!string.IsNullOrEmpty(value.Currency)) bp.Currency = value.Currency;
                    if (!string.IsNullOrEmpty(value.Notes)) bp.Notes = value.Notes;

                    // UDFs
                    if (!string.IsNullOrEmpty(value.U_BPP_BPAT)) bp.UserFields.Fields.Item("U_BPP_BPAT").Value = value.U_BPP_BPAT;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPTD)) bp.UserFields.Fields.Item("U_BPP_BPTD").Value = value.U_BPP_BPTD;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPTP)) bp.UserFields.Fields.Item("U_BPP_BPTP").Value = value.U_BPP_BPTP;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPN1)) bp.UserFields.Fields.Item("U_BPP_BPN1").Value = value.U_BPP_BPN1;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPN2)) bp.UserFields.Fields.Item("U_BPP_BPN2").Value = value.U_BPP_BPN2;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPNO)) bp.UserFields.Fields.Item("U_BPP_BPNO").Value = value.U_BPP_BPNO;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPAP)) bp.UserFields.Fields.Item("U_BPP_BPAP").Value = value.U_BPP_BPAP;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPAM)) bp.UserFields.Fields.Item("U_BPP_BPAM").Value = value.U_BPP_BPAM;
                    if (!string.IsNullOrEmpty(value.U_FIB_Divi)) bp.UserFields.Fields.Item("U_FIB_Divi").Value = value.U_FIB_Divi;
                    if (!string.IsNullOrEmpty(value.U_FIB_Sector)) bp.UserFields.Fields.Item("U_FIB_Sector").Value = value.U_FIB_Sector;

                    // Direcciones
                    if (value.Addresses != null)
                    {
                        int addrCount = 0;
                        foreach (var addr in value.Addresses)
                        {
                            if (addrCount > 0) bp.Addresses.Add();
                            bp.Addresses.AddressName = addr.AddressName;
                            bp.Addresses.AddressType = addr.AddressType == "S" ? BoAddressType.bo_ShipTo : BoAddressType.bo_BillTo;
                            bp.Addresses.Street = addr.Street;
                            bp.Addresses.Block = addr.Block;
                            bp.Addresses.City = addr.City;
                            bp.Addresses.ZipCode = addr.ZipCode;
                            bp.Addresses.County = addr.County;
                            bp.Addresses.State = addr.State;
                            bp.Addresses.Country = addr.Country;
                            bp.Addresses.BuildingFloorRoom = addr.BuildingFloorRoom;
                            if (!string.IsNullOrEmpty(addr.GlblLocNum)) bp.Addresses.GlobalLocationNumber = addr.GlblLocNum;
                            addrCount++;
                        }
                    }

                    // Contactos
                    if (value.ContactEmployees != null)
                    {
                        int contactCount = 0;
                        foreach (var contact in value.ContactEmployees)
                        {
                            if (contactCount > 0) bp.ContactEmployees.Add();
                            bp.ContactEmployees.Name = contact.Name;
                            bp.ContactEmployees.FirstName = contact.FirstName;
                            bp.ContactEmployees.MiddleName = contact.MiddleName;
                            bp.ContactEmployees.LastName = contact.LastName;
                            bp.ContactEmployees.Title = contact.Title;
                            bp.ContactEmployees.Position = contact.Position;
                            bp.ContactEmployees.Address = contact.Address;
                            bp.ContactEmployees.Phone1 = contact.Phone1;
                            bp.ContactEmployees.Phone2 = contact.Phone2;
                            bp.ContactEmployees.MobilePhone = contact.MobilePhone;
                            bp.ContactEmployees.E_Mail = contact.E_Mail;
                            contactCount++;
                        }
                    }

                    int reg = bp.Add();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = company.GetNewObjectKey();
                    }
                    else
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Error SAP {errorCode}: {errorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message;
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(bp);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> SetUpdate(BusinessPartnersUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            BusinessPartners bp = null;

            return await Task.Run(() =>
            {
                try
                {
                    var company = _companyProviderSap.GetCompany();
                    bp = company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                    if (!bp.GetByKey(value.CardCode))
                    {
                        company.GetLastError(out int errCode, out string errMsg);
                        throw new Exception($"No se encontró el socio de negocio {value.CardCode}. Error SAP {errCode}: {errMsg}");
                    }

                    if (!string.IsNullOrEmpty(value.CardName)) bp.CardName = value.CardName;

                    if (!string.IsNullOrEmpty(value.CardType))
                    {
                        bp.CardType = value.CardType == "S" ? BoCardTypes.cSupplier : value.CardType == "L" ? BoCardTypes.cLid : BoCardTypes.cCustomer;
                    }
                    if (value.GroupCode.HasValue) bp.GroupCode = value.GroupCode.Value;
                    if (!string.IsNullOrEmpty(value.LicTradNum)) bp.FederalTaxID = value.LicTradNum;
                    if (!string.IsNullOrEmpty(value.Phone1)) bp.Phone1 = value.Phone1;
                    if (!string.IsNullOrEmpty(value.Cellular)) bp.Cellular = value.Cellular;
                    if (!string.IsNullOrEmpty(value.EmailAddress)) bp.EmailAddress = value.EmailAddress;
                    if (!string.IsNullOrEmpty(value.Address)) bp.Address = value.Address;
                    if (value.SlpCode.HasValue && value.SlpCode > 0) bp.SalesPersonCode = value.SlpCode.Value;
                    if (value.GroupNum.HasValue) bp.PayTermsGrpCode = value.GroupNum.Value;
                    if (value.CreditLine.HasValue) bp.CreditLimit = (double)value.CreditLine.Value;
                    if (value.DebitLine.HasValue) bp.MaxCommitment = (double)value.DebitLine.Value;
                    if (!string.IsNullOrEmpty(value.CntctPrsn)) bp.ContactPerson = value.CntctPrsn;
                    if (!string.IsNullOrEmpty(value.Currency)) bp.Currency = value.Currency;
                    if (!string.IsNullOrEmpty(value.Notes)) bp.Notes = value.Notes;

                    // UDFs
                    if (!string.IsNullOrEmpty(value.U_BPP_BPAT)) bp.UserFields.Fields.Item("U_BPP_BPAT").Value = value.U_BPP_BPAT;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPTD)) bp.UserFields.Fields.Item("U_BPP_BPTD").Value = value.U_BPP_BPTD;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPTP)) bp.UserFields.Fields.Item("U_BPP_BPTP").Value = value.U_BPP_BPTP;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPN1)) bp.UserFields.Fields.Item("U_BPP_BPN1").Value = value.U_BPP_BPN1;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPN2)) bp.UserFields.Fields.Item("U_BPP_BPN2").Value = value.U_BPP_BPN2;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPNO)) bp.UserFields.Fields.Item("U_BPP_BPNO").Value = value.U_BPP_BPNO;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPAP)) bp.UserFields.Fields.Item("U_BPP_BPAP").Value = value.U_BPP_BPAP;
                    if (!string.IsNullOrEmpty(value.U_BPP_BPAM)) bp.UserFields.Fields.Item("U_BPP_BPAM").Value = value.U_BPP_BPAM;
                    if (!string.IsNullOrEmpty(value.U_FIB_Divi)) bp.UserFields.Fields.Item("U_FIB_Divi").Value = value.U_FIB_Divi;
                    if (!string.IsNullOrEmpty(value.U_FIB_Sector)) bp.UserFields.Fields.Item("U_FIB_Sector").Value = value.U_FIB_Sector;

                    // Direcciones
                    if (value.Addresses != null)
                    {
                        foreach (var addr in value.Addresses)
                        {
                            bool found = false;
                            var targetedType = addr.AddressType == "S" ? BoAddressType.bo_ShipTo : BoAddressType.bo_BillTo;

                            for (int i = 0; i < bp.Addresses.Count; i++)
                            {
                                bp.Addresses.SetCurrentLine(i);
                                if (bp.Addresses.AddressName == addr.AddressName && bp.Addresses.AddressType == targetedType)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                bp.Addresses.Add();
                                bp.Addresses.AddressName = addr.AddressName;
                                bp.Addresses.AddressType = targetedType;
                            }

                            bp.Addresses.Street = addr.Street;
                            bp.Addresses.Block = addr.Block;
                            bp.Addresses.City = addr.City;
                            bp.Addresses.ZipCode = addr.ZipCode;
                            bp.Addresses.County = addr.County;
                            bp.Addresses.State = addr.State;
                            bp.Addresses.Country = addr.Country;
                            bp.Addresses.BuildingFloorRoom = addr.BuildingFloorRoom;
                            if (!string.IsNullOrEmpty(addr.GlblLocNum)) bp.Addresses.GlobalLocationNumber = addr.GlblLocNum;
                        }
                    }

                    // Contactos
                    if (value.ContactEmployees != null)
                    {
                        foreach (var contact in value.ContactEmployees)
                        {
                            bool found = false;
                            for (int i = 0; i < bp.ContactEmployees.Count; i++)
                            {
                                bp.ContactEmployees.SetCurrentLine(i);
                                if (bp.ContactEmployees.Name == contact.Name)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                bp.ContactEmployees.Add();
                                bp.ContactEmployees.Name = contact.Name;
                            }

                            bp.ContactEmployees.FirstName = contact.FirstName;
                            bp.ContactEmployees.MiddleName = contact.MiddleName;
                            bp.ContactEmployees.LastName = contact.LastName;
                            bp.ContactEmployees.Title = contact.Title;
                            bp.ContactEmployees.Position = contact.Position;
                            bp.ContactEmployees.Address = contact.Address;
                            bp.ContactEmployees.Phone1 = contact.Phone1;
                            bp.ContactEmployees.Phone2 = contact.Phone2;
                            bp.ContactEmployees.MobilePhone = contact.MobilePhone;
                            bp.ContactEmployees.E_Mail = contact.E_Mail;
                        }
                    }

                    int reg = bp.Update();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "Actualizado con éxito.";
                    }
                    else
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Error SAP {errorCode}: {errorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message;
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(bp);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> SetDelete(string cardCode)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            BusinessPartners bp = null;

            return await Task.Run(() =>
            {
                try
                {
                    var company = _companyProviderSap.GetCompany();
                    bp = company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                    if (!bp.GetByKey(cardCode))
                    {
                        throw new Exception($"No se encontró el socio de negocio {cardCode}");
                    }

                    int reg = bp.Remove();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "Registro eliminado con éxito.";
                    }
                    else
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Error SAP {errorCode}: {errorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message;
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(bp);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionEntity<BusinessPartnersQueryEntity>> GetByRUC(string ruc)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.BusinessPartners
                .AsNoTracking()
                .Where(p => p.LicTradNum == ruc)
                .Select(p => new BusinessPartnersQueryEntity
                {
                    CardCode = p.CardCode,
                    LicTradNum = p.LicTradNum,
                    CardName = p.CardName,
                    U_BPP_BPTD = p.U_BPP_BPTD
                })
                .FirstOrDefaultAsync();

                if (data == null)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = "No se encontró el RUC especificado.";
                }
                else
                {
                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                    resultTransaccion.data = data;
                }
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
