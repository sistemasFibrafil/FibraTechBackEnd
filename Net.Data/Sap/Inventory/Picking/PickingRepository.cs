using System;
using System.IO;
using AutoMapper;
using SAPbobsCOM;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Globalization;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
namespace Net.Data.Sap
{
    public class PickingRepository : RepositoryBase<PickingEntity>, IPickingRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IMapper _mapper;
        private readonly DataContextSap _dbSap;
        private readonly DataContextProfil _dbProfil;
        private readonly CompanyProviderSap _companyProviderSap;

        public PickingRepository(IConnectionSQL context, IConfiguration configuration, DataContextSap dbSap, DataContextProfil dbProfil, CompanyProviderSap companyProviderSap)
            : base(context)
        {
            _dbSap = dbSap;
            _dbProfil = dbProfil;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }
        
        public async Task<ResultadoTransaccionEntity<PickingQueryEntity>> GetListByFilter(PickingFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PickingQueryEntity>
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
                    U_Quantity = m.Sum(p => p.U_Quantity ?? 0),
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

        public async Task<ResultadoTransaccionEntity<PickingEntity>> GetListByBaseEntry(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PickingEntity>
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
                    U_ItemCode = n.U_ItemCode,
                    U_CodeBar = n.U_CodeBar,
                    U_IsReleased = n.U_IsReleased,
                    U_Quantity = n.U_Quantity,
                    U_WeightKg = n.U_WeightKg
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

        public async Task<ResultadoTransaccionEntity<PickingQueryEntity>> GetListByBaseEntryBaseType(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PickingQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = 
                from p in _dbSap.Picking
                where p.U_Status == "O" && p.U_BaseType == value.U_BaseType && p.U_BaseEntry == value.U_BaseEntry
                join s in _dbSap.SolicitudTraslado1 on new { e = p.U_BaseEntry, l = p.U_BaseLine } equals new { e = (int?)s.DocEntry, l = (int?)s.LineNum } into ps
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

        public async Task<ResultadoTransaccionEntity<PickingEntity>> GetListByTarget(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PickingEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                value.U_CodeBar = value.U_CodeBar?.ToString().Trim() ?? string.Empty;

                var list = await _dbSap.Picking
                .Where(n => n.U_Status == "C" && n.U_TrgetEntry == value.U_TrgetEntry && n.U_TargetType == value.U_TargetType && n.U_TrgetLine == value.U_TrgetLine && n.U_CodeBar.Contains(value.U_CodeBar))
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

        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoQueryEntity>> GetToCopy(PickingCopyToFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _dbSap.SolicitudTraslado
                .Where(n => n.DocStatus == "O" && n.DocEntry == value.U_BaseEntry)
                .Select(n => new SolicitudTrasladoQueryEntity
                {
                    CardCode = n.CardCode,
                    CardName = n.CardName,
                    CntctCode = n.CntctCode,
                    Address = n.Address,
                    Filler = n.Filler,
                    ToWhsCode = n.ToWhsCode,
                    U_FIB_TIP_TRAS = n.U_FIB_TIP_TRAS,
                    U_BPP_MDMT = n.U_BPP_MDMT,
                    U_BPP_MDTS = n.U_BPP_MDTS,
                    SlpCode = n.SlpCode,
                    U_FIB_NBULTOS = n.U_FIB_NBULTOS ?? 0,
                    U_FIB_KG = n.U_FIB_KG ?? 0
                })
                .FirstOrDefaultAsync() ?? throw new Exception("No se encontró la solicitud de transferencia.");


                foreach (var line in value.Lines)
                {
                    var lines = new List<SolicitudTraslado1QueryEntity>();

                    if (line.U_FIB_IsPkg == "Y")
                    {
                        var pickingLines = await _dbSap.Picking
                        .Where
                        (p =>
                            p.U_Status == "O" &&
                            p.U_BaseEntry == line.U_BaseEntry &&
                            p.U_BaseType == line.U_BaseType &&
                            p.U_BaseLine == line.U_BaseLine
                        )
                        .ToListAsync();

                        var solicitudTraslado1 = await _dbSap.SolicitudTraslado1
                        .Select(s => new
                        {
                            s.DocEntry,
                            ObjType = int.Parse(s.ObjType),
                            s.LineNum,
                            s.U_tipoOpT12
                        })
                        .ToListAsync();

                        var tipoOperacion = await _dbSap.TipoOperacion
                        .Select(t => new { t.Code, t.U_descrp })
                        .ToListAsync();


                        lines = (
                        from p in pickingLines
                        join s in solicitudTraslado1 on new { Entry = p.U_BaseEntry ?? 0, Type = p.U_BaseType ?? 0, Line = p.U_BaseLine ?? 0 } equals new { Entry = s.DocEntry, Type = s.ObjType, Line = s.LineNum }
                        join tip in tipoOperacion on s.U_tipoOpT12 equals tip.Code into tipJoin
                        from tip in tipJoin.DefaultIfEmpty()
                        group new { p, s, tip } by new
                        {
                            p.U_BaseEntry,
                            p.U_BaseType,
                            p.U_BaseLine,
                            p.U_ItemCode,
                            p.U_Dscription,
                            p.U_FromWhsCod,
                            p.U_WhsCode,
                            p.U_UnitMsr
                        } into g
                        select new SolicitudTraslado1QueryEntity
                        {
                            BaseEntry = g.Key.U_BaseEntry,
                            BaseType = (int)g.Key.U_BaseType,
                            BaseLine = g.Key.U_BaseLine,
                            ItemCode = g.Key.U_ItemCode,
                            Dscription = g.Key.U_Dscription,
                            FromWhsCod = g.Key.U_FromWhsCod,
                            WhsCode = g.Key.U_WhsCode,
                            U_tipoOpT12 = g.Select(x => x.s.U_tipoOpT12).FirstOrDefault() ?? "",
                            U_tipoOpT12Nam = g.Select(x => x.tip?.U_descrp ?? "").FirstOrDefault(),
                            UnitMsr = string.IsNullOrEmpty(g.Key.U_UnitMsr) ? "KG" : g.Key.U_UnitMsr.ToUpper(),
                            Quantity = g.Sum(x => string.IsNullOrEmpty(x.p.U_UnitMsr) || x.p.U_UnitMsr == "KG" ? x.p.U_WeightKg ?? 0 : x.p.U_Quantity ?? 0),
                            U_FIB_FromPkg = "Y",
                            U_FIB_PesoKg = g.Sum(x => x.p.U_WeightKg ?? 0),
                            U_FIB_NBulto = g.Sum(x => x.p.U_NumBulk ?? 0)
                        }).ToList();

                        foreach (var line1 in pickingLines)
                        {
                            data.PickingLines.Add(line1);
                        }
                    }
                    else
                    {
                        lines = await _dbSap.SolicitudTraslado1
                        .Include(s => s.TipoOperacion)
                        .Where(d => d.LineStatus == "O" && d.OpenQty > 0 && d.DocEntry == line.U_BaseEntry && d.LineNum == line.U_BaseLine)
                        .Select(s => new SolicitudTraslado1QueryEntity
                        {
                            DocEntry = s.DocEntry,
                            LineNum = s.LineNum,
                            ObjType = s.ObjType,
                            BaseType = int.Parse(s.ObjType),
                            BaseEntry = s.DocEntry,
                            BaseLine = s.LineNum,
                            U_FIB_FromPkg = s.U_FIB_FromPkg ?? "N",
                            LineStatus = s.LineStatus,
                            ItemCode = s.ItemCode,
                            Dscription = s.Dscription,
                            FromWhsCod = s.FromWhsCod,
                            WhsCode = s.WhsCode,
                            U_tipoOpT12 = s.U_tipoOpT12 ?? "",
                            U_tipoOpT12Nam = s.TipoOperacion.U_descrp ?? "",
                            UnitMsr = s.UnitMsr,
                            Quantity = s.OpenQty,
                            OpenQty = s.OpenQty,
                            U_FIB_OpQtyPkg = s.U_FIB_OpQtyPkg ?? 0,
                            U_FIB_NBulto = s.U_FIB_NBulto ?? 0,
                            U_FIB_PesoKg = s.U_FIB_PesoKg ?? 0
                        })
                        .ToListAsync();
                    }

                    foreach (var line1 in lines)
                    {
                        data.Lines.Add(line1);
                    }
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<PickingQueryEntity>> SetCreate(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PickingQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            StockTransfer stockTransfer = null;
            GeneralService oGeneralService = null;

            return await Task.Run(() =>
            {
                try
                {
                    if(_dbSap.Picking.Any(n => n.U_Status != "D" && n.U_IsReleased != "Y" && n.U_CodeBar == value.U_CodeBar))
                    {
                        throw new Exception("Este código de barras ya se encuentra registrado.");
                    }

                    if (_dbSap.Picking.Any(n => n.U_Status != "D" && n.U_IsReleased == "Y" && n.U_CodeBar == value.U_CodeBar && n.U_BaseEntry == value.U_BaseEntry ))
                    {
                        throw new Exception("El código ya fue liberado en esta transferencia y no puede volver a ser leído.");
                    }

                    // Se obtiene el código de artículo y el peso del código de barras ingresado
                    var pesaje = _dbProfil.Pesaje
                    .Select(n => new
                    {
                        n.ItemNo,
                        line = n.Pesaje1
                               .Where(m => m.CODEBAR == value.U_CodeBar)
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
                    .FirstOrDefault() ?? throw new Exception("El código de barras ingresado no existe.");


                    // Se valida que el peso del código de barras no sea cero
                    if (pesaje.WeightKg == null || pesaje.WeightKg == 0)
                    {
                        throw new Exception("El peso del código de barras es cero.");
                    }


                    // Se obtiene la cabecera de la solicitud de transferencia relacionada al picking
                    var data = _dbSap.SolicitudTraslado.Where(n => n.DocEntry == value.U_BaseEntry).FirstOrDefault();


                    // Se obtiene la lista de líneas del detalle de la solicitud de transferencia que coincidan con el código de artículo del código de barras ingresado
                    var lines = _dbSap.SolicitudTraslado1.Where(n => n.DocEntry == value.U_BaseEntry && n.ItemCode == pesaje.ItemCode).ToList();


                    // Se valida que el artículo del código relacionado con el código de barras ingresada exista en la solicitud de transferencia
                    if (!lines.Any()) throw new Exception("No se encontró el artículo en la solicitud de transferencia para el código de barras ingresado.");


                    // Se obtiene una línea del detalle de la solicitud de transferencia, pendiente de picking
                    var line = lines
                    .Where(n => n.LineStatus == "O" && n.U_FIB_LinStPkg == "O" && n.U_FIB_OpQtyPkg > 0)
                    .Select(n => new SolicitudTraslado1Entity
                    {
                        ObjType = n.ObjType,
                        DocEntry = n.DocEntry,
                        LineNum = n.LineNum,
                        U_FIB_LinStPkg = n.U_FIB_LinStPkg,
                        ItemCode = n.ItemCode,
                        Dscription = n.Dscription,
                        WhsCode = n.WhsCode,
                        FromWhsCod = value.U_FromWhsCod,
                        UnitMsr = string.IsNullOrWhiteSpace(n.UnitMsr) ? "KG" : n.UnitMsr.ToUpper().Trim(),
                        U_FIB_OpQtyPkg = n.U_FIB_OpQtyPkg,
                    })
                    .FirstOrDefault() ?? throw new Exception("Este artículo no presenta pendientes de picking.");


                    // Se obtiene la suma de stock's del artículo en el almacén de origen
                    var sumStock = _dbSap.ItemWarehouseInfo.Where(n => n.ItemCode == line.ItemCode && n.WhsCode == line.FromWhsCod).Sum(n => n.OnHand);


                    // Se obtiene la suma de pesos del picking del artículo en el almacén de origen
                    var sumWeight = _dbSap.Picking.Where(n => n.U_Status == "O" && n.U_ItemCode == line.ItemCode && n.U_FromWhsCod == line.FromWhsCod).Sum(n => n.U_WeightKg);


                    // Se obtiene la suma de cantidades del picking del artículo en el almacén de origen
                    var sumQuantity = _dbSap.Picking.Where(n => n.U_Status == "O" && n.U_ItemCode == line.ItemCode && n.U_FromWhsCod == line.FromWhsCod).Sum(n => n.U_Quantity);


                    // Se calcula el stock disponible del artículo en el almacén de origen, dependiendo de la unidad de medida, con los datos del picking ya registrado
                    var stock = line.UnitMsr == "KG" ? ((double)sumStock - (double)sumWeight) : ((double)sumStock - (double)sumQuantity);


                    // Se valida que exista stock disponible en el almacén de origen para el artículo
                    if (stock <= 0) throw new Exception("No hay stock disponible para el artículo en el almacén de origen " + line.FromWhsCod + ".");


                    // Se obtiene la cantidad por paquete del artículo. Sino tiene cantidad por paquete, se lanza una excepción
                    var quantity = _dbSap.Items
                    .Where(n => n.ItemCode == line.ItemCode)
                    .Select(n => (decimal?)n.SalPackUn).FirstOrDefault() ?? throw new Exception("Define la cantidad por paquete en SAP Business One, en los Datos maestros de artículo, pestaña Datos de ventas.");


                    // Volveremos a validar el stock con el peso o cantidad del código de barras
                    if (line.UnitMsr == "KG" && stock - (double)(pesaje.WeightKg ?? 0) < 0)
                    {
                        throw new Exception("No hay stock disponible para el artículo en el almacén de origen " + line.FromWhsCod + ".");
                    }
                    else if (line.UnitMsr != "KG" && stock - (double)quantity < 0)
                    {
                        throw new Exception("No hay stock disponible para el artículo en el almacén de origen " + line.FromWhsCod + ".");
                    }


                    // Validamos que la cantidad a leer no supere la cantidad pendiente por leer en la línea de la solicitud de transferencia
                    if (line.UnitMsr == "KG" && (line.U_FIB_OpQtyPkg ?? 0) - pesaje.WeightKg < 0)
                    {
                        throw new Exception("El peso del artículo excede el peso pendiente en el picking.");
                    }
                    else if (line.UnitMsr != "KG" && line.U_FIB_OpQtyPkg - quantity < 0)
                    {
                        throw new Exception("La cantidad del artículo excede la cantidad pendiente en el picking.");
                    }


                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Empieza la transacción en SAP
                    if (!company.InTransaction) company.StartTransaction();


                    // Se instancia el servicio de objetos generales
                    oCompService = company.GetCompanyService();
                    // se instancia el objeto de datos generales
                    oGeneralService = oCompService.GetGeneralService("FIB_OPKG");
                    // se instancia el objeto de datos generales
                    oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);


                    // Se asignan los valores a las propiedades del objeto de datos generales
                    oGeneralData.SetProperty("U_BaseEntry", data.DocEntry);
                    oGeneralData.SetProperty("U_BaseNum", data.DocNum);
                    oGeneralData.SetProperty("U_BaseType", data.ObjType);
                    oGeneralData.SetProperty("U_BaseLine", line.LineNum);
                    oGeneralData.SetProperty("U_DocDate", data.DocDate);
                    oGeneralData.SetProperty("U_TaxDate", data.TaxDate);
                    oGeneralData.SetProperty("U_DocDueDate", data.DocDueDate);
                    if (data.CardCode != null) oGeneralData.SetProperty("U_CardCode", data.CardCode);
                    if (data.CardName != null) oGeneralData.SetProperty("U_CardName", data.CardName);
                    if (line.ItemCode != null) oGeneralData.SetProperty("U_ItemCode", line.ItemCode);
                    if (line.Dscription != null) oGeneralData.SetProperty("U_Dscription", line.Dscription);
                    if (pesaje.CodeBar != null) oGeneralData.SetProperty("U_CodeBar", pesaje.CodeBar);
                    if (line.FromWhsCod != null) oGeneralData.SetProperty("U_FromWhsCod", line.FromWhsCod);
                    if (line.WhsCode != null) oGeneralData.SetProperty("U_WhsCode", line.WhsCode);
                    oGeneralData.SetProperty("U_UnitMsr", line.UnitMsr);
                    oGeneralData.SetProperty("U_Quantity", Convert.ToDouble(Math.Round(Convert.ToDouble(quantity), 6)));
                    oGeneralData.SetProperty("U_WeightKg", Convert.ToDouble(Math.Round(Convert.ToDouble(pesaje.WeightKg), 6)));
                    oGeneralData.SetProperty("U_NumBulk", Convert.ToDouble(Math.Round(Convert.ToDouble(1), 6)));
                    if (value.U_UsrCreate != null) oGeneralData.SetProperty("U_UsrCreate", value.U_UsrCreate);
                    // Se crea el registro
                    oGeneralService.Add(oGeneralData);


                    // Se instancia el objeto de la solicitud transferencia
                    stockTransfer = company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);


                    // Se obtiene la solicitud de transferencia. Sino existe, se lanza una excepción
                    if (!stockTransfer.GetByKey(line.DocEntry)) throw new Exception("No existe la solictud de transferencia.");


                    //// Se valida que la solicitud de transferencia tenga líneas en el detalle
                    if (stockTransfer.Lines.Count == 0) throw new Exception("No existe el detalle de la solictud de transferencia.");


                    // Se restan los pesos o las cantidades leídas en el picking a la cantidad pendiente por leer en la línea de la solicitud de transferencia
                    var opQtyPicking = line.U_FIB_OpQtyPkg - (line.UnitMsr == "KG" ? pesaje.WeightKg : quantity);


                    // Se actualiza la línea de la solicitud de transferencia
                    if (opQtyPicking != null)
                    {
                        for (int i = 0; i < stockTransfer.Lines.Count; i++)
                        {
                            stockTransfer.Lines.SetCurrentLine(i);
                            if (stockTransfer.Lines.LineNum == line.LineNum)
                            {
                                stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = Convert.ToDouble(Math.Round(Convert.ToDecimal(opQtyPicking), 6));
                                stockTransfer.Update();
                                break;
                            }
                        }
                    }


                    // Se recorre las líneas de la solicitud de transferencia para actualizar el estado de picking de la línea a "C" (completada), si la cantidad pendiente por picar es cero
                    for (int i = 0; i < stockTransfer.Lines.Count; i++)
                    {
                        stockTransfer.Lines.SetCurrentLine(i);
                        if (stockTransfer.Lines.LineNum == line.LineNum)
                        {
                            var opQtyPkg = Convert.ToDouble(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0);
                            if (opQtyPkg == 0)
                            {
                                stockTransfer.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = "C";
                                stockTransfer.Update();
                            }
                            break;
                        }
                    }


                    // Variable para contar las líneas que aún están abiertas y con cantidad pendiente por picar
                    int openPkg = 0;


                    // Se recorre las líneas de la solicitud de transferencia para contar las líneas que aún están abiertas y con cantidad pendiente por picar
                    for (int i = 0; i < stockTransfer.Lines.Count; i++)
                    {
                        stockTransfer.Lines.SetCurrentLine(i);
                        if (stockTransfer.Lines.LineNum == line.LineNum)
                        {
                            if (Convert.ToString(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value ?? "O") == "O" && Convert.ToDouble(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0) > 0)
                            {
                                openPkg++;
                                break;
                            }
                        }
                    }


                    if (openPkg == 0)
                    {
                        stockTransfer.UserFields.Fields.Item("U_FIB_DocStPkg").Value = "C";
                        stockTransfer.Update();
                    }

                    // Se finaliza la transacción en SAP
                    if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_Commit);


                    // Se consulta la lista de picking registrados en la base de datos
                    var query = 
                    from p in _dbSap.Picking
                    where p.U_Status == "O" && p.U_BaseType == value.U_BaseType && p.U_BaseEntry == value.U_BaseEntry
                    join s in _dbSap.SolicitudTraslado1 on new { e = p.U_BaseEntry, l = p.U_BaseLine } equals new { e = (int?)s.DocEntry, l = (int?)s.LineNum } into ps
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

                    var list = query.OrderBy(n => n.U_BaseLine).ToList();

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.dataList = list;
                    resultTransaccion.ResultadoDescripcion = "Realizado con éxito.";
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
                    _companyProviderSap.LiberarObjetosCOM(oGeneralData, oCompService, oGeneralService, stockTransfer);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionEntity<PickingEntity>> SetRelease(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PickingEntity>
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

        public async Task<ResultadoTransaccionEntity<PickingEntity>> SetDelete(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PickingEntity>
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


                    // Se instancia el objeto de la solicitud transferencia
                    stockTransfer = company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);


                    // Se obtiene la solicitud de transferencia. Sino existe, se lanza una excepción
                    if (!stockTransfer.GetByKey(line.U_BaseEntry?? 0)) throw new Exception("No existe la solictud de transferencia.");


                    //// Se valida que la solicitud de transferencia tenga líneas en el detalle
                    if (stockTransfer.Lines.Count == 0) throw new Exception("No existe el detalle de la solictud de transferencia.");


                    // Variable para contar las líneas que aún están abiertas y con cantidad pendiente por picar
                    decimal opQtyPkg = 0;


                    // Se obtiene la cantidad por paquete de la línea de la solicitud de transferencia
                    for (int i = 0; i < stockTransfer.Lines.Count; i++)
                    {
                        stockTransfer.Lines.SetCurrentLine(i);
                        if (stockTransfer.Lines.LineNum == line.U_BaseLine)
                        {
                            opQtyPkg = Convert.ToDecimal(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0);
                            break;
                        }
                    }


                    // Se seuma los pesos o las cantidades leídas en el picking a la cantidad pendiente por leer en la línea de la solicitud de transferencia
                    var opQtyPicking = opQtyPkg + (line.U_UnitMsr == "KG" ? line.U_WeightKg : line.U_Quantity);


                    // Se actualiza la línea de la solicitud de transferencia
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


                    // Se recorre las líneas de la solicitud de transferencia para actualizar el estado de picking de la línea a "C" (completada), si la cantidad pendiente por picar es cero
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


                    // Se declara la variable para contar las líneas abiertas y con cantidad pendiente por picar en la solicitud de transferencia
                    int OpenPkg = 0;


                    // Se recorre las líneas de la solicitud de transferencia para contar las líneas que aún están abiertas y con cantidad pendiente por picar
                    for (int i = 0; i < stockTransfer.Lines.Count; i++)
                    {
                        stockTransfer.Lines.SetCurrentLine(i);
                        if (Convert.ToString(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value ?? "O") == "O" && Convert.ToDouble(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0) > 0)
                        {
                            OpenPkg++;
                            break;
                        }
                    }


                    // Si hay líneas abiertas y con cantidad pendiente por picar, se actualiza el estado de picking de la solicitud de transferencia a "O" (Pendiente)
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

        public async Task<ResultadoTransaccionEntity<PickingEntity>> SetDeleteMassive(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PickingEntity>
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


                        // Se instancia el objeto de la solicitud transferencia
                        stockTransfer = company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);


                        // Se obtiene la solicitud de transferencia. Sino existe, se lanza una excepción
                        if (!stockTransfer.GetByKey(line.U_BaseEntry ?? 0)) throw new Exception("No existe la solictud de transferencia.");


                        //// Se valida que la solicitud de transferencia tenga líneas en el detalle
                        if (stockTransfer.Lines.Count == 0) throw new Exception("No existe el detalle de la solictud de transferencia.");


                        // Variable para contar las líneas que aún están abiertas y con cantidad pendiente por picar
                        decimal opQtyPkg = 0;


                        // Se obtiene la cantidad por paquete de la línea de la solicitud de transferencia
                        for (int i = 0; i < stockTransfer.Lines.Count; i++)
                        {
                            stockTransfer.Lines.SetCurrentLine(i);
                            if (stockTransfer.Lines.LineNum == line.U_BaseLine)
                            {
                                opQtyPkg = Convert.ToDecimal(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0);
                                break;
                            }
                        }


                        // Se seuma los pesos o las cantidades leídas en el picking a la cantidad pendiente por leer en la línea de la solicitud de transferencia
                        var opQtyPicking = opQtyPkg + (line.U_UnitMsr == "KG" ? line.U_WeightKg : line.U_Quantity);


                        // Se actualiza la línea de la solicitud de transferencia
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


                        // Se recorre las líneas de la solicitud de transferencia para actualizar el estado de picking de la línea a "C" (completada), si la cantidad pendiente por picar es cero
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


                        // Se declara la variable para contar las líneas abiertas y con cantidad pendiente por picar en la solicitud de transferencia
                        int OpenPkg = 0;


                        // Se recorre las líneas de la solicitud de transferencia para contar las líneas que aún están abiertas y con cantidad pendiente por picar
                        for (int i = 0; i < stockTransfer.Lines.Count; i++)
                        {
                            stockTransfer.Lines.SetCurrentLine(i);
                            if (Convert.ToString(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value ?? "O") == "O" && Convert.ToDouble(stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value ?? 0) > 0)
                            {
                                OpenPkg++;
                                break;
                            }
                        }


                        // Si hay líneas abiertas y con cantidad pendiente por picar, se actualiza el estado de picking de la solicitud de transferencia a "O" (Pendiente)
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

        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetPickingPrint(PickingEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await
                (
                    from p in _dbSap.Picking
                    join t in _dbSap.TransferenciaStock on new { DocEntry = p.U_TrgetEntry, ObjType = p.U_TargetType.ToString() } equals new { DocEntry = (int?)t.DocEntry, t.ObjType }
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
                            string valor = (i + j < codeBars.Count) ? codeBars[i + j] : "";
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
                var pathLogo = System.IO.Path.Combine(Environment.CurrentDirectory, "logos", "fibrafil-logo.jpg");

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
