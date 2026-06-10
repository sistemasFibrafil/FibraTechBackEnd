using System;
using System.IO;
using SAPbobsCOM;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.BusinessPartners.BusinessPartnerGroups;
namespace Net.Data.SAPBusinessOne
{
    public class BusinessPartnersRepository : RepositoryBase<BusinessPartnersEntity>, IBusinessPartnersRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        public BusinessPartnersRepository(IConnectionSQL context, IOptions<ParametrosTokenConfig> tokenConfig, IConfiguration configuration, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }



        #region <<< CONSULTAS >>>

        public async Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetListByFilter(BusinessPartnersFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersQueryEntity>
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
                    var filter = value.SearchText.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.CardCode, $"%{filter}%") ||
                        EF.Functions.Like(n.LicTradNum, $"%{filter}%") ||
                        EF.Functions.Like(n.CardName, $"%{filter}%")
                    );
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
                resultTransaccion.DataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetListModalByFilter(BusinessPartnersModalFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersQueryEntity>
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
                    var filter = value.BusinessPartner.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.CardCode, $"%{filter}%") ||
                        EF.Functions.Like(n.LicTradNum, $"%{filter}%") ||
                        EF.Functions.Like(n.CardName, $"%{filter}%")
                    );
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
                })
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
                resultTransaccion.DataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetByCode(string cardCode)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersQueryEntity>
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
                    GroupCode = p.GroupCode,
                    GroupsLines = _db.BusinessPartnerGroups
                                    .Where(c => c.GroupType == p.CardType)
                                    .Select(c => new BusinessPartnerGroupsEntity
                                    {
                                        GroupCode = c.GroupCode,
                                        GroupName = c.GroupName
                                    })
                                    .ToList(),
                    Currency = p.Currency,
                    CurrencyCodesLines = _db.CurrencyCodes
                                    .Where(c => p.Currency == "##" || c.CurrCode == p.Currency)
                                    .Select(c => new CurrencyCodesEntity
                                    {
                                        CurrCode = c.CurrCode,
                                        CurrName = c.CurrName
                                    })
                                    .ToList(),

                    U_BPP_BPTD = p.U_BPP_BPTD,
                    U_BPP_BPTP = p.U_BPP_BPTP,
                    U_FIB_Divi = p.U_FIB_Divi,
                    U_FIB_Sector = p.U_FIB_Sector,

                    Phone1 = p.Phone1,
                    Phone2 = p.Phone2,
                    Cellular = p.Cellular,
                    Email = p.E_Mail,
                    ValidFor = p.ValidFor,
                    SlpCode = p.SlpCode,
                    SlpName = p.SalesPersons.SlpName,
                    Notes = p.Notes,

                    // Propiedad primaria de contacto
                    CntctCode = _db.ContactEmployees
                                .Where(c => c.CardCode == p.CardCode && c.Name == p.CntctPrsn)
                                .Select(c => (int?)c.CntctCode)
                                .FirstOrDefault() ?? 0,
                    CntctPrsn = p.CntctPrsn,
                    ContactEmployeesLines = _db.ContactEmployees
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
                                            Position = c.Position,
                                            Default = c.Name == p.CntctPrsn ? "X" : ""
                                        })
                                        .ToList(),

                    // Direcciones
                    BillToDef = p.BillToDef,
                    PayAddressLines = _db.Addresses
                                        .Where(a => a.CardCode == p.CardCode && a.AdresType == "B")
                                        .OrderBy(a => a.LineNum)
                                        .Select(a => new AddressesQueryEntity
                                        {
                                            LineNum = a.LineNum,
                                            CardCode = a.CardCode,
                                            AdresType = a.AdresType,
                                            Address = a.Address,
                                            Street = a.Street,
                                            Country  = a.Country,
                                            GlblLocNum = a.GlblLocNum,
                                            City = a.City,
                                            County = a.County,
                                            State = a.State,
                                            TaxCode = a.TaxCode,
                                            Default = a.Address == p.BillToDef ? "X" : ""
                                        })
                                        .ToList(),
                    ShipToDef = p.ShipToDef,
                    ShipAddressLines = _db.Addresses
                                        .Where(a => a.CardCode == p.CardCode && a.AdresType == "S")
                                        .OrderBy(a => a.LineNum)
                                        .Select(a => new AddressesQueryEntity
                                        {
                                            LineNum = a.LineNum,
                                            CardCode = a.CardCode,
                                            AdresType = a.AdresType,
                                            Address = a.Address,
                                            Street = a.Street,
                                            Country = a.Country,
                                            GlblLocNum = a.GlblLocNum,
                                            City = a.City,
                                            County = a.County,
                                            State = a.State,
                                            TaxCode = a.TaxCode,
                                            Default = a.Address == p.ShipToDef ? "X" : ""
                                        })
                                        .ToList(),

                    
                    // Condiciones de pago
                    GroupNum = p.GroupNum,
                    GroupName = p.BusinessPartnerGroups.GroupName,
                    ListNum = p.ListNum,
                    CreditLine = p.CreditLine ?? 0,
                    DebtLine = p.DebtLine ?? 0,                    

                    // Otros
                    U_BPP_BPAT = p.U_BPP_BPAT,
                    U_FIB_EMAIL2 = p.U_FIB_EMAIL2,
                    U_FIB_EMAIL3 = p.U_FIB_EMAIL3,
                    U_BPP_BPNO = p.U_BPP_BPNO,
                    U_BPP_BPAP = p.U_BPP_BPAP,
                    U_BPP_BPAM = p.U_BPP_BPAM
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.Data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetVehicleByCode(string cardCode)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersQueryEntity>
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
                    VehiclesLines = p.LinesVehicles.Select(s => new VehiclesQueryEntity
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
                resultTransaccion.Data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetDriverByCode(string cardCode)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersQueryEntity>
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
                    DriversLines = p.LinesDrivers.Select(s => new DriversQueryEntity
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
                resultTransaccion.Data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<BusinessPartnersViewEntity>> GetListClienteBySectorStatus(BusinessPartnersSectorStatusFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersViewEntity>
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
                resultTransaccion.DataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<BusinessPartnersViewEntity>> GetLitClienteContactoBySectorStatus(BusinessPartnersSectorStatusFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersViewEntity>
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
                resultTransaccion.DataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetClienteBySectorStatusExcel(BusinessPartnersSectorStatusFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            var ms = new MemoryStream();

            try
            {
                var objectGetCliente = await GetListClienteBySectorStatus(value);
                var objectGetClienteContacto = await GetLitClienteContactoBySectorStatus(value);
                ms = GetArchivoClienteBySectorStatusExcel(objectGetCliente.DataList.ToList(), objectGetClienteContacto.DataList.ToList());

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                resultTransaccion.Data = ms;
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
        public async Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> GetByRUC(string ruc)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersQueryEntity>
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
                    resultTransaccion.Data = data;
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

        #endregion


        #region <<< OPERACIONES >>>

        public async Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> SetCreate(BusinessPartnersCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            SAPbobsCOM.BusinessPartners businessPartners = null;

            return await Task.Run(() =>
            {
                try
                {
                    var company = _companyProviderSap.GetCompany();

                    businessPartners = company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                    businessPartners.CardCode = value.CardCode;
                    businessPartners.CardName = value.CardName;
                    if (!string.IsNullOrEmpty(value.CardType))
                    {
                        businessPartners.CardType = value.CardType == "S" ? BoCardTypes.cSupplier : value.CardType == "L" ? BoCardTypes.cLid : BoCardTypes.cCustomer;
                    }
                    businessPartners.GroupCode = value.GroupCode;
                    businessPartners.FederalTaxID = value.LicTradNum;
                    businessPartners.Currency = value.Currency;

                    businessPartners.UserFields.Fields.Item("U_BPP_BPTP").Value = value.U_BPP_BPTP;
                    businessPartners.UserFields.Fields.Item("U_BPP_BPTD").Value = value.U_BPP_BPTD;
                    businessPartners.UserFields.Fields.Item("U_FIB_Divi").Value = value.U_FIB_Divi;
                    businessPartners.UserFields.Fields.Item("U_FIB_Sector").Value = value.U_FIB_Sector;

                    businessPartners.Phone1 = value.Phone1;
                    businessPartners.Phone2 = value.Phone2;
                    businessPartners.Cellular = value.Cellular;
                    businessPartners.EmailAddress = value.Email;
                    businessPartners.Valid = value.ValidFor == "Y" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                    businessPartners.SalesPersonCode = value.SlpCode;
                    businessPartners.Notes = value.Notes;


                    businessPartners.ContactPerson = value.CntctPrsn;

                    businessPartners.BilltoDefault = value.BillToDef;
                    businessPartners.Address = value.Address;
                    businessPartners.ShipToDefault = value.ShipToDef;
                    businessPartners.MailAddress = value.MailAddres;

                    businessPartners.PayTermsGrpCode = value.GroupNum;
                    businessPartners.CreditLimit = (double)value.CreditLine;
                    businessPartners.MaxCommitment = (double)value.DebitLine;
                    businessPartners.PriceListNum = value.ListNum;

                    businessPartners.UserFields.Fields.Item("U_BPP_BPAT").Value = value.U_BPP_BPAT;
                    businessPartners.UserFields.Fields.Item("U_FIB_EMAIL2").Value = value.U_FIB_EMAIL2;
                    businessPartners.UserFields.Fields.Item("U_FIB_EMAIL3").Value = value.U_FIB_EMAIL3;
                    businessPartners.UserFields.Fields.Item("U_BPP_BPNO").Value = value.U_BPP_BPNO;
                    businessPartners.UserFields.Fields.Item("U_BPP_BPAP").Value = value.U_BPP_BPAP;
                    businessPartners.UserFields.Fields.Item("U_BPP_BPAM").Value = value.U_BPP_BPAM;



                    #region <<< DIRECCIONES >>>

                    for (int i = 0; i < value.AddressesLines.Count; i++)
                    {
                        var addr = value.AddressesLines[i];

                        businessPartners.Addresses.AddressName = addr.Address;
                        businessPartners.Addresses.AddressType = addr.AdresType == "S"
                            ? BoAddressType.bo_ShipTo
                            : BoAddressType.bo_BillTo;

                        businessPartners.Addresses.Street = addr.Street;
                        businessPartners.Addresses.City = addr.City;
                        businessPartners.Addresses.County = addr.County;
                        businessPartners.Addresses.State = addr.State;
                        businessPartners.Addresses.Country = addr.Country;
                        businessPartners.Addresses.GlobalLocationNumber = addr.GlblLocNum;
                        businessPartners.Addresses.TaxCode = addr.TaxCode;

                        businessPartners.Addresses.Add();
                    }

                    #endregion



                    #region <<< CONTACTOS >>>

                    for (int i = 0; i < value.ContactEmployeesLines.Count; i++)
                    {
                        var contact = value.ContactEmployeesLines[i];

                        businessPartners.ContactEmployees.Name = contact.Name;
                        businessPartners.ContactEmployees.FirstName = contact.FirstName;
                        businessPartners.ContactEmployees.MiddleName = contact.MiddleName;
                        businessPartners.ContactEmployees.LastName = contact.LastName;
                        businessPartners.ContactEmployees.Title = contact.Title;
                        businessPartners.ContactEmployees.Position = contact.Position;
                        businessPartners.ContactEmployees.Address = contact.Address;
                        businessPartners.ContactEmployees.Phone1 = contact.Phone1;
                        businessPartners.ContactEmployees.Phone2 = contact.Phone2;
                        businessPartners.ContactEmployees.MobilePhone = contact.MobilePhone;
                        businessPartners.ContactEmployees.E_Mail = contact.E_MailL;

                        if (i < value.ContactEmployeesLines.Count - 1)
                            businessPartners.ContactEmployees.Add();
                    }

                    #endregion



                    if (businessPartners.Add() != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "El socio de negocio ha sido registrado con éxito.";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message;
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(businessPartners);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> SetUpdate(BusinessPartnersUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            SAPbobsCOM.BusinessPartners businessPartners = null;

            return await Task.Run(() =>
            {
                try
                {
                    var company = _companyProviderSap.GetCompany();

                    businessPartners = company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                    if (!businessPartners.GetByKey(value.CardCode))
                    {
                        throw new Exception("No existe el socio de negocio.");
                    }

                    businessPartners.CardName = value.CardName;
                    if (!string.IsNullOrEmpty(value.CardType))
                    {
                        businessPartners.CardType = value.CardType == "S" ? BoCardTypes.cSupplier : value.CardType == "L" ? BoCardTypes.cLid : BoCardTypes.cCustomer;
                    }
                    businessPartners.GroupCode = value.GroupCode;
                    businessPartners.FederalTaxID = value.LicTradNum;
                    businessPartners.Currency = value.Currency;

                    businessPartners.UserFields.Fields.Item("U_BPP_BPTP").Value = value.U_BPP_BPTP;
                    businessPartners.UserFields.Fields.Item("U_BPP_BPTD").Value = value.U_BPP_BPTD;
                    businessPartners.UserFields.Fields.Item("U_FIB_Divi").Value = value.U_FIB_Divi;
                    businessPartners.UserFields.Fields.Item("U_FIB_Sector").Value = value.U_FIB_Sector;

                    businessPartners.Phone1 = value.Phone1;
                    businessPartners.Phone2 = value.Phone2;
                    businessPartners.Cellular = value.Cellular;
                    businessPartners.EmailAddress = value.Email;
                    businessPartners.Valid = value.ValidFor == "Y" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                    businessPartners.SalesPersonCode = value.SlpCode;
                    businessPartners.Notes = value.Notes;

                    businessPartners.ContactPerson = value.CntctPrsn;

                    businessPartners.BilltoDefault = value.BillToDef;
                    businessPartners.Address = value.Address;
                    businessPartners.ShipToDefault = value.ShipToDef;
                    businessPartners.MailAddress = value.MailAddres;

                    businessPartners.PayTermsGrpCode = value.GroupNum;
                    businessPartners.CreditLimit = (double)value.CreditLine;
                    businessPartners.MaxCommitment = (double)value.DebitLine;
                    businessPartners.PriceListNum = value.ListNum;

                    businessPartners.UserFields.Fields.Item("U_BPP_BPAT").Value = value.U_BPP_BPAT;
                    businessPartners.UserFields.Fields.Item("U_FIB_EMAIL2").Value = value.U_FIB_EMAIL2;
                    businessPartners.UserFields.Fields.Item("U_FIB_EMAIL3").Value = value.U_FIB_EMAIL3;
                    businessPartners.UserFields.Fields.Item("U_BPP_BPNO").Value = value.U_BPP_BPNO;
                    businessPartners.UserFields.Fields.Item("U_BPP_BPAP").Value = value.U_BPP_BPAP;
                    businessPartners.UserFields.Fields.Item("U_BPP_BPAM").Value = value.U_BPP_BPAM;



                    #region <<< DIRECCIONES >>>

                    var newAddresses = value.AddressesLines.Where(d => d.Record == 1).ToList();

                    // NUEVO: SE AGREGA NUEVO ITEM
                    for (int i = 0; i < newAddresses.Count; i++)
                    {
                        var line = newAddresses[i];

                        businessPartners.Addresses.AddressName = line.Address;
                        businessPartners.Addresses.AddressType = line.AdresType == "S"
                            ? BoAddressType.bo_ShipTo
                            : BoAddressType.bo_BillTo;

                        businessPartners.Addresses.Street = line.Street;
                        businessPartners.Addresses.City = line.City;
                        businessPartners.Addresses.County = line.County;
                        businessPartners.Addresses.State = line.State;
                        businessPartners.Addresses.Country = line.Country;
                        businessPartners.Addresses.GlobalLocationNumber = line.GlblLocNum;
                        businessPartners.Addresses.TaxCode = line.TaxCode;

                        businessPartners.Addresses.Add();
                    }

                    // EXISTE: SE MODIFICA EL ITEM
                    foreach (var line in value.AddressesLines.Where(d => d.Record == 2))
                    {
                        if (line.LineNum < 0 || line.LineNum >= businessPartners.Addresses.Count)
                            continue;

                        businessPartners.Addresses.SetCurrentLine(line.LineNum);

                        businessPartners.Addresses.AddressName = line.Address;
                        businessPartners.Addresses.AddressType = line.AdresType == "S"
                            ? BoAddressType.bo_ShipTo
                            : BoAddressType.bo_BillTo;

                        businessPartners.Addresses.Street = line.Street;
                        businessPartners.Addresses.City = line.City;
                        businessPartners.Addresses.County = line.County;
                        businessPartners.Addresses.State = line.State;
                        businessPartners.Addresses.Country = line.Country;
                        businessPartners.Addresses.GlobalLocationNumber = line.GlblLocNum;
                        businessPartners.Addresses.TaxCode = line.TaxCode;
                    }

                    // EXISTE: SE ELIMINA EL ITEM
                    foreach (var line in value.AddressesLines.Where(x => x.Record == 3).OrderByDescending(x => x.LineNum))
                    {
                        if (line.LineNum < 0 || line.LineNum >= businessPartners.Addresses.Count)
                            continue;

                        businessPartners.Addresses.SetCurrentLine(line.LineNum);
                        businessPartners.Addresses.Delete();
                    }

                    #endregion



                    #region <<< CONTACTOS >>>

                    // NUEVO: SE AGREGA NUEVO ITEM
                    var newContactsLines = value.ContactEmployeesLines.Where(c => c.Record == 1).ToList();

                    bool hasExistingContacts = businessPartners.ContactEmployees.Count > 0;

                    for (int i = 0; i < newContactsLines.Count; i++)
                    {
                        var line = newContactsLines[i];

                        if (hasExistingContacts || i > 0)
                        {
                            businessPartners.ContactEmployees.Add();
                        }

                        businessPartners.ContactEmployees.Name = line.Name;
                        businessPartners.ContactEmployees.FirstName = line.FirstName;
                        businessPartners.ContactEmployees.MiddleName = line.MiddleName;
                        businessPartners.ContactEmployees.LastName = line.LastName;
                        businessPartners.ContactEmployees.Title = line.Title;
                        businessPartners.ContactEmployees.Position = line.Position;
                        businessPartners.ContactEmployees.Address = line.Address;
                        businessPartners.ContactEmployees.Phone1 = line.Phone1;
                        businessPartners.ContactEmployees.Phone2 = line.Phone2;
                        businessPartners.ContactEmployees.MobilePhone = line.MobilePhone;
                        businessPartners.ContactEmployees.E_Mail = line.E_MailL;
                    }

                    // EXISTE: SE MODIFICA EL ITEM
                    foreach (var line in value.ContactEmployeesLines.Where(x => x.Record == 2))
                    {
                        for (int i = 0; i < businessPartners.ContactEmployees.Count; i++)
                        {
                            businessPartners.ContactEmployees.SetCurrentLine(i);

                            if (businessPartners.ContactEmployees.InternalCode != line.CntctCode)
                                continue;

                            businessPartners.ContactEmployees.Name = line.Name;
                            businessPartners.ContactEmployees.FirstName = line.FirstName;
                            businessPartners.ContactEmployees.MiddleName = line.MiddleName;
                            businessPartners.ContactEmployees.LastName = line.LastName;
                            businessPartners.ContactEmployees.Title = line.Title;
                            businessPartners.ContactEmployees.Position = line.Position;
                            businessPartners.ContactEmployees.Address = line.Address;
                            businessPartners.ContactEmployees.Phone1 = line.Phone1;
                            businessPartners.ContactEmployees.Phone2 = line.Phone2;
                            businessPartners.ContactEmployees.MobilePhone = line.MobilePhone;
                            businessPartners.ContactEmployees.E_Mail = line.E_MailL;

                            break;
                        }
                    }

                    // EXISTE: SE ELIMINA EL ITEM
                    foreach (var line in value.ContactEmployeesLines.Where(x => x.Record == 3))
                    {
                        for (int i = 0; i < businessPartners.ContactEmployees.Count; i++)
                        {
                            businessPartners.ContactEmployees.SetCurrentLine(i);

                            if (businessPartners.ContactEmployees.InternalCode != line.CntctCode)
                                continue;

                            businessPartners.ContactEmployees.Delete();
                            break;
                        }
                    }

                    #endregion



                    if (businessPartners.Update() != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "El socio de negocio ha sido actualizado con éxito.";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message;
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(businessPartners);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionResponse<BusinessPartnersQueryEntity>> SetDelete(string cardCode)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            SAPbobsCOM.BusinessPartners businessPartners = null;

            return await Task.Run(() =>
            {
                try
                {
                    var company = _companyProviderSap.GetCompany();
                    businessPartners = company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                    if (!businessPartners.GetByKey(cardCode))
                    {
                        throw new Exception("No existe el socio de negocio.");
                    }

                    int reg = businessPartners.Remove();

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
                    _companyProviderSap.LiberarObjetosCOM(businessPartners);
                }

                return resultTransaccion;
            });
        }

        #endregion
    }
}
