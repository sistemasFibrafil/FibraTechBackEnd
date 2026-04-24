using System;
using System.IO;
using SAPbobsCOM;
using AutoMapper;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Find;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Query;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Filter;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Create;
using Net.Business.Entities.SAPBusinessOne.Inventory.Picking.Entities;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Query;
namespace Net.Data.SAPBusinessOne
{
    public class PickingRepository : RepositoryBase<PickingEntity>, IPickingRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IMapper _mapper;
        private readonly DataContextProfil _dbProfil;
        private readonly DataContextSAPBusinessOne _dbSap;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        public PickingRepository(IConnectionSQL context, IConfiguration configuration, DataContextSAPBusinessOne dbSap, DataContextProfil dbProfil, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _dbSap = dbSap;
            _dbProfil = dbProfil;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }
        
        public async Task<ResultadoTransaccionResponse<PickingQueryEntity>> GetListByFilter(PickingFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PickingQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _dbSap.Picking
                .AsNoTracking()
                .Where(n => n.U_Status != "D" && n.U_BaseType == value.ObjType && n.U_DocDate >= value.StartDate && n.U_DocDate <= value.EndDate);

                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.U_BaseNum.ToString(), $"%{filter}%")
                    );
                }

                if (!string.IsNullOrWhiteSpace(value.Status))
                {
                    var status = value.Status.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    query = query.Where(n => status.Contains(n.U_Status));
                }

                var list = await query
                .GroupBy(n => new
                {
                    n.U_Status,
                    n.U_BaseEntry,
                    n.U_BaseNum,
                    n.U_BaseType,
                    n.U_DocDate,
                    n.U_DocDueDate,
                    n.U_BaseLine,
                    n.U_ItemCode,
                    n.U_Dscription,
                    n.U_FromWhsCod,
                    n.U_WhsCode,
                    n.U_UnitMsr
                })
                .Select(m => new PickingQueryEntity
                {
                    U_Status = m.Key.U_Status,
                    U_BaseEntry = m.Key.U_BaseEntry,
                    U_BaseNum = m.Key.U_BaseNum,
                    U_BaseType = m.Key.U_BaseType,
                    U_BaseLine = m.Key.U_BaseLine,
                    U_DocDate = m.Key.U_DocDate,
                    U_DocDueDate = m.Key.U_DocDueDate,
                    U_ItemCode = m.Key.U_ItemCode,
                    U_Dscription = m.Key.U_Dscription,
                    U_FromWhsCod = m.Key.U_FromWhsCod,
                    U_WhsCode = m.Key.U_WhsCode,
                    U_UnitMsr = m.Key.U_UnitMsr,
                    U_FIB_IsPkg = "Y",
                    U_Quantity = m.Sum(p => (string.IsNullOrEmpty(p.U_UnitMsr) ? "KG" : p.U_UnitMsr).ToUpper() == "KG" ? p.U_WeightKg ?? 0 : p.U_Quantity ?? 0),
                    U_WeightKg = m.Sum(p => p.U_WeightKg ?? 0),
                    U_NumBulk = m.Sum(p => p.U_NumBulk ?? 0)
                })
                .OrderByDescending(n => n.U_BaseEntry)
                .ThenBy(n => n.U_BaseLine)
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

        public async Task<ResultadoTransaccionResponse<PickingEntity>> GetListByBaseEntry(PickingFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PickingEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                value.U_CodeBar = value.U_CodeBar?.ToString().Trim() ?? string.Empty;

                var list = await _dbSap.Picking
                .Where(n => n.U_Status == value.U_Status && n.U_BaseEntry == value.U_BaseEntry && n.U_BaseType == value.U_BaseType && n.U_BaseLine == value.U_BaseLine && n.U_CodeBar.Contains(value.U_CodeBar))
                .Select(n => new PickingEntity
                {
                    DocEntry = n.DocEntry,
                    U_Status = n.U_Status,
                    U_ItemCode = n.U_ItemCode,
                    U_CodeBar = n.U_CodeBar,
                    U_IsReleased = n.U_IsReleased,
                    U_UnitMsr = n.U_UnitMsr,
                    U_WeightKg = n.U_WeightKg,
                    U_Quantity = (string.IsNullOrEmpty(n.U_UnitMsr) ? "KG" : n.U_UnitMsr).ToUpper() == "KG" ? n.U_WeightKg ?? 0 : n.U_Quantity ?? 0
                })
                .OrderBy(n => n.DocEntry)
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

        public async Task<ResultadoTransaccionResponse<PickingQueryEntity>> GetListByBaseEntryBaseType(PickingFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PickingQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = 
                from p in _dbSap.Picking
                where p.U_Status == "O" && p.U_BaseType == value.U_BaseType && p.U_BaseEntry == value.U_BaseEntry
                join s in _dbSap.InventoryTransferRequest1 on new { e = p.U_BaseEntry, l = p.U_BaseLine } equals new { e = (int?)s.DocEntry, l = (int?)s.LineNum } into ps
                from s in ps.Where(x => x.ObjType == value.U_BaseType.ToString()).DefaultIfEmpty()
                group new { p, s } by new
                {
                    p.U_Status,
                    p.U_BaseEntry,
                    p.U_BaseNum,
                    p.U_BaseType,
                    p.U_BaseLine,
                    p.U_ItemCode,
                    p.U_Dscription,
                    p.U_FromWhsCod,
                    p.U_WhsCode,
                    p.U_UnitMsr
                } into m
                select new PickingQueryEntity
                {
                    U_Status = m.Key.U_Status,
                    U_BaseEntry = m.Key.U_BaseEntry,
                    U_BaseNum = m.Key.U_BaseNum,
                    U_BaseType = m.Key.U_BaseType,
                    U_BaseLine = m.Key.U_BaseLine,
                    U_ItemCode = m.Key.U_ItemCode,
                    U_Dscription = m.Key.U_Dscription,
                    U_FromWhsCod = m.Key.U_FromWhsCod,
                    U_WhsCode = m.Key.U_WhsCode,
                    U_UnitMsr = m.Key.U_UnitMsr,
                    U_Quantity = m.Sum(p => p.p.U_Quantity ?? 0),
                    U_WeightKg = m.Sum(p => p.p.U_WeightKg ?? 0),
                    U_NumBulk = m.Sum(p => p.p.U_NumBulk ?? 0),
                    U_FIB_OpQtyPkg = m.Max(p => p.s != null ? p.s.U_FIB_OpQtyPkg ?? 0 : 0)
                };

                var list = await query.OrderBy(n => n.U_BaseLine).ToListAsync();

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

        public async Task<ResultadoTransaccionResponse<PickingEntity>> GetListByTarget(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PickingEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _dbSap.Picking
                .AsNoTracking()
                .Where(n => n.U_Status == "C" && n.U_TrgetEntry == value.U_TrgetEntry && n.U_TargetType == value.U_TargetType && n.U_TrgetLine == value.U_TrgetLine);

                if (!string.IsNullOrWhiteSpace(value.U_CodeBar))
                {
                    var filter = value.U_CodeBar.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.U_CodeBar, $"%{filter}%")
                    );
                }

                var list = await query
                .Select(n => new PickingEntity
                {
                    DocEntry = n.DocEntry,
                    U_ItemCode = n.U_ItemCode,
                    U_Dscription = n.U_Dscription,
                    U_CodeBar = n.U_CodeBar,
                    U_UnitMsr = n.U_UnitMsr,
                    U_WeightKg = n.U_WeightKg,
                    U_Quantity = (string.IsNullOrEmpty(n.U_UnitMsr) ? "KG" : n.U_UnitMsr).ToUpper() == "KG" ? n.U_WeightKg ?? 0 : n.U_Quantity ?? 0,
                })
                .OrderBy(x => x.DocEntry).ToListAsync();

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

        public async Task<ResultadoTransaccionResponse<InventoryTransferRequestQueryEntity>> GetToCopyTransferRequest(PickingCopyToFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<InventoryTransferRequestQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                // ==========================
                // 🔑 NORMALIZACIÓN (CLAVES ESCALARES)
                // ==========================

                // Líneas normales (sin paquete)
                var normalKeys = value.Lines
                .Where(v => v.U_FIB_IsPkg == "N")
                .Select(v => $"{v.U_BaseEntry}|{v.U_BaseLine}")
                .Distinct()
                .ToList();

                // Líneas con paquete (picking)
                var pkgKeys = value.Lines
                .Where(v => v.U_FIB_IsPkg == "Y")
                .Select(v => $"{v.U_BaseEntry}|{v.U_BaseType}|{v.U_BaseLine}")
                .Distinct()
                .ToList();

                // 🔑 PickingLines (MISMA ESTRATEGIA DE NORMALIZACIÓN)
                var pickingKeys = value.Lines
                    .Select(l => $"{l.U_BaseEntry}|{l.U_BaseType}|{l.U_BaseLine}")
                    .Distinct()
                    .ToList();

                // ==========================
                // 1️⃣ CABECERA
                // ==========================
                var data = await _dbSap.InventoryTransferRequest
                .AsNoTracking()
                .Where(x => x.DocStatus == "O" && x.DocEntry == value.U_BaseEntry)
                .Select(x => new InventoryTransferRequestQueryEntity
                {
                    CardCode = x.CardCode,
                    CardName = x.CardName,
                    CntctCode = x.CntctCode,
                    Address = x.Address,
                    Filler = x.Filler,
                    ToWhsCode = x.ToWhsCode,
                    U_FIB_TIP_TRAS = x.U_FIB_TIP_TRAS,
                    U_BPP_MDMT = x.U_BPP_MDMT,
                    U_BPP_MDTS = x.U_BPP_MDTS,
                    SlpCode = x.SlpCode,
                    U_FIB_NBULTOS = x.U_FIB_NBULTOS ?? 0,
                    U_FIB_KG = x.U_FIB_KG ?? 0
                })
                .FirstOrDefaultAsync()
                ?? throw new Exception("No se encontró la solicitud de traslado.");

                // ==========================
                // 2️⃣ LÍNEAS SIN PAQUETE
                // ==========================
                var normalLines = normalKeys.Any()
                ? await _dbSap.InventoryTransferRequest1
                .AsNoTracking()
                .Include(x => x.OperationType)
                .Where(x =>
                    x.LineStatus == "O" &&
                    x.OpenQty > 0 &&
                    normalKeys.Contains(
                        x.DocEntry.ToString() + "|" + x.LineNum.ToString()
                    )
                )
                .OrderBy(x => x.DocEntry)
                .ThenBy(x => x.LineNum)
                .Select(x => new InventoryTransferRequest1QueryEntity
                {
                    DocEntry = x.DocEntry,
                    LineNum = x.LineNum,
                    ObjType = x.ObjType,
                    BaseType = int.Parse(x.ObjType),
                    BaseEntry = x.DocEntry,
                    BaseLine = x.LineNum,
                    LineStatus = x.LineStatus,
                    ItemCode = x.ItemCode,
                    Dscription = x.Dscription,
                    FromWhsCod = x.FromWhsCod,
                    WhsCode = x.WhsCode,
                    U_tipoOpT12 = x.U_tipoOpT12 ?? "",
                    U_tipoOpT12Nam = x.OperationType.U_descrp ?? "",
                    UnitMsr = x.UnitMsr,
                    Quantity = x.OpenQty,
                    OpenQty = x.OpenQty,
                    U_FIB_FromPkg = "N",
                    U_FIB_OpQtyPkg = x.U_FIB_OpQtyPkg ?? 0,
                    U_FIB_NBulto = x.U_FIB_NBulto ?? 0,
                    U_FIB_PesoKg = x.U_FIB_PesoKg ?? 0
                })
                .ToListAsync()
                : new List<InventoryTransferRequest1QueryEntity>();

                // ==========================
                // 3️⃣ LÍNEAS CON PAQUETE (PICKING)
                // ==========================
                var pkgLines = pkgKeys.Any()
                ? await (
                    from p in _dbSap.Picking.AsNoTracking()
                    join s in _dbSap.InventoryTransferRequest1.AsNoTracking()
                        on new
                        {
                            Entry = p.U_BaseEntry.ToString(),
                            Type = p.U_BaseType.ToString(),
                            Line = p.U_BaseLine.ToString()
                        }
                        equals new
                        {
                            Entry = s.DocEntry.ToString(),
                            Type = s.ObjType,
                            Line = s.LineNum.ToString()
                        }
                    join t in _dbSap.OperationType.AsNoTracking()
                        on s.U_tipoOpT12 equals t.Code into tj
                    from t in tj.DefaultIfEmpty()
                    where p.U_Status == "O"
                        && pkgKeys.Contains(
                            p.U_BaseEntry.ToString() + "|" +
                            p.U_BaseType.ToString() + "|" +
                            p.U_BaseLine.ToString()
                        )
                    group new { p, s, t } by new
                    {
                        s.DocEntry,
                        s.LineNum,
                        p.U_BaseEntry,
                        p.U_BaseType,
                        p.U_BaseLine,
                        p.U_ItemCode,
                        p.U_Dscription,
                        p.U_FromWhsCod,
                        p.U_WhsCode,
                        p.U_UnitMsr,
                        s.U_tipoOpT12
                    }
                    into g
                    select new InventoryTransferRequest1QueryEntity
                    {
                        DocEntry = g.Key.DocEntry,
                        LineNum = g.Key.LineNum,
                        BaseEntry = g.Key.U_BaseEntry,
                        BaseType = g.Key.U_BaseType ?? 1250000001,
                        BaseLine = g.Key.U_BaseLine,
                        ItemCode = g.Key.U_ItemCode,
                        Dscription = g.Key.U_Dscription,
                        FromWhsCod = g.Key.U_FromWhsCod,
                        WhsCode = g.Key.U_WhsCode,
                        U_tipoOpT12 = g.Key.U_tipoOpT12 ?? "",
                        U_tipoOpT12Nam = g.Select(x => x.t.U_descrp).FirstOrDefault() ?? "",
                        UnitMsr = string.IsNullOrEmpty(g.Key.U_UnitMsr) ? "KG" : g.Key.U_UnitMsr,
                        U_FIB_FromPkg = "Y",
                        U_FIB_PesoKg = g.Sum(x => x.p.U_WeightKg ?? 0),
                        U_FIB_NBulto = g.Sum(x => x.p.U_NumBulk ?? 0),
                        Quantity = g.Sum(x => (string.IsNullOrEmpty(x.p.U_UnitMsr) ? "KG" : x.p.U_UnitMsr).ToUpper() == "KG" ? x.p.U_WeightKg ?? 0 : x.p.U_Quantity ?? 0)
                    }
                )
                .OrderBy(x => x.DocEntry)
                .ThenBy(x => x.LineNum)
                .ToListAsync()
                : new List<InventoryTransferRequest1QueryEntity>();

                // ==========================
                // 4️⃣ RESULTADO FINAL
                // ==========================
                data.Lines.AddRange(normalLines);
                data.Lines.AddRange(pkgLines);

                // 🔑 PickingLines usando normalización escalar (EF-friendly)
                if (pickingKeys.Any())
                {
                    data.PickingLines.AddRange(
                    await _dbSap.Picking
                    .AsNoTracking()
                    .Where(p =>
                        p.U_Status == "O" &&
                        pickingKeys.Contains(
                            p.U_BaseEntry.ToString() + "|" +
                            p.U_BaseType.ToString() + "|" +
                            p.U_BaseLine.ToString()
                        )
                    )
                    .OrderBy(p => p.U_BaseEntry)
                    .ThenBy(p => p.U_BaseLine)
                    .Select(p => new PickingEntity
                    {
                        DocEntry = p.DocEntry,
                        U_BaseEntry = p.U_BaseEntry,
                        U_BaseType = p.U_BaseType,
                        U_BaseLine = p.U_BaseLine,
                        U_ItemCode = p.U_ItemCode,
                        U_Dscription = p.U_Dscription,
                        U_CodeBar = p.U_CodeBar,
                        U_FromWhsCod = p.U_FromWhsCod,
                        U_WhsCode = p.U_WhsCode,
                        U_UnitMsr = p.U_UnitMsr,
                        U_NumBulk = p.U_NumBulk,
                        U_WeightKg = p.U_WeightKg,
                        U_Quantity = (string.IsNullOrEmpty(p.U_UnitMsr) ? "KG" : p.U_UnitMsr).ToUpper() == "KG" ? p.U_WeightKg ?? 0 : p.U_Quantity ?? 0,
                        U_Status = "C"
                    })
                    .ToListAsync());
                }

                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<OrdersQueryEntity>> GetToCopyOrder(PickingCopyToFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OrdersQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var adminInfo = await _dbSap.AdminInfo
                .Select(n => new
                {
                    MaMainCurncy = n.MainCurncy
                })
                .FirstOrDefaultAsync();

                // ==========================
                // 🔑 NORMALIZACIÓN (CLAVES ESCALARES)
                // ==========================

                // Líneas con paquete (picking)
                var pkgKeys = value.Lines
                .Where(v => v.U_FIB_IsPkg == "Y")
                .Select(v => $"{v.U_BaseEntry}|{v.U_BaseType}|{v.U_BaseLine}")
                .Distinct()
                .ToList();

                // 🔑 PickingLines (MISMA ESTRATEGIA DE NORMALIZACIÓN)
                var pickingKeys = value.Lines
                    .Select(l => $"{l.U_BaseEntry}|{l.U_BaseType}|{l.U_BaseLine}")
                    .Distinct()
                    .ToList();

                // ==========================
                // 1️⃣ CABECERA
                // ==========================
                var data = await _dbSap.Orders
                .AsNoTracking()
                .Where(n => n.DocStatus == "O" && n.DocEntry == value.U_BaseEntry)
                .Select(f => new OrdersQueryEntity
                {
                    DocType = f.DocType,
                    CardCode = f.CardCode,
                    CardName = f.CardName,
                    CntctCode = f.CntctCode,
                    NumAtCard = f.NumAtCard,
                    DocCur = f.DocCur,
                    CurrencyList = _dbSap.CurrencyCodes
                                   .Where(c => c.CurrCode == f.DocCur)
                                   .Select(c => new CurrencyCodesEntity
                                   {
                                       CurrCode = c.CurrCode,
                                       CurrName = c.CurrName
                                   })
                                   .ToList(),
                    DocRate = f.DocRate,

                    PayToCode = f.PayToCode,
                    // ✅ DIRECCIONES DE PAGO (CRD1 AdresType = 'B')
                    PayAddressList = _dbSap.Addresses
                                     .Where(a => a.CardCode == f.CardCode && a.AdresType == "B")
                                     .OrderBy(a => a.LineNum)
                                     .Select(a => new AddressesEntity
                                     {
                                         CardCode = a.CardCode,
                                         AdresType = a.AdresType,
                                         Address = a.Address,
                                         Street = a.Street,
                                         LineNum = a.LineNum
                                     })
                                     .ToList(),
                    Address = f.Address,
                    ShipToCode = f.ShipToCode,
                    // ✅ DIRECCIONES DE DESPACHO (CRD1 AdresType = 'S')
                    ShipAddressList = _dbSap.Addresses
                                      .Where(a => a.CardCode == f.CardCode && a.AdresType == "S")
                                      .OrderBy(a => a.LineNum)
                                      .Select(a => new AddressesEntity
                                      {
                                          CardCode = a.CardCode,
                                          AdresType = a.AdresType,
                                          Address = a.Address,
                                          Street = a.Street,
                                          LineNum = a.LineNum
                                      })
                                      .ToList(),
                    Address2 = f.Address2,

                    GroupNum = f.GroupNum,

                    U_BPP_MDCT = f.U_BPP_MDCT,
                    U_BPP_MDRT = f.U_BPP_MDRT,
                    U_BPP_MDNT = f.U_BPP_MDNT,
                    U_FIB_CODT = f.U_FIB_CODT,
                    // ✅ DIRECCIONES DE DESPACHO (CRD1 AdresType = 'S')
                    AgencyAddressList = _dbSap.Addresses
                                        .Where(a => a.CardCode == f.U_BPP_MDCT && a.AdresType == "S")
                                        .OrderBy(a => a.LineNum)
                                        .Select(a => new AddressesEntity
                                        {
                                            CardCode = a.CardCode,
                                            AdresType = a.AdresType,
                                            Address = a.Address,
                                            Street = a.Street,
                                            LineNum = a.LineNum
                                        })
                                        .ToList(),
                    U_BPP_MDDT = f.U_BPP_MDDT,

                    U_TipoFlete = f.U_TipoFlete,
                    U_ValorFlete = f.U_ValorFlete,
                    U_FIB_TFLETE = f.U_FIB_TFLETE,
                    U_FIB_IMPSEG = f.U_FIB_IMPSEG,
                    U_FIB_PUERTO = f.U_FIB_PUERTO,

                    U_STR_TVENTA = f.U_STR_TVENTA,

                    SlpCode = f.SlpCode,
                    U_FIB_NBULTOS = f.U_FIB_NBULTOS ?? 0,
                    U_FIB_KG = f.U_FIB_KG ?? 0,
                    U_NroOrden = f.U_NroOrden,
                    U_OrdenCompra = f.U_OrdenCompra,

                    DiscPrcnt = f.DiscPrcnt ?? 0,
                    DiscSum = adminInfo.MaMainCurncy == f.DocCur ? f.DiscSum : f.DiscSumSy
                })
                .FirstOrDefaultAsync()
                ?? throw new Exception("No se encontró la orden de venta.");


                // ==========================
                // 20 LÍNEAS CON PAQUETE (PICKING)
                // ==========================

                var pkgLines = pkgKeys.Any()
                ? await (
                    from p in _dbSap.Picking.AsNoTracking()

                    join s in _dbSap.Orders1.AsNoTracking()
                        on new
                        {
                            Entry = p.U_BaseEntry.ToString(),
                            Type = p.U_BaseType.ToString(),
                            Line = p.U_BaseLine.ToString()
                        }
                        equals new
                        {
                            Entry = s.DocEntry.ToString(),
                            Type = s.ObjType,
                            Line = s.LineNum.ToString()
                        }

                        // 🧾 LEFT JOIN ChartOfAccounts (OACT)
                    join c in _dbSap.ChartOfAccounts.AsNoTracking() on s.AcctCode equals c.AcctCode into cj
                    from c in cj.DefaultIfEmpty()

                        // 🏷️ LEFT JOIN TipoOperacion
                    join t in _dbSap.OperationType.AsNoTracking() on s.U_tipoOpT12 equals t.Code into tj
                    from t in tj.DefaultIfEmpty()

                        // 📦 LEFT JOIN Items (OITM)
                    join i in _dbSap.Items.AsNoTracking() on s.ItemCode equals i.ItemCode into ij
                    from i in ij.DefaultIfEmpty()

                    where p.U_Status == "O"
                       && pkgKeys.Contains(
                            p.U_BaseEntry.ToString() + "|" +
                            p.U_BaseType.ToString() + "|" +
                            p.U_BaseLine.ToString()
                       )

                    group new { p, s, c, t, i } by new
                    {
                        // 🔑 Claves base
                        s.DocEntry,
                        s.LineNum,
                        p.U_BaseEntry,
                        p.U_BaseType,
                        p.U_BaseLine,

                        p.U_ItemCode,
                        p.U_Dscription,
                        p.U_FromWhsCod,
                        p.U_UnitMsr,

                        // 🧾 Cuenta contable
                        s.AcctCode,
                        AcctName = c != null ? c.AcctName : null,
                        Segment0 = c != null ? c.Segment_0 : null,
                        Segment1 = c != null ? c.Segment_1 : null,
                        Segment2 = c != null ? c.Segment_2 : null,

                        // 🏷️ Tipo operación
                        s.U_tipoOpT12,
                        U_tipoOpT12Nam = t != null ? t.U_descrp : null,

                        // 📦 Item
                        OnHand = i != null ? i.OnHand : 0,

                        // 💰 Precios / impuestos
                        s.Currency,
                        s.PriceBefDi,
                        s.DiscPrcnt,
                        s.Price,
                        s.TaxCode,
                        s.VatPrcnt
                    }
                    into g

                    select new Orders1QueryEntity
                    {
                        DocEntry = g.Key.DocEntry,
                        LineNum = g.Key.LineNum,
                        BaseEntry = g.Key.U_BaseEntry,
                        BaseType = g.Key.U_BaseType ?? 0,
                        BaseLine = g.Key.U_BaseLine,

                        U_FIB_FromPkg = "Y",

                        ItemCode = g.Key.U_ItemCode,
                        Dscription = g.Key.U_Dscription,
                        // 🧾 Cuenta contable
                        AcctCode = g.Key.AcctCode,
                        AcctName = g.Key.AcctName,
                        FormatCode =
                            (g.Key.Segment0 ?? "") + "-" +
                            (g.Key.Segment1 ?? "") + "-" +
                            (g.Key.Segment2 ?? ""),
                        WhsCode = g.Key.U_FromWhsCod,

                        // 🏷️ Tipo operación
                        U_tipoOpT12 = g.Key.U_tipoOpT12 ?? "",
                        U_tipoOpT12Nam = g.Key.U_tipoOpT12Nam ?? "",

                        UnitMsr = string.IsNullOrEmpty(g.Key.U_UnitMsr) ? "KG" : g.Key.U_UnitMsr,
                        // 📦 Stock
                        OnHand = g.Key.OnHand,
                        // 📦 Cantidades
                        U_FIB_PesoKg = g.Sum(x => x.p.U_WeightKg ?? 0),
                        U_FIB_NBulto = g.Sum(x => x.p.U_NumBulk ?? 0),
                        Quantity = g.Sum(x => (string.IsNullOrEmpty(x.p.U_UnitMsr) ? "KG" : x.p.U_UnitMsr).ToUpper() == "KG" ? x.p.U_WeightKg ?? 0 : x.p.U_Quantity ?? 0),

                        // 💰 Precios / impuestos
                        Currency = g.Key.Currency,
                        PriceBefDi = g.Key.PriceBefDi,
                        DiscPrcnt = g.Key.DiscPrcnt ?? 0,
                        Price = g.Key.Price,
                        TaxCode = g.Key.TaxCode,
                        VatPrcnt = g.Key.VatPrcnt ?? 0
                    }
                )
                .OrderBy(x => x.DocEntry)
                .ThenBy(x => x.LineNum)
                .ToListAsync()
                : new List<Orders1QueryEntity>();


                // ==========================
                // 30 RESULTADO FINAL
                // ==========================
                data.Lines.AddRange(pkgLines);

                // 🔑 PickingLines usando normalización escalar (EF-friendly)
                if (pickingKeys.Any())
                {
                    data.PickingLines.AddRange(
                    await _dbSap.Picking
                    .AsNoTracking()
                    .Where(p =>
                        p.U_Status == "O" &&
                        pickingKeys.Contains(
                            p.U_BaseEntry.ToString() + "|" +
                            p.U_BaseType.ToString() + "|" +
                            p.U_BaseLine.ToString()
                        )
                    )
                    .OrderBy(p => p.U_BaseEntry)
                    .ThenBy(p => p.U_BaseLine)
                    .Select(p => new PickingEntity
                    {
                        DocEntry = p.DocEntry,
                        U_BaseEntry = p.U_BaseEntry,
                        U_BaseType = p.U_BaseType,
                        U_BaseLine = p.U_BaseLine,
                        U_ItemCode = p.U_ItemCode,
                        U_Dscription = p.U_Dscription,
                        U_CodeBar = p.U_CodeBar,
                        U_FromWhsCod = p.U_FromWhsCod,
                        U_WhsCode = p.U_WhsCode,
                        U_UnitMsr = p.U_UnitMsr,
                        U_NumBulk = p.U_NumBulk,
                        U_WeightKg = p.U_WeightKg,
                        U_Quantity = (string.IsNullOrEmpty(p.U_UnitMsr) ? "KG" : p.U_UnitMsr).ToUpper() == "KG" ? p.U_WeightKg ?? 0 : p.U_Quantity ?? 0,
                        U_Status = "C"
                    })
                    .ToListAsync());
                }

                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<InvoicesQueryEntity>> GetToCopyInvoice(PickingCopyToFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<InvoicesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var adminInfo = await _dbSap.AdminInfo
                .Select(n => new
                {
                    MaMainCurncy = n.MainCurncy
                })
                .FirstOrDefaultAsync();

                // ==========================
                // 🔑 NORMALIZACIÓN (CLAVES ESCALARES)
                // ==========================

                // Líneas con paquete (picking)
                var pkgKeys = value.Lines
                .Where(v => v.U_FIB_IsPkg == "Y")
                .Select(v => $"{v.U_BaseEntry}|{v.U_BaseType}|{v.U_BaseLine}")
                .Distinct()
                .ToList();

                // 🔑 PickingLines (MISMA ESTRATEGIA DE NORMALIZACIÓN)
                var pickingKeys = value.Lines
                    .Select(l => $"{l.U_BaseEntry}|{l.U_BaseType}|{l.U_BaseLine}")
                    .Distinct()
                    .ToList();

                // ==========================
                // 1️⃣ CABECERA
                // ==========================
                var data = await _dbSap.Invoices
                .AsNoTracking()
                .Where(n => n.DocStatus == "O" && n.DocEntry == value.U_BaseEntry)
                .Select(f => new InvoicesQueryEntity
                {
                    DocType = f.DocType,
                    CardCode = f.CardCode,
                    CardName = f.CardName,
                    CntctCode = f.CntctCode,
                    NumAtCard = f.NumAtCard,
                    DocCur = f.DocCur,
                    CurrencyList = _dbSap.CurrencyCodes
                                   .Where(c => c.CurrCode == f.DocCur)
                                   .Select(c => new CurrencyCodesEntity
                                   {
                                       CurrCode = c.CurrCode,
                                       CurrName = c.CurrName
                                   })
                                   .ToList(),
                    DocRate = f.DocRate,

                    PayToCode = f.PayToCode,
                    // ✅ DIRECCIONES DE PAGO (CRD1 AdresType = 'B')
                    PayAddressList = _dbSap.Addresses
                                     .Where(a => a.CardCode == f.CardCode && a.AdresType == "B")
                                     .OrderBy(a => a.LineNum)
                                     .Select(a => new AddressesEntity
                                     {
                                         CardCode = a.CardCode,
                                         AdresType = a.AdresType,
                                         Address = a.Address,
                                         Street = a.Street,
                                         LineNum = a.LineNum
                                     })
                                     .ToList(),
                    Address = f.Address,
                    ShipToCode = f.ShipToCode,
                    // ✅ DIRECCIONES DE DESPACHO (CRD1 AdresType = 'S')
                    ShipAddressList = _dbSap.Addresses
                                      .Where(a => a.CardCode == f.CardCode && a.AdresType == "S")
                                      .OrderBy(a => a.LineNum)
                                      .Select(a => new AddressesEntity
                                      {
                                          CardCode = a.CardCode,
                                          AdresType = a.AdresType,
                                          Address = a.Address,
                                          Street = a.Street,
                                          LineNum = a.LineNum
                                      })
                                      .ToList(),
                    Address2 = f.Address2,

                    GroupNum = f.GroupNum,

                    U_BPP_MDCT = f.U_BPP_MDCT,
                    U_BPP_MDRT = f.U_BPP_MDRT,
                    U_BPP_MDNT = f.U_BPP_MDNT,
                    U_FIB_CODT = f.U_FIB_CODT,
                    // ✅ DIRECCIONES DE DESPACHO (CRD1 AdresType = 'S')
                    AgencyAddressList = _dbSap.Addresses
                                        .Where(a => a.CardCode == f.U_BPP_MDCT && a.AdresType == "S")
                                        .OrderBy(a => a.LineNum)
                                        .Select(a => new AddressesEntity
                                        {
                                            CardCode = a.CardCode,
                                            AdresType = a.AdresType,
                                            Address = a.Address,
                                            Street = a.Street,
                                            LineNum = a.LineNum
                                        })
                                        .ToList(),
                    U_BPP_MDDT = f.U_BPP_MDDT,

                    U_TipoFlete = f.U_TipoFlete,
                    U_ValorFlete = f.U_ValorFlete,
                    U_FIB_TFLETE = f.U_FIB_TFLETE,
                    U_FIB_IMPSEG = f.U_FIB_IMPSEG,
                    U_FIB_PUERTO = f.U_FIB_PUERTO,

                    U_STR_TVENTA = f.U_STR_TVENTA,

                    SlpCode = f.SlpCode,
                    U_FIB_NBULTOS = f.U_FIB_NBULTOS ?? 0,
                    U_FIB_KG = f.U_FIB_KG ?? 0,
                    U_NroOrden = f.U_NroOrden,
                    U_OrdenCompra = f.U_OrdenCompra,

                    DiscPrcnt = f.DiscPrcnt ?? 0,
                    DiscSum = adminInfo.MaMainCurncy == f.DocCur ? f.DiscSum : f.DiscSumSy
                })
                .FirstOrDefaultAsync()
                ?? throw new Exception("No se encontró la orden de venta.");


                // ==========================
                // 20 LÍNEAS CON PAQUETE (PICKING)
                // ==========================

                var pkgLines = pkgKeys.Any()
                ? await (
                    from p in _dbSap.Picking.AsNoTracking()

                    join s in _dbSap.Invoices1.AsNoTracking()
                        on new
                        {
                            Entry = p.U_BaseEntry.ToString(),
                            Type = p.U_BaseType.ToString(),
                            Line = p.U_BaseLine.ToString()
                        }
                        equals new
                        {
                            Entry = s.DocEntry.ToString(),
                            Type = s.ObjType,
                            Line = s.LineNum.ToString()
                        }

                        // 🧾 LEFT JOIN ChartOfAccounts (OACT)
                    join c in _dbSap.ChartOfAccounts.AsNoTracking() on s.AcctCode equals c.AcctCode into cj
                    from c in cj.DefaultIfEmpty()

                        // 🏷️ LEFT JOIN TipoOperacion
                    join t in _dbSap.OperationType.AsNoTracking() on s.U_tipoOpT12 equals t.Code into tj
                    from t in tj.DefaultIfEmpty()

                        // 📦 LEFT JOIN Items (OITM)
                    join i in _dbSap.Items.AsNoTracking() on s.ItemCode equals i.ItemCode into ij
                    from i in ij.DefaultIfEmpty()

                    where p.U_Status == "O"
                       && pkgKeys.Contains(
                            p.U_BaseEntry.ToString() + "|" +
                            p.U_BaseType.ToString() + "|" +
                            p.U_BaseLine.ToString()
                       )

                    group new { p, s, c, t, i } by new
                    {
                        // 🔑 Claves base
                        s.DocEntry,
                        s.LineNum,
                        p.U_BaseEntry,
                        p.U_BaseType,
                        p.U_BaseLine,

                        p.U_ItemCode,
                        p.U_Dscription,
                        p.U_FromWhsCod,
                        p.U_UnitMsr,

                        // 🧾 Cuenta contable
                        s.AcctCode,
                        AcctName = c != null ? c.AcctName : null,
                        Segment0 = c != null ? c.Segment_0 : null,
                        Segment1 = c != null ? c.Segment_1 : null,
                        Segment2 = c != null ? c.Segment_2 : null,

                        // 🏷️ Tipo operación
                        s.U_tipoOpT12,
                        U_tipoOpT12Nam = t != null ? t.U_descrp : null,

                        // 📦 Item
                        OnHand = i != null ? i.OnHand : 0,

                        // 💰 Precios / impuestos
                        s.Currency,
                        s.PriceBefDi,
                        s.DiscPrcnt,
                        s.Price,
                        s.TaxCode,
                        s.VatPrcnt
                    }
                    into g

                    select new Invoices1QueryEntity
                    {
                        DocEntry = g.Key.DocEntry,
                        LineNum = g.Key.LineNum,
                        BaseEntry = g.Key.U_BaseEntry,
                        BaseType = g.Key.U_BaseType ?? 0,
                        BaseLine = g.Key.U_BaseLine,

                        U_FIB_FromPkg = "Y",

                        ItemCode = g.Key.U_ItemCode,
                        Dscription = g.Key.U_Dscription,
                        // 🧾 Cuenta contable
                        AcctCode = g.Key.AcctCode,
                        AcctName = g.Key.AcctName,
                        FormatCode =
                            (g.Key.Segment0 ?? "") + "-" +
                            (g.Key.Segment1 ?? "") + "-" +
                            (g.Key.Segment2 ?? ""),
                        WhsCode = g.Key.U_FromWhsCod,

                        // 🏷️ Tipo operación
                        U_tipoOpT12 = g.Key.U_tipoOpT12 ?? "",
                        U_tipoOpT12Nam = g.Key.U_tipoOpT12Nam ?? "",

                        UnitMsr = string.IsNullOrEmpty(g.Key.U_UnitMsr) ? "KG" : g.Key.U_UnitMsr,
                        // 📦 Stock
                        OnHand = g.Key.OnHand,
                        // 📦 Cantidades
                        U_FIB_PesoKg = g.Sum(x => x.p.U_WeightKg ?? 0),
                        U_FIB_NBulto = g.Sum(x => x.p.U_NumBulk ?? 0),
                        Quantity = g.Sum(x => (string.IsNullOrEmpty(x.p.U_UnitMsr) ? "KG" : x.p.U_UnitMsr).ToUpper() == "KG" ? x.p.U_WeightKg ?? 0 : x.p.U_Quantity ?? 0),

                        // 💰 Precios / impuestos
                        Currency = g.Key.Currency,
                        PriceBefDi = g.Key.PriceBefDi,
                        DiscPrcnt = g.Key.DiscPrcnt ?? 0,
                        Price = g.Key.Price,
                        TaxCode = g.Key.TaxCode,
                        VatPrcnt = g.Key.VatPrcnt ?? 0
                    }
                )
                .OrderBy(x => x.DocEntry)
                .ThenBy(x => x.LineNum)
                .ToListAsync()
                : new List<Invoices1QueryEntity>();


                // ==========================
                // 30 RESULTADO FINAL
                // ==========================
                data.Lines.AddRange(pkgLines);

                // 🔑 PickingLines usando normalización escalar (EF-friendly)
                if (pickingKeys.Any())
                {
                    data.PickingLines.AddRange(
                    await _dbSap.Picking
                    .AsNoTracking()
                    .Where(p =>
                        p.U_Status == "O" &&
                        pickingKeys.Contains(
                            p.U_BaseEntry.ToString() + "|" +
                            p.U_BaseType.ToString() + "|" +
                            p.U_BaseLine.ToString()
                        )
                    )
                    .OrderBy(p => p.U_BaseEntry)
                    .ThenBy(p => p.U_BaseLine)
                    .Select(p => new PickingEntity
                    {
                        DocEntry = p.DocEntry,
                        U_BaseEntry = p.U_BaseEntry,
                        U_BaseType = p.U_BaseType,
                        U_BaseLine = p.U_BaseLine,
                        U_ItemCode = p.U_ItemCode,
                        U_Dscription = p.U_Dscription,
                        U_CodeBar = p.U_CodeBar,
                        U_FromWhsCod = p.U_FromWhsCod,
                        U_WhsCode = p.U_WhsCode,
                        U_UnitMsr = p.U_UnitMsr,
                        U_NumBulk = p.U_NumBulk,
                        U_WeightKg = p.U_WeightKg,
                        U_Quantity = (string.IsNullOrEmpty(p.U_UnitMsr) ? "KG" : p.U_UnitMsr).ToUpper() == "KG" ? p.U_WeightKg ?? 0 : p.U_Quantity ?? 0,
                        U_Status = "C"
                    })
                    .ToListAsync());
                }

                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<PickingQueryEntity>> SetCreate(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PickingQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Recordset rs = null;
            Company company = null;
            Documents order = null;
            Documents invoices = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            StockTransfer stockTransfer = null;
            GeneralService oGeneralService = null;

            try
            {
                // =========================
                // Validaciones básicas
                // =========================
                if (value == null) throw new Exception("Solicitud inválida.");
                if (string.IsNullOrWhiteSpace(value.U_CodeBar)) throw new Exception("El código de barras es requerido.");
                if (value.U_BaseEntry == null) throw new Exception("BaseEntry es requerido.");
                if (value.U_BaseType == null) throw new Exception("BaseType es requerido.");
                if (string.IsNullOrWhiteSpace(value.U_FromWhsCod)) throw new Exception("Almacén origen es requerido.");

                var baseEntry = value.U_BaseEntry.Value;
                var baseType = value.U_BaseType.Value;
                var codeBar = value.U_CodeBar.Trim();

                // =========================
                // Validaciones de duplicidad
                // =========================
                var existsInOpenPicking = await _dbSap.Picking
                    .AsNoTracking()
                    .AnyAsync(n => n.U_Status != "D"
                                && n.U_IsReleased != "Y"
                                && n.U_CodeBar == codeBar);

                if (existsInOpenPicking)
                    throw new Exception("Este código de barras ya se encuentra registrado.");

                var existsReleasedSameDoc = await _dbSap.Picking
                    .AsNoTracking()
                    .AnyAsync(n => n.U_Status != "D"
                                && n.U_IsReleased == "Y"
                                && n.U_CodeBar == codeBar
                                && n.U_BaseEntry == baseEntry
                                && n.U_BaseType == baseType);

                if (existsReleasedSameDoc)
                    throw new Exception("El código ya fue liberado en este documento y no puede volver a ser leído.");

                // =========================
                // Pesaje (Item + Peso) por CodeBar
                // =========================
                var pesaje = await _dbProfil.Pesaje
                    .AsNoTracking()
                    .Select(n => new
                    {
                        n.ItemNo,
                        line = n.Pesaje1
                            .Where(m => m.CODEBAR == codeBar)
                            .Select(m => new { m.CODEBAR, m.PesoBob })
                            .FirstOrDefault()
                    })
                    .Where(x => x.line != null)
                    .Select(x => new
                    {
                        ItemCode = x.ItemNo,
                        CodeBar = x.line.CODEBAR,
                        WeightKg = x.line.PesoBob
                    })
                    .FirstOrDefaultAsync();

                if (pesaje == null) throw new Exception("El código de barras ingresado no existe.");
                if (pesaje.WeightKg == null || pesaje.WeightKg == 0) throw new Exception("El peso del código de barras es cero.");

                // =========================
                // Cabecera + Línea pendiente según BaseType
                // 67 => Solicitud de Traslado (OWTQ/WTQ1)
                // 17 => Orden de Venta (ORDR/RDR1)
                // =========================
                var (header, line) = await GetHeaderAndPendingLineByBaseTypeAsync(
                    baseType: baseType,
                    baseEntry: baseEntry,
                    itemCode: pesaje.ItemCode,
                    fromWhsCod: value.U_FromWhsCod
                );

                // =========================
                // Stock disponible (OnHand - picking ya leído)
                // =========================
                var sumStock = await _dbSap.ItemWarehouseInfo
                    .AsNoTracking()
                    .Where(n => n.ItemCode == line.ItemCode && n.WhsCode == line.FromWhsCod)
                    .SumAsync(n => (decimal?)n.OnHand) ?? 0m;

                var sumWeight = await _dbSap.Picking
                    .AsNoTracking()
                    .Where(n => n.U_Status == "O" && n.U_ItemCode == line.ItemCode && n.U_FromWhsCod == line.FromWhsCod)
                    .SumAsync(n => (decimal?)(n.U_WeightKg ?? 0)) ?? 0m;

                var sumQuantity = await _dbSap.Picking
                    .AsNoTracking()
                    .Where(n => n.U_Status == "O" && n.U_ItemCode == line.ItemCode && n.U_FromWhsCod == line.FromWhsCod)
                    .SumAsync(n => (decimal?)(n.U_Quantity ?? 0)) ?? 0m;

                var stock = line.UnitMsr == "KG" ? sumStock - sumWeight : sumStock - sumQuantity;

                if (stock <= 0)
                    throw new Exception($"No hay stock disponible para el artículo en el almacén de origen {line.FromWhsCod}.");

                // =========================
                // Cantidad por paquete (SalPackUn)
                // =========================
                var quantity = await _dbSap.Items
                    .AsNoTracking()
                    .Where(n => n.ItemCode == line.ItemCode)
                    .Select(n => n.SalPackUn)
                    .FirstOrDefaultAsync()
                    ?? throw new Exception("Define la cantidad por paquete en SAP Business One, en los Datos maestros de artículo, pestaña Datos de ventas.");

                // =========================
                // Validaciones con lectura actual
                // =========================
                if (line.UnitMsr == "KG")
                {
                    var lectura = (pesaje.WeightKg ?? 0m);
                    if (stock - lectura < 0) throw new Exception($"No hay stock disponible para el artículo en el almacén de origen {line.FromWhsCod}.");
                    if ((line.U_FIB_OpQtyPkg ?? 0m) - (pesaje.WeightKg ?? 0m) < 0) throw new Exception("El peso del artículo excede el peso pendiente en el picking.");
                }
                else
                {
                    var lectura = quantity;
                    if (stock - lectura < 0) throw new Exception($"No hay stock disponible para el artículo en el almacén de origen {line.FromWhsCod}.");
                    if ((line.U_FIB_OpQtyPkg ?? 0m) - quantity < 0) throw new Exception("La cantidad del artículo excede la cantidad pendiente en el picking.");
                }

                // =========================
                // Conexión SAP + BoRecordset + Transacción 
                // =========================
                company = _companyProviderSap.GetCompany();
                rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
                if (!company.InTransaction) company.StartTransaction();

                // =========================
                // Crear registro en UDO (FIB_OPKG)
                // =========================
                oCompService = company.GetCompanyService();
                oGeneralService = oCompService.GetGeneralService("FIB_OPKG");
                oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                oGeneralData.SetProperty("U_BaseEntry", header.DocEntry);
                oGeneralData.SetProperty("U_BaseNum", header.DocNum);
                oGeneralData.SetProperty("U_BaseType", header.ObjType);
                oGeneralData.SetProperty("U_BaseLine", line.LineNum);
                oGeneralData.SetProperty("U_DocDate", header.DocDate);
                oGeneralData.SetProperty("U_TaxDate", header.TaxDate);
                oGeneralData.SetProperty("U_DocDueDate", header.DocDueDate);

                if (!string.IsNullOrWhiteSpace(header.CardCode)) oGeneralData.SetProperty("U_CardCode", header.CardCode);
                if (!string.IsNullOrWhiteSpace(header.CardName)) oGeneralData.SetProperty("U_CardName", header.CardName);
                if (!string.IsNullOrWhiteSpace(line.ItemCode)) oGeneralData.SetProperty("U_ItemCode", line.ItemCode);
                if (!string.IsNullOrWhiteSpace(line.Dscription)) oGeneralData.SetProperty("U_Dscription", line.Dscription);
                if (!string.IsNullOrWhiteSpace(pesaje.CodeBar)) oGeneralData.SetProperty("U_CodeBar", pesaje.CodeBar);
                if (!string.IsNullOrWhiteSpace(line.FromWhsCod)) oGeneralData.SetProperty("U_FromWhsCod", line.FromWhsCod);
                if (!string.IsNullOrWhiteSpace(line.WhsCode)) oGeneralData.SetProperty("U_WhsCode", line.WhsCode);

                oGeneralData.SetProperty("U_UnitMsr", line.UnitMsr);
                oGeneralData.SetProperty("U_Quantity", Convert.ToDouble(Math.Round(quantity, 6)));
                oGeneralData.SetProperty("U_WeightKg", Convert.ToDouble(Math.Round(pesaje.WeightKg ?? 0m, 6)));
                oGeneralData.SetProperty("U_NumBulk", Convert.ToDouble(Math.Round(1m, 6)));
                if (value.U_UsrCreate != null) oGeneralData.SetProperty("U_UsrCreate", value.U_UsrCreate);

                oGeneralService.Add(oGeneralData);

                // =========================
                // DI API: Actualiza Solicitud de Traslado (Inventory Transfer Request)
                // Nota: esto aplica para el documento de traslado (si BaseType 1250000001).
                // Si BaseType 17 (Orden de venta), aquí normalmente NO deberías tocar oInventoryTransferRequest.
                // =========================
                if (baseType == 1250000001)
                {
                    stockTransfer = UpdateInventoryTransactionsPickingInSap(company: company, line: line, pesoKg: pesaje.WeightKg, quantity: quantity, decimales: 6);
                }
                else if (baseType == 17)
                {
                    order = UpdateOrdersPickingInSap(company: company, line: line, pesoKg: pesaje.WeightKg, quantity: quantity, decimales: 6);
                }
                else if (baseType == 13)
                {
                    //invoices = UpdateInvoicesPickingInSap(company: company, line: line, pesoKg: pesaje.WeightKg, quantity: quantity, decimales: 6);
                    UpdateInvoicesPickingInSap_SQL(rs: rs, line: line, pesoKg: pesaje.WeightKg, quantity: quantity, decimales: 6);
                }


                // ✅ Commit SAP
                if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_Commit);


                // =========================
                // EF: Resumen de picking (según BaseType)
                // =========================
                var list = await GetPickingResumenByBaseTypeAsync(value);

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.dataList = list;
                resultTransaccion.ResultadoDescripcion = "Realizado con éxito.";

                return resultTransaccion;
            }
            catch (Exception ex)
            {
                if (company != null && company.Connected)
                {
                    if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_RollBack);
                }

                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;

                return resultTransaccion;
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(oGeneralData, oCompService, oGeneralService, stockTransfer, order, invoices, rs);
            }
        }

        /// <summary>
        /// ✅ Selector único: según BaseType decide qué consulta usar
        /// 67 => Solicitud de Traslado
        /// 17 => Orden de Venta
        /// 13 => Factura de reserva
        /// </summary>
        private async Task<(PickingCreateEntity Header, Picking1CreateEntity Line)> GetHeaderAndPendingLineByBaseTypeAsync(int baseType, int baseEntry, string itemCode, string fromWhsCod)
        {
            return baseType switch
            {
                1250000001 => await GetInventoryTransferRequestdHeaderAndPendingLineAsync(baseEntry, itemCode, fromWhsCod),
                17 => await GetOrdersHeaderAndPendingLineAsync(baseEntry, itemCode, fromWhsCod),
                13 => await GetInvoicesHeaderAndPendingLineAsync(baseEntry, itemCode, fromWhsCod),
                _ => throw new Exception($"BaseType no soportado para picking: {baseType}.")
            };
        }
        private async Task<(PickingCreateEntity Header, Picking1CreateEntity Line)> GetInventoryTransferRequestdHeaderAndPendingLineAsync(int baseEntry, string itemCode, string fromWhsCod)
        {
            var header = await _dbSap.InventoryTransferRequest
                .AsNoTracking()
                .Select(n => new PickingCreateEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    ObjType = n.ObjType,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    CardCode = n.CardCode,
                    CardName = n.CardName
                })
                .FirstOrDefaultAsync(n => n.DocEntry == baseEntry)
                ?? throw new Exception("No existe la solicitud de traslado.");

            var existsItemInDoc = await _dbSap.InventoryTransferRequest1
                .AsNoTracking()
                .AnyAsync(n => n.DocEntry == baseEntry && n.ItemCode == itemCode);

            if (!existsItemInDoc)
                throw new Exception("No se encontró el artículo en la solicitud de traslado para el código de barras ingresado.");

            var line = await _dbSap.InventoryTransferRequest1
                .AsNoTracking()
                .Where(n => n.DocEntry == baseEntry
                         && n.ItemCode == itemCode
                         && n.LineStatus == "O"
                         && n.U_FIB_LinStPkg == "O"
                         && (n.U_FIB_OpQtyPkg ?? 0) > 0)
                .OrderBy(n => n.LineNum)
                .Select(n => new Picking1CreateEntity
                {
                    ObjType = n.ObjType,
                    DocEntry = n.DocEntry,
                    LineNum = n.LineNum,
                    U_FIB_LinStPkg = n.U_FIB_LinStPkg,
                    ItemCode = n.ItemCode,
                    Dscription = n.Dscription,
                    WhsCode = n.WhsCode,
                    FromWhsCod = fromWhsCod,
                    UnitMsr = string.IsNullOrWhiteSpace(n.UnitMsr) ? "KG" : n.UnitMsr.ToUpper().Trim(),
                    U_FIB_OpQtyPkg = n.U_FIB_OpQtyPkg
                })
                .FirstOrDefaultAsync()
                ?? throw new Exception("Este artículo no presenta pendientes de picking.");

            return (header, line);
        }
        private async Task<(PickingCreateEntity Header, Picking1CreateEntity Line)> GetOrdersHeaderAndPendingLineAsync(int baseEntry, string itemCode, string fromWhsCod)
        {
            var header = await _dbSap.Orders
                .AsNoTracking()
                .Select(n => new PickingCreateEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    ObjType = n.ObjType,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    CardCode = n.CardCode,
                    CardName = n.CardName
                })
                .FirstOrDefaultAsync(n => n.DocEntry == baseEntry)
                ?? throw new Exception("No existe la orden de venta.");

            var existsItemInDoc = await _dbSap.Orders1
                .AsNoTracking()
                .AnyAsync(n => n.DocEntry == baseEntry && n.ItemCode == itemCode);

            if (!existsItemInDoc)
                throw new Exception("No se encontró el artículo en la orden de venta para el código de barras ingresado.");

            var line = await _dbSap.Orders1
                .AsNoTracking()
                .Where(n => n.DocEntry == baseEntry
                         && n.ItemCode == itemCode
                         && n.LineStatus == "O"
                         && n.U_FIB_LinStPkg == "O"
                         && (n.U_FIB_OpQtyPkg ?? 0) > 0)
                .OrderBy(n => n.LineNum)
                .Select(n => new Picking1CreateEntity
                {
                    ObjType = n.ObjType,
                    DocEntry = n.DocEntry,
                    LineNum = n.LineNum,
                    U_FIB_LinStPkg = n.U_FIB_LinStPkg,
                    ItemCode = n.ItemCode,
                    Dscription = n.Dscription,
                    WhsCode = n.WhsCode,
                    FromWhsCod = fromWhsCod,
                    UnitMsr = string.IsNullOrWhiteSpace(n.UnitMsr) ? "KG" : n.UnitMsr.ToUpper().Trim(),
                    U_FIB_OpQtyPkg = n.U_FIB_OpQtyPkg
                })
                .FirstOrDefaultAsync()
                ?? throw new Exception("Este artículo no presenta pendientes de picking.");

            return (header, line);
        }
        private async Task<(PickingCreateEntity Header, Picking1CreateEntity Line)> GetInvoicesHeaderAndPendingLineAsync(int baseEntry, string itemCode, string fromWhsCod)
        {
            var header = await _dbSap.Invoices
                .AsNoTracking()
                .Select(n => new PickingCreateEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    ObjType = n.ObjType,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    CardCode = n.CardCode,
                    CardName = n.CardName
                })
                .FirstOrDefaultAsync(n => n.DocEntry == baseEntry)
                ?? throw new Exception("No existe la factura de reserve.");

            var existsItemInDoc = await _dbSap.Invoices1
                .AsNoTracking()
                .AnyAsync(n => n.DocEntry == baseEntry && n.ItemCode == itemCode);

            if (!existsItemInDoc)
                throw new Exception("No se encontró el artículo en la factura de reserve para el código de barras ingresado.");

            var line = await _dbSap.Invoices1
                .AsNoTracking()
                .Where(n => n.DocEntry == baseEntry
                         && n.ItemCode == itemCode
                         && n.LineStatus == "O"
                         && n.U_FIB_LinStPkg == "O"
                         && (n.U_FIB_OpQtyPkg ?? 0) > 0)
                .OrderBy(n => n.LineNum)
                .Select(n => new Picking1CreateEntity
                {
                    ObjType = n.ObjType,
                    DocEntry = n.DocEntry,
                    LineNum = n.LineNum,
                    U_FIB_LinStPkg = n.U_FIB_LinStPkg,
                    ItemCode = n.ItemCode,
                    Dscription = n.Dscription,
                    WhsCode = n.WhsCode,
                    FromWhsCod = fromWhsCod,
                    UnitMsr = string.IsNullOrWhiteSpace(n.UnitMsr) ? "KG" : n.UnitMsr.ToUpper().Trim(),
                    U_FIB_OpQtyPkg = n.U_FIB_OpQtyPkg
                })
                .FirstOrDefaultAsync()
                ?? throw new Exception("Este artículo no presenta pendientes de picking.");

            return (header, line);
        }


        /// <summary>
        /// ✅ Selector único: según BaseType actualiza el documento
        /// 67 => Solicitud de Traslado
        /// 17 => Orden de Venta
        /// 13 => Factura de reserva
        private StockTransfer UpdateInventoryTransactionsPickingInSap(Company company, Picking1CreateEntity line, decimal? pesoKg, decimal quantity, int decimales = 6)
        {
            var inventoryTransferRequest = (StockTransfer)company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);

            if (!inventoryTransferRequest.GetByKey(line.DocEntry))
                throw new Exception("No existe la solicitud de traslado.");

            if (inventoryTransferRequest.Lines.Count == 0)
                throw new Exception("No existe el detalle de la solicitud de traslado.");

            decimal lectura = line.UnitMsr == "KG" ? pesoKg ?? 0m : quantity;
            decimal opQtyPicking = (line.U_FIB_OpQtyPkg ?? 0m) - lectura;
            if (opQtyPicking < 0m) opQtyPicking = 0m;

            bool foundLine = false;

            // 1) Actualiza la línea
            for (int i = 0; i < inventoryTransferRequest.Lines.Count; i++)
            {
                inventoryTransferRequest.Lines.SetCurrentLine(i);

                if (inventoryTransferRequest.Lines.LineNum != line.LineNum || inventoryTransferRequest.Lines.ItemCode != line.ItemCode)
                    continue;

                foundLine = true;

                inventoryTransferRequest.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = Convert.ToDouble(Math.Round(opQtyPicking, decimales));

                if (opQtyPicking == 0m)
                    inventoryTransferRequest.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = "C";

                var ret = inventoryTransferRequest.Update();
                if (ret != 0) throw new Exception(company.GetLastErrorDescription());

                break;
            }

            if (!foundLine)
                throw new Exception($"No se encontró la línea {line.LineNum} del artículo {line.ItemCode} en la solicitud de traslado.");

            // 2) Cerrar cabecera si no hay pendientes
            bool existsOpenPkg = false;

            for (int i = 0; i < inventoryTransferRequest.Lines.Count; i++)
            {
                inventoryTransferRequest.Lines.SetCurrentLine(i);

                var linSt = Convert.ToString(inventoryTransferRequest.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value ?? "O");
                var opQty = Convert.ToDouble(inventoryTransferRequest.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0);

                if (linSt == "O" && opQty > 0)
                {
                    existsOpenPkg = true;
                    break;
                }
            }

            if (!existsOpenPkg)
            {
                inventoryTransferRequest.UserFields.Fields.Item("U_FIB_DocStPkg").Value = "C";
                var ret = inventoryTransferRequest.Update();
                if (ret != 0) throw new Exception(company.GetLastErrorDescription());
            }

            return inventoryTransferRequest;
        }
        private Documents UpdateOrdersPickingInSap(Company company, Picking1CreateEntity line, decimal? pesoKg, decimal quantity,int decimales = 6)
        {
            var orders = (Documents)company.GetBusinessObject(BoObjectTypes.oOrders);

            if (!orders.GetByKey(line.DocEntry))
                throw new Exception("No existe la orden de venta.");

            if (orders.Lines.Count == 0)
                throw new Exception("No existe el detalle de la orden de venta.");

            decimal lectura = line.UnitMsr == "KG" ? pesoKg ?? 0m : quantity;
            decimal opQtyPicking = (line.U_FIB_OpQtyPkg ?? 0m) - lectura;
            if (opQtyPicking < 0m) opQtyPicking = 0m;

            bool foundLine = false;

            // 1) Actualiza la línea
            for (int i = 0; i < orders.Lines.Count; i++)
            {
                orders.Lines.SetCurrentLine(i);

                if (orders.Lines.LineNum != line.LineNum || orders.Lines.ItemCode != line.ItemCode)
                    continue;

                foundLine = true;

                orders.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value =
                    Convert.ToDouble(Math.Round(opQtyPicking, decimales));

                if (opQtyPicking == 0m)
                    orders.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = "C";

                var ret = orders.Update();
                if (ret != 0) throw new Exception(company.GetLastErrorDescription());

                break;
            }

            if (!foundLine)
                throw new Exception($"No se encontró la línea {line.LineNum} del artículo {line.ItemCode} en la orden de venta.");

            // 2) Cerrar cabecera si no hay pendientes
            bool existsOpenPkg = false;

            for (int i = 0; i < orders.Lines.Count; i++)
            {
                orders.Lines.SetCurrentLine(i);

                var linSt = Convert.ToString(orders.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value ?? "O");
                var opQty = Convert.ToDouble(orders.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0);

                if (linSt == "O" && opQty > 0)
                {
                    existsOpenPkg = true;
                    break;
                }
            }

            if (!existsOpenPkg)
            {
                orders.UserFields.Fields.Item("U_FIB_DocStPkg").Value = "C";
                var ret = orders.Update();
                if (ret != 0) throw new Exception(company.GetLastErrorDescription());
            }

            return orders;
        }
        private Documents UpdateInvoicesPickingInSap(Company company, Picking1CreateEntity line, decimal? pesoKg, decimal quantity, int decimales = 6)
        {
            var invoices = (Documents)company.GetBusinessObject(BoObjectTypes.oInvoices);

            if (!invoices.GetByKey(line.DocEntry))
                throw new Exception("No existe la factura de reserva.");

            if (invoices.Lines.Count == 0)
                throw new Exception("No existe el detalle de la factura de reserva.");

            decimal lectura = line.UnitMsr == "KG" ? (pesoKg ?? 0m) : quantity;
            decimal opQtyPicking = (line.U_FIB_OpQtyPkg ?? 0m) - lectura;

            if (opQtyPicking < 0m)
                opQtyPicking = 0m;

            bool foundLine = false;

            // =========================
            // 1) Buscar y actualizar línea
            // =========================
            for (int i = 0; i < invoices.Lines.Count; i++)
            {
                invoices.Lines.SetCurrentLine(i);

                // 🔥 Validación más segura
                if (invoices.Lines.ItemCode != line.ItemCode)
                    continue;

                // (Opcional) Si quieres ser más estricto:
                if (invoices.Lines.LineNum != line.LineNum)
                    continue;

                foundLine = true;

                // 👉 Actualiza UDFs
                invoices.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = Convert.ToDouble(Math.Round(opQtyPicking, decimales));

                if (opQtyPicking == 0m)
                {
                    invoices.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = "C";
                }

                break; // 🔥 IMPORTANTE
            }

            if (!foundLine)
                throw new Exception($"No se encontró la línea {line.LineNum} del artículo {line.ItemCode} en la factura.");

            // =========================
            // 2) UPDATE (FUERA DEL LOOP)
            // =========================
            var ret = invoices.Update();
            if (ret != 0)
                throw new Exception(company.GetLastErrorDescription());

            // =========================
            // 3) Verificar si quedan pendientes
            // =========================
            bool existsOpenPkg = false;

            for (int i = 0; i < invoices.Lines.Count; i++)
            {
                invoices.Lines.SetCurrentLine(i);

                var linStObj = invoices.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value;
                var opQtyObj = invoices.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value;

                string linSt = Convert.ToString(linStObj ?? "O");
                double opQty = Convert.ToDouble(opQtyObj ?? 0);

                if (linSt == "O" && opQty > 0)
                {
                    existsOpenPkg = true;
                    break;
                }
            }

            // =========================
            // 4) Cerrar cabecera si no hay pendientes
            // =========================
            if (!existsOpenPkg)
            {
                invoices.UserFields.Fields.Item("U_FIB_DocStPkg").Value = "C";

                var ret2 = invoices.Update();
                if (ret2 != 0)
                    throw new Exception(company.GetLastErrorDescription());
            }

            return invoices;
        }
        private void UpdateInvoicesPickingInSap_SQL(Recordset rs, Picking1CreateEntity line, decimal? pesoKg, decimal quantity, int decimales = 6)
        {
            decimal lectura = line.UnitMsr == "KG" ? (pesoKg ?? 0m) : quantity;

            decimal opQtyPicking = (line.U_FIB_OpQtyPkg ?? 0m) - lectura;
            
            if (opQtyPicking < 0m)
                opQtyPicking = 0m;

            string opQtyStr = Convert.ToDouble(Math.Round(opQtyPicking, decimales)).ToString(CultureInfo.InvariantCulture);

            string estadoLinea = opQtyPicking == 0 ? "C" : "O";

            rs.DoQuery($@"
                UPDATE INV1
                SET 
                    U_FIB_OpQtyPkg = {opQtyStr},
                    U_FIB_LinStPkg = '{estadoLinea}'
                WHERE 
                    DocEntry = {line.DocEntry}
                    AND LineNum = {line.LineNum}
            ");

            rs.DoQuery($@"
                SELECT COUNT(*) CNT
                FROM INV1
                WHERE DocEntry = {line.DocEntry}
                  AND U_FIB_LinStPkg = 'O'
                  AND ISNULL(U_FIB_OpQtyPkg,0) > 0
            ");

            int pendientes = 0;

            if (!rs.EoF)
                pendientes = Convert.ToInt32(rs.Fields.Item("CNT").Value);

            if (pendientes == 0)
            {
                rs.DoQuery($@"
                    UPDATE OINV
                    SET U_FIB_DocStPkg = 'C'
                    WHERE DocEntry = {line.DocEntry}
                ");
            }
        }
        
        /// <summary>
        /// ✅ Selector único: según BaseType obtiene el resumen de picking
        /// 67 => Solicitud de Traslado
        /// 17 => Orden de Venta
        /// 13 => Factura de reserva
        private async Task<List<PickingQueryEntity>> GetPickingResumenByBaseTypeAsync(PickingEntity value)
        {
            return value.U_BaseType switch
            {
                1250000001 => await GetPickingInventoryTransactionsResumenAsync(value),
                17 => await GetPickingOrdersResumenAsync(value),
                13 => await GetPickingIvoicesResumenAsync(value),
                _ => throw new Exception($"BaseType no soportado para resumen de picking: {value.U_BaseType}.")
            };
        }
        private async Task<List<PickingQueryEntity>> GetPickingInventoryTransactionsResumenAsync(PickingEntity value)
        {
            var query =
                from p in _dbSap.Picking.AsNoTracking()
                where p.U_Status == "O" && p.U_BaseType == value.U_BaseType && p.U_BaseEntry == value.U_BaseEntry
                join s in _dbSap.InventoryTransferRequest1.AsNoTracking() on new { e = p.U_BaseEntry, l = p.U_BaseLine } equals new { e = (int?)s.DocEntry, l = (int?)s.LineNum } into ps
                from s in ps.Where(x => x.ObjType == value.U_BaseType.ToString()).DefaultIfEmpty()
                group new { p, s } by new
                {
                    p.U_Status,
                    p.U_BaseEntry,
                    p.U_BaseNum,
                    p.U_BaseType,
                    p.U_BaseLine,
                    p.U_ItemCode,
                    p.U_Dscription,
                    p.U_FromWhsCod,
                    p.U_WhsCode,
                    p.U_UnitMsr
                }
                into m
                select new PickingQueryEntity
                {
                    U_Status = m.Key.U_Status,
                    U_BaseEntry = m.Key.U_BaseEntry,
                    U_BaseNum = m.Key.U_BaseNum,
                    U_BaseType = m.Key.U_BaseType,
                    U_BaseLine = m.Key.U_BaseLine,
                    U_ItemCode = m.Key.U_ItemCode,
                    U_Dscription = m.Key.U_Dscription,
                    U_FromWhsCod = m.Key.U_FromWhsCod,
                    U_WhsCode = m.Key.U_WhsCode,
                    U_UnitMsr = m.Key.U_UnitMsr,
                    U_Quantity = m.Sum(x => x.p.U_Quantity ?? 0),
                    U_WeightKg = m.Sum(x => x.p.U_WeightKg ?? 0),
                    U_NumBulk = m.Sum(x => x.p.U_NumBulk ?? 0),
                    U_FIB_OpQtyPkg = m.Max(x => x.s != null ? x.s.U_FIB_OpQtyPkg ?? 0 : 0)
                };

            return await query.OrderBy(n => n.U_BaseLine).ToListAsync();
        }
        private async Task<List<PickingQueryEntity>> GetPickingOrdersResumenAsync(PickingEntity value)
        {
            var query =
                from p in _dbSap.Picking.AsNoTracking()
                where p.U_Status == "O" && p.U_BaseType == value.U_BaseType && p.U_BaseEntry == value.U_BaseEntry
                join s in _dbSap.Orders1.AsNoTracking() on new { e = p.U_BaseEntry, l = p.U_BaseLine } equals new { e = (int?)s.DocEntry, l = (int?)s.LineNum } into ps
                from s in ps.Where(x => x.ObjType == value.U_BaseType.ToString()).DefaultIfEmpty()
                group new { p, s } by new
                {
                    p.U_Status,
                    p.U_BaseEntry,
                    p.U_BaseNum,
                    p.U_BaseType,
                    p.U_BaseLine,
                    p.U_ItemCode,
                    p.U_Dscription,
                    p.U_FromWhsCod,
                    p.U_WhsCode,
                    p.U_UnitMsr
                }
                into m
                select new PickingQueryEntity
                {
                    U_Status = m.Key.U_Status,
                    U_BaseEntry = m.Key.U_BaseEntry,
                    U_BaseNum = m.Key.U_BaseNum,
                    U_BaseType = m.Key.U_BaseType,
                    U_BaseLine = m.Key.U_BaseLine,
                    U_ItemCode = m.Key.U_ItemCode,
                    U_Dscription = m.Key.U_Dscription,
                    U_FromWhsCod = m.Key.U_FromWhsCod,
                    U_WhsCode = m.Key.U_WhsCode,
                    U_UnitMsr = m.Key.U_UnitMsr,
                    U_Quantity = m.Sum(x => x.p.U_Quantity ?? 0),
                    U_WeightKg = m.Sum(x => x.p.U_WeightKg ?? 0),
                    U_NumBulk = m.Sum(x => x.p.U_NumBulk ?? 0),
                    U_FIB_OpQtyPkg = m.Max(x => x.s != null ? x.s.U_FIB_OpQtyPkg ?? 0 : 0)
                };

            return await query.OrderBy(n => n.U_BaseLine).ToListAsync();
        }
        private async Task<List<PickingQueryEntity>> GetPickingIvoicesResumenAsync(PickingEntity value)
        {
            var query =
                from p in _dbSap.Picking.AsNoTracking()
                where p.U_Status == "O" && p.U_BaseType == value.U_BaseType && p.U_BaseEntry == value.U_BaseEntry
                join s in _dbSap.Invoices1.AsNoTracking() on new { e = p.U_BaseEntry, l = p.U_BaseLine } equals new { e = (int?)s.DocEntry, l = (int?)s.LineNum } into ps
                from s in ps.Where(x => x.ObjType == value.U_BaseType.ToString()).DefaultIfEmpty()
                group new { p, s } by new
                {
                    p.U_Status,
                    p.U_BaseEntry,
                    p.U_BaseNum,
                    p.U_BaseType,
                    p.U_BaseLine,
                    p.U_ItemCode,
                    p.U_Dscription,
                    p.U_FromWhsCod,
                    p.U_WhsCode,
                    p.U_UnitMsr
                }
                into m
                select new PickingQueryEntity
                {
                    U_Status = m.Key.U_Status,
                    U_BaseEntry = m.Key.U_BaseEntry,
                    U_BaseNum = m.Key.U_BaseNum,
                    U_BaseType = m.Key.U_BaseType,
                    U_BaseLine = m.Key.U_BaseLine,
                    U_ItemCode = m.Key.U_ItemCode,
                    U_Dscription = m.Key.U_Dscription,
                    U_FromWhsCod = m.Key.U_FromWhsCod,
                    U_WhsCode = m.Key.U_WhsCode,
                    U_UnitMsr = m.Key.U_UnitMsr,
                    U_Quantity = m.Sum(x => x.p.U_Quantity ?? 0),
                    U_WeightKg = m.Sum(x => x.p.U_WeightKg ?? 0),
                    U_NumBulk = m.Sum(x => x.p.U_NumBulk ?? 0),
                    U_FIB_OpQtyPkg = m.Max(x => x.s != null ? x.s.U_FIB_OpQtyPkg ?? 0 : 0)
                };

            return await query.OrderBy(n => n.U_BaseLine).ToListAsync();
        }


        public async Task<ResultadoTransaccionResponse<PickingEntity>> SetRelease(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PickingEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            StockTransfer stockTransfer = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralParams = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Se valida que el código de barras no esté ya liberado
                    if (_dbSap.Picking.Any(n => n.U_Status == "C" && n.U_IsReleased == "Y" && n.U_BaseType == value.U_BaseType && n.U_CodeBar == value.U_CodeBar))
                    {
                        throw new Exception("El código de barras ya está liberado.");
                    }


                    // Se obtiene el picking a liberar
                    var pickingToRelease = _dbSap.Picking.Where(n => n.U_Status == "C" && n.U_IsReleased == "N" && n.U_BaseType == value.U_BaseType && n.U_CodeBar == value.U_CodeBar)
                    .FirstOrDefault() ?? throw new Exception("El código de barras no está pendiente de liberación.");


                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Se instancia el servicio de objetos generales
                    oCompService = company.GetCompanyService();
                    // Se instancia el objeto de datos generales
                    oGeneralService = oCompService.GetGeneralService("FIB_OPKG");
                    // Se instancia el objeto de datos generales
                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                    // Se asigna el valor de la llave primaria del registro a eliminar
                    oGeneralParams.SetProperty("DocEntry", pickingToRelease.DocEntry);
                    // Se asigna el ID de registro a eliminar
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    // Se actualiza el estado del registro a "Y" (Liberado)
                    oGeneralData.SetProperty("U_IsReleased", value.U_IsReleased);
                    oGeneralData.SetProperty("U_UsrRelease", value.U_UsrRelease);
                    // Se actualiza el registro
                    oGeneralService.Update(oGeneralData);


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Registro eliminado con éxito.";
                }
                catch (Exception ex)
                {
                    if (company != null && company.Connected)
                    {
                        if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }

                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(oGeneralData, oCompService, oGeneralService, oGeneralParams, stockTransfer);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionResponse<PickingEntity>> SetDelete(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PickingEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            StockTransfer stockTransfer = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralParams = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Se obtiene la línea de picking a eliminar
                    var line = _dbSap.Picking.Where(n => n.U_Status == "O" && n.DocEntry == value.DocEntry).FirstOrDefault() ?? throw new Exception("No se encontró el picking pendiente a eliminar.");


                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Empieza la transacción en SAP
                    if (!company.InTransaction) company.StartTransaction();


                    // Se instancia el servicio de objetos generales
                    oCompService = company.GetCompanyService();
                    // Se instancia el objeto de datos generales
                    oGeneralService = oCompService.GetGeneralService("FIB_OPKG");
                    // Se instancia el objeto de datos generales
                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                    // Se asigna el valor de la llave primaria del registro a eliminar
                    oGeneralParams.SetProperty("DocEntry", line.DocEntry);
                    // Se asigna el ID de registro a eliminar
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    // Se actualiza el estado del registro a "D" (eliminado)
                    oGeneralData.SetProperty("U_Status", "D");
                    // Se actualiza el registro
                    oGeneralService.Update(oGeneralData);


                    // Se instancia el objeto de la solicitud traslado
                    stockTransfer = company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);


                    // Se obtiene la solicitud de traslado. Sino existe, se lanza una excepción
                    if (!stockTransfer.GetByKey(line.U_BaseEntry?? 0)) throw new Exception("No existe la solictud de traslado.");


                    //// Se valida que la solicitud de traslado tenga líneas en el detalle
                    if (stockTransfer.Lines.Count == 0) throw new Exception("No existe el detalle de la solictud de traslado.");


                    // Variable para contar las líneas que aún están abiertas y con cantidad pendiente por picar
                    decimal opQtyPkg = 0;


                    // Se obtiene la cantidad por paquete de la línea de la solicitud de traslado
                    for (int i = 0; i < stockTransfer.Lines.Count; i++)
                    {
                        stockTransfer.Lines.SetCurrentLine(i);
                        if (stockTransfer.Lines.LineNum == line.U_BaseLine)
                        {
                            opQtyPkg = Convert.ToDecimal(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0);
                            break;
                        }
                    }


                    // Se seuma los pesos o las cantidades leídas en el picking a la cantidad pendiente por leer en la línea de la solicitud de  traslado
                    var opQtyPicking = opQtyPkg + (line.U_UnitMsr == "KG" ? line.U_WeightKg : line.U_Quantity);


                    // Se actualiza la línea de la solicitud de  traslado
                    if (opQtyPicking != null)
                    {
                        for (int i = 0; i < stockTransfer.Lines.Count; i++)
                        {
                            stockTransfer.Lines.SetCurrentLine(i);
                            if (stockTransfer.Lines.LineNum == line.U_BaseLine)
                            {
                                stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = Convert.ToDouble(Math.Round(Convert.ToDecimal(opQtyPicking), 6));
                                stockTransfer.Update();
                                break;
                            }
                        }
                    }


                    // Se recorre las líneas de la solicitud de  traslado para actualizar el estado de picking de la línea a "C" (completada), si la cantidad pendiente por picar es cero
                    for (int i = 0; i < stockTransfer.Lines.Count; i++)
                    {
                        stockTransfer.Lines.SetCurrentLine(i);
                        if (stockTransfer.Lines.LineNum == line.U_BaseLine)
                        {
                            var linStPkg = Convert.ToString(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value ?? "C");
                            if (linStPkg == "C")
                            {
                                stockTransfer.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = "O";
                                stockTransfer.Update();
                            }
                            break;
                        }
                    }


                    // Se declara la variable para contar las líneas abiertas y con cantidad pendiente por picar en la solicitud de  traslado
                    int OpenPkg = 0;


                    // Se recorre las líneas de la solicitud de  traslado para contar las líneas que aún están abiertas y con cantidad pendiente por picar
                    for (int i = 0; i < stockTransfer.Lines.Count; i++)
                    {
                        stockTransfer.Lines.SetCurrentLine(i);
                        if (Convert.ToString(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value ?? "O") == "O" && Convert.ToDouble(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0) > 0)
                        {
                            OpenPkg++;
                            break;
                        }
                    }


                    // Si hay líneas abiertas y con cantidad pendiente por picar, se actualiza el estado de picking de la solicitud de  traslado a "O" (Pendiente)
                    if (Convert.ToString(stockTransfer.UserFields.Fields.Item("U_FIB_DocStPkg").Value) == "C" && OpenPkg > 0)
                    {
                        stockTransfer.UserFields.Fields.Item("U_FIB_DocStPkg").Value = "O";
                        stockTransfer.Update();
                    }


                    // Se finaliza la transacción en SAP
                    if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_Commit);


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Registro eliminado con éxito.";
                }
                catch (Exception ex)
                {
                    if (company != null && company.Connected)
                    {
                        if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }

                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(oGeneralData, oCompService, oGeneralService, oGeneralParams, stockTransfer);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionResponse<PickingEntity>> SetDeleteMassive(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PickingEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            StockTransfer stockTransfer = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralParams = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Se consulta la lista de picking registrados en la base de datos
                    var lines = _dbSap.Picking.Where(n => n.U_Status == "O" && n.U_BaseEntry == value.U_BaseEntry && n.U_BaseType == value.U_BaseType && n.U_BaseLine == value.U_BaseLine).ToList();


                    // Se valida que existan pickings para eliminar
                    if (!lines.Any()) throw new Exception("No se encontraron pickings pendientes por eliminar..");


                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    // Empieza la transacción en SAP
                    if (!company.InTransaction) company.StartTransaction();


                    // Se instancia el servicio de objetos generales
                    oCompService = company.GetCompanyService();
                    // se instancia el objeto de datos generales
                    oGeneralService = oCompService.GetGeneralService("FIB_OPKG");


                    // Se recorre la lista de pickings a eliminar
                    foreach (var line in lines)
                    {
                        // Se instancia el objeto de datos generales
                        oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                        // Se asigna el valor de la llave primaria del registro a eliminar
                        oGeneralParams.SetProperty("DocEntry", line.DocEntry);
                        // Se asigna el ID de registro a eliminar
                        oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                        // Se actualiza el estado del registro a "D" (eliminado)
                        oGeneralData.SetProperty("U_Status", "D");
                        // Se actualiza el registro
                        oGeneralService.Update(oGeneralData);


                        // Se instancia el objeto de la solicitud  traslado
                        stockTransfer = company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);


                        // Se obtiene la solicitud de  traslado. Sino existe, se lanza una excepción
                        if (!stockTransfer.GetByKey(line.U_BaseEntry ?? 0)) throw new Exception("No existe la solictud de traslado.");


                        //// Se valida que la solicitud de  traslado tenga líneas en el detalle
                        if (stockTransfer.Lines.Count == 0) throw new Exception("No existe el detalle de la solictud de traslado.");


                        // Variable para contar las líneas que aún están abiertas y con cantidad pendiente por picar
                        decimal opQtyPkg = 0;


                        // Se obtiene la cantidad por paquete de la línea de la solicitud de  traslado
                        for (int i = 0; i < stockTransfer.Lines.Count; i++)
                        {
                            stockTransfer.Lines.SetCurrentLine(i);
                            if (stockTransfer.Lines.LineNum == line.U_BaseLine)
                            {
                                opQtyPkg = Convert.ToDecimal(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0);
                                break;
                            }
                        }


                        // Se seuma los pesos o las cantidades leídas en el picking a la cantidad pendiente por leer en la línea de la solicitud de  traslado
                        var opQtyPicking = opQtyPkg + (line.U_UnitMsr == "KG" ? line.U_WeightKg : line.U_Quantity);


                        // Se actualiza la línea de la solicitud de  traslado
                        if (opQtyPicking != null)
                        {
                            for (int i = 0; i < stockTransfer.Lines.Count; i++)
                            {
                                stockTransfer.Lines.SetCurrentLine(i);
                                if (stockTransfer.Lines.LineNum == line.U_BaseLine)
                                {
                                    stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = Convert.ToDouble(Math.Round(Convert.ToDecimal(opQtyPicking), 6));
                                    stockTransfer.Update();
                                    break;
                                }
                            }
                        }


                        // Se recorre las líneas de la solicitud de  traslado para actualizar el estado de picking de la línea a "C" (completada), si la cantidad pendiente por picar es cero
                        for (int i = 0; i < stockTransfer.Lines.Count; i++)
                        {
                            stockTransfer.Lines.SetCurrentLine(i);
                            if (stockTransfer.Lines.LineNum == line.U_BaseLine)
                            {
                                var linStPkg = Convert.ToString(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value ?? "C");
                                if (linStPkg == "C")
                                {
                                    stockTransfer.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = "O";
                                    stockTransfer.Update();
                                }
                                break;
                            }
                        }


                        // Se declara la variable para contar las líneas abiertas y con cantidad pendiente por picar en la solicitud de  traslado
                        int OpenPkg = 0;


                        // Se recorre las líneas de la solicitud de  traslado para contar las líneas que aún están abiertas y con cantidad pendiente por picar
                        for (int i = 0; i < stockTransfer.Lines.Count; i++)
                        {
                            stockTransfer.Lines.SetCurrentLine(i);
                            if (Convert.ToString(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value ?? "O") == "O" && Convert.ToDouble(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0) > 0)
                            {
                                OpenPkg++;
                                break;
                            }
                        }


                        // Si hay líneas abiertas y con cantidad pendiente por picar, se actualiza el estado de picking de la solicitud de  traslado a "O" (Pendiente)
                        if (Convert.ToString(stockTransfer.UserFields.Fields.Item("U_FIB_DocStPkg").Value) == "C" && OpenPkg > 0)
                        {
                            stockTransfer.UserFields.Fields.Item("U_FIB_DocStPkg").Value = "O";
                            stockTransfer.Update();
                        }
                    }


                    // Se finaliza la transacción en SAP
                    if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_Commit);


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Registro eliminado con éxito.";
                }
                catch (Exception ex)
                {
                    if (company != null && company.Connected)
                    {
                        if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }

                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(oGeneralData, oCompService, oGeneralService, oGeneralParams, stockTransfer);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetPickingPrint(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await
                (
                    from p in _dbSap.Picking
                    join t in _dbSap.StockTransfers on new { DocEntry = p.U_TrgetEntry, ObjType = p.U_TargetType.ToString() } equals new { DocEntry = (int?)t.DocEntry, t.ObjType }
                    where p.U_Status == "C" && p.U_TargetType == value.U_TargetType && p.U_TrgetEntry == value.U_TrgetEntry
                    select new PickingQueryEntity
                    {
                        U_TrgetEntry = p.U_TrgetEntry,
                        U_TargetType = p.U_TargetType,
                        U_TrgetDocNum = t.DocNum,
                        U_TrgetDocDate = t.DocDate,
                        U_TrgetNumber = t.U_BPP_MDSD + "-" + t.U_BPP_MDCD,
                        U_CardName = p.U_CardName,
                        U_Contenedor = string.Empty
                    }
                )
                .Distinct()
                .FirstOrDefaultAsync() ?? throw new Exception("No exiten registros de picking.");


                var lista = await
                (
                    from p in _dbSap.Picking
                    where p.U_Status == "C" && p.U_TargetType == value.U_TargetType && p.U_TrgetEntry == value.U_TrgetEntry
                    select new PickingQueryEntity
                    {
                        U_TrgetEntry = p.U_TrgetEntry,
                        U_TargetType = p.U_TargetType,
                        U_TrgetLine = p.U_TrgetLine,
                        U_Dscription = p.U_Dscription,
                        U_CodeBar = p.U_CodeBar,
                        U_WeightKg = p.U_WeightKg
                    }
                )
                .ToListAsync() ?? throw new Exception("No exiten registros de picking.");


                iTextSharp.text.Document doc = new iTextSharp.text.Document();
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                if(string.IsNullOrEmpty(data.U_CardName))
                {
                    doc.SetMargins(15f, 10f, 120f, 15f);
                }
                else
                {
                    doc.SetMargins(15f, 10f, 120f, 15f);
                }
                MemoryStream ms = new MemoryStream();
                iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                // Our custom Header and Footer is done using Event Handler
                PageEventHelperPicking pageEventHelperPicking = new PageEventHelperPicking();
                write.PageEvent = pageEventHelperPicking;

                // Colocamos la fuente que deseamos que tenga el documento
                iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
                // Titulo
                iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 11f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
                iTextSharp.text.Font parrafoItem = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 11f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                iTextSharp.text.Font parrafoNegroItalic = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.Black);

                // Define the page header
                pageEventHelperPicking.Title = "PICKING LIST";
                pageEventHelperPicking.DocNum = data.U_TrgetDocNum?.ToString() ?? "";
                pageEventHelperPicking.DocDate = data.U_TrgetDocDate?.ToString("dd/MM/yyyy") ?? "";
                pageEventHelperPicking.NumeroGuia = data.U_TrgetNumber ?? "";
                pageEventHelperPicking.Cliente = data.U_CardName ?? "";
                pageEventHelperPicking.Contenedor = data.U_Contenedor ?? "";


                // Abrimos el documento
                doc.Open();


                var listPicking = lista
                .Select(n => new PickingEntity 
                {
                    U_TrgetEntry = n.U_TrgetEntry,
                    U_TargetType = n.U_TargetType,
                    U_TrgetLine = n.U_TrgetLine,
                    U_Dscription = n.U_Dscription
                })
                .DistinctBy(x => new 
                {
                    x.U_TrgetEntry,
                    x.U_TargetType,
                    x.U_TrgetLine,
                    x.U_Dscription
                })
                .ToList();


                foreach (var linea in listPicking)
                {
                    //============================
                    //Tabla: 2
                    var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 100f }) { WidthPercentage = 100 };
                    //Línea 1
                    var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(linea.U_Dscription, parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                    tbl.AddCell(c1);

                    
                    // Agregamos la tabla al documento
                    doc.Add(tbl);


                    var listCodeBar = lista
                    .Where(n => n.U_TrgetEntry == linea.U_TrgetEntry && n.U_TargetType == linea.U_TargetType && n.U_TrgetLine == linea.U_TrgetLine)
                    .Select(n => new PickingEntity
                    {
                        U_CodeBar = n.U_CodeBar,
                        U_WeightKg = n.U_WeightKg
                    })
                    .ToList();


                    var codeBars = listCodeBar
                   .Select(x => $"{x.U_CodeBar} / {x.U_WeightKg?.ToString("#,##0.00", CultureInfo.InvariantCulture)}")
                   .ToList();


                    //============================
                    //Tabla: 3
                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100 };


                    for (int i = 0; i < codeBars.Count; i += 4)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            string valor = i + j < codeBars.Count ? codeBars[i + j] : "";
                            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(valor, parrafoItem)) { BorderWidth = 1, Padding = 5 };
                            tbl.AddCell(c1);
                        }
                    }
                        
                    // Agregamos la tabla al documento
                    doc.Add(tbl);


                    //============================
                    //Tabla: 4 (Totales)
                    int totalItems = listCodeBar.Count;
                    decimal totalPeso = listCodeBar.Sum(x => x.U_WeightKg ?? 0);

                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 15f, 2f, 15f, 36f, 15f, 2f, 15f }) { WidthPercentage = 100 };
                    var c1Tot = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Total Items", parrafoNegro)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                    tbl.AddCell(c1Tot);
                    c1Tot = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNegro)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                    tbl.AddCell(c1Tot);
                    c1Tot = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(totalItems.ToString(), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                    tbl.AddCell(c1Tot);
                    c1Tot = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                    tbl.AddCell(c1Tot);
                    c1Tot = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Peso Total", parrafoNegro)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                    tbl.AddCell(c1Tot);
                    c1Tot = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNegro)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                    tbl.AddCell(c1Tot);
                    c1Tot = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"{totalPeso.ToString("#,##0.00", CultureInfo.InvariantCulture)}", parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                    tbl.AddCell(c1Tot);

                    doc.Add(tbl);
                }

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Se generó correctamente el archivos";
                resultTransaccion.data = file;
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


    public class PageEventHelperPicking : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;
        DateTime PrintTime = DateTime.Now;

        #region Properties
        public string Title { get; set; }
        public string DocNum { get; set; }
        public string DocDate { get; set; }
        public string NumeroGuia { get; set; }
        public string Cliente { get; set; }
        public string Contenedor { get; set; }
        #endregion

        // we override the onOpenDocument method
        public override void OnOpenDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            try
            {
                bfTitulo = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA_BOLD, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                bfTexto = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                PrintTime = DateTime.Now;
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch (iTextSharp.text.DocumentException)
            {
            }
            catch (IOException)
            {
            }
        }

        public override void OnStartPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnStartPage(writer, document);
            iTextSharp.text.Rectangle pageSize = document.PageSize;
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(bfTitulo, 11f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);

            if (Title != string.Empty)
            {
                //Titulo
                cb.BeginText();
                cb.SetFontAndSize(bfTitulo, 25);
                cb.SetTextMatrix(pageSize.GetRight(380), pageSize.GetTop(55));
                cb.ShowText(Title);
                cb.EndText();

                //Logo
                var pathLogo = Path.Combine(Environment.CurrentDirectory, "logos", "fibrafil-logo.jpg");

                var logo = iTextSharp.text.Image.GetInstance(pathLogo);

                logo.ScaleToFit(100f, 35f);
                logo.SetAbsolutePosition(document.Left, pageSize.GetTop(55));
                cb.AddImage(logo);


                ////=========================
                /// INICIO: Número SAP
                ////=========================
                if (!string.IsNullOrWhiteSpace(DocNum))
                {
                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetTop(90));
                    cb.ShowText("Número SAP");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(90), pageSize.GetTop(90));
                    cb.ShowText(":");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTexto, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(100), pageSize.GetTop(90));
                    cb.ShowText(DocNum);
                    cb.EndText();
                }

                ////=========================
                /// FIN: Número SAP
                ////=========================


                ////=========================
                /// INICIO: Número guía
                ////=========================
                if (!string.IsNullOrWhiteSpace(NumeroGuia))
                {
                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(200), pageSize.GetTop(90));
                    cb.ShowText("Número guía");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(280), pageSize.GetTop(90));
                    cb.ShowText(":");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTexto, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(290), pageSize.GetTop(90));
                    cb.ShowText(NumeroGuia);
                    cb.EndText();
                }

                ////=========================
                /// FIN: Número guía
                ////=========================


                ////=========================
                /// INICIO: Fecha
                ////=========================
                if (!string.IsNullOrWhiteSpace(DocDate))
                {
                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(380), pageSize.GetTop(90));
                    cb.ShowText("Fecha");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(420), pageSize.GetTop(90));
                    cb.ShowText(":");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTexto, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(430), pageSize.GetTop(90));
                    cb.ShowText(DocDate);
                    cb.EndText();
                }

                ////=========================
                /// FIN: Fecha
                ////=========================




                if (!string.IsNullOrEmpty(Cliente))
                {
                    ////=========================
                    /// INICIO: CLIENTE
                    ////=========================
                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetTop(90));
                    cb.ShowText("Cliente");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(105), pageSize.GetTop(90));
                    cb.ShowText(":");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTexto, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(115), pageSize.GetTop(90));
                    cb.ShowText(Cliente);
                    cb.EndText();
                
                    ////=========================
                    /// FIN: CLIENTE
                    ////=========================


                    ////=========================
                    /// INICIO: CONTENEDOR
                    ////=========================
                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetTop(110));
                    cb.ShowText("Contenedor");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(105), pageSize.GetTop(110));
                    cb.ShowText(":");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTexto, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(115), pageSize.GetTop(110));
                    cb.ShowText(Contenedor);
                    cb.EndText();
                    ////=========================
                    /// FIN: CONTENEDOR
                    ////=========================
                }
            }
        }
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);

            /*
                =====================================================
                Codigo para que el número de página muestre en el pie
                =====================================================
            */
            int pageN = writer.PageNumber;
            string text = "Página " + pageN + "/";
            float len = bfTexto.GetWidthPoint(text, 8);
            iTextSharp.text.Rectangle pageSize = document.PageSize;
            cb.SetRgbColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 8);
            cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();
            cb.AddTemplate(footerTemplate, pageSize.GetLeft(15) + len, pageSize.GetBottom(30));
        }
        public override void OnCloseDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnCloseDocument(writer, document);

            /*
               =====================================================
               Codigo para que el número de página muestre en el pie
               =====================================================
           */
            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bfTexto, 8);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText("" + (writer.PageNumber - 1));
            footerTemplate.EndText();
        }
    }
}
