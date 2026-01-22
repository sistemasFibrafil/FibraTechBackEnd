using System;
using System.IO;
using SAPbobsCOM;
using System.Linq;
using System.Data;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
namespace Net.Data.Sap
{
    public class SolicitudTrasladoRepository : RepositoryBase<SolicitudTrasladoEntity>, ISolicitudTrasladoRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly DataContextSap _db;
        private readonly CompanyProviderSap _companyProviderSap;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_SOLICITUDTRASLADO_BY_DOCENTRY = DB_ESQUEMA + "INV_GetSolicitudTrasladoByDocEntry";
        const string SP_GET_LIST_SOLICITUDTRASLADO_DETALLE_BY_DOCENTRY = DB_ESQUEMA + "INV_GetListSolicitudTrasladoDetalleByDocEntry";

        public SolicitudTrasladoRepository(IConnectionSQL context, IConfiguration configuration, DataContextSap db, CompanyProviderSap companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }

        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> GetListOpen()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.SolicitudTraslado
                .Where(n => n.DocStatus == "O" && n.U_FIB_DocStPkg == "O" && n.U_FIB_IsPkg == "Y")
                .Select(s => new SolicitudTrasladoEntity
                {
                    DocEntry = s.DocEntry,
                    DocNum = s.DocNum,
                    Filler = s.Filler
                })
                .OrderByDescending(n=>n.DocEntry)
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
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> GetListByFilter(SolicitudTrasladoFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.SolicitudTraslado
                .AsNoTracking()
                .Where(x => x.DocDate >= value.StartDate && x.DocDate <= value.EndDate);

                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.DocNum.ToString(), $"%{filter}%")
                    );
                }

                if (!string.IsNullOrWhiteSpace(value.DocStatus))
                {
                    var docStatus = value.DocStatus.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(x => docStatus.Contains(x.DocStatus));
                }

                var list = await query
                .Select(n => new SolicitudTrasladoEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    DocStatus = n.DocStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    U_FIB_IsPkg = n.U_FIB_IsPkg,
                    Filler = n.Filler,
                    ToWhsCode = n.ToWhsCode,

                })
                .OrderByDescending(x => x.DocEntry).ToListAsync();

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
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoQueryEntity>> GetByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.SolicitudTraslado
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new SolicitudTrasladoQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    DocStatus = n.DocStatus,
                    U_FIB_DocStPkg = n.U_FIB_DocStPkg,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    U_FIB_IsPkg = n.U_FIB_IsPkg,
                    CardCode = n.CardCode,
                    CardName = n.CardName,
                    CntctCode = n.CntctCode,
                    Address = n.Address,
                    Filler = n.Filler,
                    ToWhsCode = n.ToWhsCode,
                    U_FIB_TIP_TRAS = n.U_FIB_TIP_TRAS,
                    U_BPP_MDMT = n.U_BPP_MDMT,
                    U_BPP_MDTS = n.U_BPP_MDTS,
                    U_FIB_NBULTOS = n.U_FIB_NBULTOS ?? 0,
                    U_FIB_KG = n.U_FIB_KG ?? 0,
                    SlpCode = n.SlpCode,
                    JrnlMemo = n.JrnlMemo,
                    Comments = n.Comments,

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines.Select(s => new SolicitudTraslado1QueryEntity
                    {
                        DocEntry = s.DocEntry,
                        LineNum = s.LineNum,
                        ObjType = s.ObjType,
                        BaseType = s.BaseType,
                        BaseEntry = s.BaseEntry,
                        BaseLine = s.BaseLine,
                        U_FIB_FromPkg = s.U_FIB_FromPkg ?? "N",
                        LineStatus = s.LineStatus,
                        ItemCode = s.ItemCode,
                        Dscription = s.Dscription,
                        FromWhsCod = s.FromWhsCod,
                        WhsCode = s.WhsCode,
                        U_tipoOpT12 = s.U_tipoOpT12 ?? "",
                        U_tipoOpT12Nam = s.TipoOperacion != null ? s.TipoOperacion.U_descrp : "",
                        UnitMsr = s.UnitMsr,
                        Quantity = s.Quantity,
                        OpenQty = s.OpenQty,
                        U_FIB_OpQtyPkg = s.U_FIB_OpQtyPkg ?? 0,
                        U_FIB_NBulto = s.U_FIB_NBulto ?? 0,
                        U_FIB_PesoKg = s.U_FIB_PesoKg ?? 0
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
        
        /// <summary>
        /// Se obtiene la solicitud de traslado para ser convertida en transferencia de stock
        /// </summary>
        /// <param name="docEntry"></param>
        /// <returns></returns>
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoQueryEntity>> GetToTransferenciaByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.SolicitudTraslado
                .Where(x => x.DocEntry == docEntry)
                .Select(s => new SolicitudTrasladoQueryEntity
                {
                    CardCode = s.CardCode,
                    CardName = s.CardName,
                    CntctCode = s.CntctCode,
                    Address = s.Address,
                    Filler = s.Filler,
                    ToWhsCode = s.ToWhsCode,
                    U_FIB_TIP_TRAS = s.U_FIB_TIP_TRAS ?? "",
                    U_BPP_MDMT = s.U_BPP_MDMT ?? "",
                    U_BPP_MDTS = s.U_BPP_MDTS ?? "",
                    SlpCode = s.SlpCode,
                    U_FIB_NBULTOS   = 0,
                    U_FIB_KG   = 0,
                })
                .FirstOrDefaultAsync();

                if (data == null)
                {
                    throw new Exception("No se encontró el registro.");
                }

                var lines = await _db.SolicitudTraslado1
                .Include(s => s.TipoOperacion)
                .Where(x => x.DocEntry == docEntry)
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
                    Quantity = s.Quantity,
                    OpenQty = s.OpenQty,
                    U_FIB_OpQtyPkg = s.U_FIB_OpQtyPkg ?? 0,
                    U_FIB_NBulto = s.U_FIB_NBulto ?? 0,
                    U_FIB_PesoKg = s.U_FIB_PesoKg ?? 0
                })
                .ToListAsync();

                data.Lines = lines;

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


        /// <summary>
        /// En el FrontEnd muestra una ventana con los suministros indirectos para ser seleccionados
        /// </summary>
        /// <returns></returns>
        public async Task<ResultadoTransaccionEntity<PickingQueryEntity>> GetListNotPicking()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PickingQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await (
                    from hea in _db.SolicitudTraslado
                    join det in _db.SolicitudTraslado1 on hea.DocEntry equals det.DocEntry
                    // LEFT JOIN
                    from tip in _db.TipoOperacion.Where(t => t.Code == det.U_tipoOpT12).DefaultIfEmpty()
                    where hea.DocStatus == "O" && det.LineStatus == "O"
                    select new PickingQueryEntity
                    {
                        U_Status = hea.DocStatus,
                        U_BaseEntry = hea.DocEntry,
                        U_BaseNum = hea.DocNum,
                        U_BaseType = int.Parse(hea.ObjType),
                        U_DocDate = hea.DocDate,
                        U_DocDueDate = hea.DocDueDate,
                        U_BaseLine = det.LineNum,
                        U_ItemCode = det.ItemCode,
                        U_Dscription = det.Dscription,
                        U_FromWhsCod = det.FromWhsCod,
                        U_WhsCode = det.WhsCode,
                        U_tipoOpT12 = det.U_tipoOpT12 ?? "",
                        U_tipoOpT12Nam = tip != null ? tip.U_descrp : "",
                        U_UnitMsr = det.UnitMsr,
                        U_FIB_IsPkg = "N",
                        U_Quantity = det.OpenQty,
                        U_WeightKg = det.U_FIB_PesoKg ?? 0,
                        U_NumBulk = det.U_FIB_NBulto ?? 0,
                    })
                    .OrderByDescending(n => n.U_BaseEntry)
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

        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> SetCreate(SolicitudTrasladoCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            StockTransfer stockTransfer = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    stockTransfer = company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);

                    #region <<< CABECERA >>>

                    stockTransfer.DocDate = value.DocDate;
                    stockTransfer.DueDate = value.DocDueDate;
                    stockTransfer.TaxDate = value.TaxDate;
                    stockTransfer.DocObjectCode = BoObjectTypes.oInventoryTransferRequest;
                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    if (value.CardCode != null) stockTransfer.CardCode = value.CardCode;
                    if (value.CardName != null) stockTransfer.CardName = value.CardName;
                    if (value.CardCode != null) stockTransfer.ContactPerson = value.CntctCode ?? 0;
                    if (value.Address != null) stockTransfer.Address = value.Address;
                    // ===========================================================================================
                    // ALMACEN
                    // ===========================================================================================
                    stockTransfer.FromWarehouse = value.Filler;
                    stockTransfer.ToWarehouse = value.ToWhsCode;
                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    if (value.U_FIB_IsPkg != null) stockTransfer.UserFields.Fields.Item("U_FIB_IsPkg").Value = value.U_FIB_IsPkg;
                    if (value.U_FIB_TIP_TRAS != null) stockTransfer.UserFields.Fields.Item("U_FIB_TIP_TRAS").Value = value.U_FIB_TIP_TRAS;
                    if (value.U_BPP_MDMT != null) stockTransfer.UserFields.Fields.Item("U_BPP_MDMT").Value = value.U_BPP_MDMT;
                    if (value.U_BPP_MDTS != null) stockTransfer.UserFields.Fields.Item("U_BPP_MDTS").Value = value.U_BPP_MDTS;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    stockTransfer.SalesPersonCode = value.SlpCode == 0 ? -1 : value.SlpCode ?? -1;
                    stockTransfer.JournalMemo = value.JrnlMemo;
                    stockTransfer.Comments = value.Comments;

                    if (value.U_UsrCreate != null) stockTransfer.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                    #endregion

                    #region <<< DETALLE >>>

                    foreach (var line in value.Lines)
                    {
                        stockTransfer.Lines.ItemCode = line.ItemCode;
                        stockTransfer.Lines.ItemDescription = line.Dscription;
                        stockTransfer.Lines.FromWarehouseCode = line.FromWhsCod;
                        stockTransfer.Lines.WarehouseCode = line.WhsCode;
                        stockTransfer.Lines.Quantity = (double)line.Quantity;
                        if (line.U_tipoOpT12 != null || line.U_tipoOpT12 == "") stockTransfer.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        if (line.U_FIB_OpQtyPkg != null) stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = Convert.ToDouble(Math.Round(Convert.ToDecimal(line.U_FIB_OpQtyPkg ?? 0), 6));
                        stockTransfer.Lines.Add();
                    }

                    #endregion

                    var reg = stockTransfer.Add();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "La solictud de transferencia registrada con éxito.";
                    }
                    else
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Código: {errorCode}. Mensaje: {errorMessage}.");
                    }
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(stockTransfer);
                }

                return resultTransaccion;
            });
        }
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> SetUpdate(SolicitudTrasladoUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            StockTransfer stockTransfer = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    stockTransfer = company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);

                    if (!stockTransfer.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la solictud de transferencia.");
                    }

                    #region <<< CABECERA >>>
                    stockTransfer.DocDate = value.DocDate;
                    stockTransfer.DueDate = value.DocDueDate;
                    stockTransfer.TaxDate = value.TaxDate;
                    stockTransfer.DocObjectCode = BoObjectTypes.oInventoryTransferRequest;
                    // ===========================================================================================
                    // ALMACEN
                    // ===========================================================================================
                    stockTransfer.FromWarehouse = value.Filler;
                    stockTransfer.ToWarehouse = value.ToWhsCode;
                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    if (value.U_FIB_IsPkg != null) stockTransfer.UserFields.Fields.Item("U_FIB_IsPkg").Value = value.U_FIB_IsPkg;
                    if (value.U_FIB_TIP_TRAS != null) stockTransfer.UserFields.Fields.Item("U_FIB_TIP_TRAS").Value = value.U_FIB_TIP_TRAS;
                    if (value.U_BPP_MDMT != null) stockTransfer.UserFields.Fields.Item("U_BPP_MDMT").Value = value.U_BPP_MDMT;
                    if (value.U_BPP_MDTS != null) stockTransfer.UserFields.Fields.Item("U_BPP_MDTS").Value = value.U_BPP_MDTS;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    stockTransfer.SalesPersonCode = value.SlpCode == 0 ? -1 : value.SlpCode ?? -1;
                    stockTransfer.JournalMemo = value.JrnlMemo;
                    stockTransfer.Comments = value.Comments;

                    if (value.U_UsrUpdate != null) stockTransfer.UserFields.Fields.Item("U_UsrUpdate").Value = value.U_UsrUpdate;

                    #endregion

                    #region <<< DETALLE >>>

                    // NUEVO: SE AGREGA LINEA
                    foreach (var line in value.Lines.Where(x => x.Record == 1))
                    {
                        stockTransfer.Lines.Add();
                        stockTransfer.Lines.ItemCode = line.ItemCode;
                        stockTransfer.Lines.ItemDescription = line.Dscription;
                        stockTransfer.Lines.FromWarehouseCode = line.FromWhsCod;
                        stockTransfer.Lines.WarehouseCode = line.WhsCode;
                        stockTransfer.Lines.Quantity = (double)line.Quantity;
                        if (line.U_tipoOpT12 != null || line.U_tipoOpT12 == "") stockTransfer.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        if (line.U_FIB_OpQtyPkg != null) stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = Convert.ToDouble(Math.Round(Convert.ToDecimal(line.U_FIB_OpQtyPkg), 6));
                    }

                    // EXISTE: SE MODIFICA LINEA
                    foreach (var line in value.Lines.Where(x => x.Record == 2 && x.LineStatus == "O"))
                    {
                        for (int i = 0; i < stockTransfer.Lines.Count; i++)
                        {
                            stockTransfer.Lines.SetCurrentLine(i);
                            if (stockTransfer.Lines.LineNum == line.LineNum)
                            {
                                stockTransfer.Lines.ItemCode = line.ItemCode;
                                stockTransfer.Lines.ItemDescription = line.Dscription;
                                stockTransfer.Lines.FromWarehouseCode = line.FromWhsCod;
                                stockTransfer.Lines.WarehouseCode = line.WhsCode;
                                stockTransfer.Lines.Quantity = (double)line.Quantity;
                                if (line.U_tipoOpT12 != null || line.U_tipoOpT12 == "") stockTransfer.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                                if (line.U_FIB_OpQtyPkg != null) stockTransfer.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = Convert.ToDouble(Math.Round(Convert.ToDecimal(line.U_FIB_OpQtyPkg), 6));
                            }
                        }
                    }

                    // EXISTE: SE ELIMINA LINEA
                    foreach (var line in value.Lines.Where(x => x.Record == 3))
                    {
                        for (int i = 0; i < stockTransfer.Lines.Count; i++)
                        {
                            stockTransfer.Lines.SetCurrentLine(i);
                            if (stockTransfer.Lines.LineNum == line.LineNum)
                            {
                                stockTransfer.Lines.Delete();
                            }
                        }
                    }

                    #endregion

                    var reg = stockTransfer.Update();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "La solictud de transferencia actualizada con éxito.";
                    }
                    else
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                    }
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(stockTransfer);
                }

                return resultTransaccion;
            });
        }
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> SetClose(SolicitudTrasladoCloseEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            StockTransfer stockTransfer = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();

                    stockTransfer = company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);
                    
                    if (!stockTransfer.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la solictud de transferencia.");
                    }

                    stockTransfer.UserFields.Fields.Item("U_UsrUpdate").Value = value.U_UsrUpdate;

                    var regUpdate = stockTransfer.Update();

                    if (regUpdate == 0)
                    {
                        var regClose = stockTransfer.Close();

                        if (regClose == 0)
                        {
                            resultTransaccion.IdRegistro = 0;
                            resultTransaccion.ResultadoCodigo = 0;
                            resultTransaccion.ResultadoDescripcion = "La solictud de transferencia cerrada con éxito ..!";
                        }
                        else
                        {
                            company.GetLastError(out int errorCode, out string errorMessage);
                            throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                        }
                    }
                    else
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                    }
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                finally
                {
                    _companyProviderSap.LiberarObjetosCOM(stockTransfer);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetFormatoPdfByDocEntry(int id)
        {
            var header = new SolicitudTrasladoFormatoEntity();
            var linea = new List<SolicitudTraslado1FormatoEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_SOLICITUDTRASLADO_BY_DOCENTRY, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@DocEntry", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            header = context.Convert<SolicitudTrasladoFormatoEntity>(reader);
                        }
                    }

                    iTextSharp.text.Document doc = new iTextSharp.text.Document();
                    doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                    doc.SetMargins(10f, 10f, 70f, 10f);
                    MemoryStream ms = new MemoryStream();
                    iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                    write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                    // Our custom Header and Footer is done using Event Handler
                    var pageEventHelperSolicitudTraslado = new PageEventHelperSolicitudTraslado();
                    write.PageEvent = pageEventHelperSolicitudTraslado;

                    // Colocamos la fuente que deseamos que tenga el documento
                    iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
                    // Titulo
                    iTextSharp.text.Font parrafoLinea = new iTextSharp.text.Font(helvetica, 5f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoHerderNegrita = new iTextSharp.text.Font(helvetica, 8.5f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoHeaderDeatailNegrita = new iTextSharp.text.Font(helvetica, 7f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoDetail = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoNegrita = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

                    // Define the page header
                    pageEventHelperSolicitudTraslado.Title = header.Title;
                    pageEventHelperSolicitudTraslado.SubTitle = header.SubTitle;
                    pageEventHelperSolicitudTraslado.Codigo = header.Codigo;
                    pageEventHelperSolicitudTraslado.Version = header.Version;
                    pageEventHelperSolicitudTraslado.Vigencia = header.Vigencia;

                    doc.Open();


                    //============================
                    //TABLA: 1
                    var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 20f, 20f, 20f, 20f, 20f }) { WidthPercentage = 100 };
                    //COLUMNAS
                    var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Fecha: " + header.TaxDate.ToString("dd/MM/yyyy"), parrafoHerderNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 12;
                    c1.PaddingBottom = 20;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Sede Origen: " + header.SedeOrigen, parrafoHerderNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 12;
                    c1.PaddingBottom = 20;
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Sede Destino: " + header.SedeDestino, parrafoHerderNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 12;
                    c1.PaddingBottom = 20;
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Tipo: " + header.TipoTraslado, parrafoHerderNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 12;
                    c1.PaddingBottom = 20;
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("# SAP: " + header.DocNum.ToString(), parrafoHerderNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 12;
                    c1.PaddingBottom = 20;
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    doc.Add(tbl);


                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_SOLICITUDTRASLADO_DETALLE_BY_DOCENTRY, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@DocEntry", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            linea = (List<SolicitudTraslado1FormatoEntity>)context.ConvertTo<SolicitudTraslado1FormatoEntity>(reader);
                        }
                    }

                    //============================
                    //TABLA: 2 - Cabecera del deatalle
                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 16f, 67f, 6f, 6f, 5f }) { WidthPercentage = 100 };
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ITEM CODE", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ITEM NAME", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ALMACEN O.", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ALMACEN D.", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("CANTIDAD", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    foreach (var item in linea)
                    {
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.ItemCode, parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.ItemName, parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.FromWhsCod, parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.WhsCode, parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.Quantity.ToString("N2"), parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                        tbl.AddCell(c1);
                    }

                    doc.Add(tbl);

                    doc.Add(new iTextSharp.text.Phrase("\n\n", parrafoLinea));

                    //TABLA: 3 - Observaciones
                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 7f, 93f }) { WidthPercentage = 100 };
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Observaciones: ", parrafoNegrita));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header.Comments, parrafoNormal));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);

                    doc.Add(tbl);

                    doc.Add(new iTextSharp.text.Phrase("\n", parrafoLinea));

                    //TABLA: 4 - Datos de los responsables
                    // LINEA: 1
                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 12f, 1f, 35f, 4f, 12f, 1f, 35f }) { WidthPercentage = 100 };
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Entregado por", parrafoNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Recibido por", parrafoNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    // LINEA: 2
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Nombres y Apellidos", parrafoNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Nombres y Apellidos", parrafoNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);

                    doc.Add(tbl);

                    doc.Add(new iTextSharp.text.Phrase("\n\n", parrafoLinea));

                    //TABLA: 5 - Firmas
                    // LINEA: 1
                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 12f, 1f, 35f, 4f, 12f, 1f, 35f }) { WidthPercentage = 100 };
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Firma", parrafoNegrita));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Firma", parrafoNegrita));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    tbl.AddCell(c1);

                    doc.Add(tbl);


                    write.Close();
                    doc.Close();
                    ms.Seek(0, SeekOrigin.Begin);
                    var file = ms;

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Se generó correctamente el archivo.s";
                    resultTransaccion.data = file;
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
    }



    public class PageEventHelperSolicitudTraslado : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate headerTemplate, footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;
        DateTime PrintTime = DateTime.Now;

        #region Properties
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Codigo { get; set; }
        public string Version { get; set; }
        public string Vigencia { get; set; }
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
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(100, 100);
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
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(bfTitulo, 7f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoTitulo = new iTextSharp.text.Font(bfTitulo, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.White);
            iTextSharp.text.Font parrafoSubTitulo = new iTextSharp.text.Font(bfTitulo, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

            if (Title != string.Empty)
            {
                //Logo
                var pathLogo = Path.Combine(Environment.CurrentDirectory, "logos", "fibrafil-logo.jpg");
                var logo = iTextSharp.text.Image.GetInstance(pathLogo);
                logo.ScaleToFit(100f, 50f);
                logo.SetAbsolutePosition(pageSize.GetLeft(12), pageSize.GetTop(65));
                cb.AddImage(logo);

                // Código
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetTop(22));
                cb.ShowText("Código");
                cb.EndText();
                // :
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(65), pageSize.GetTop(22));
                cb.ShowText(":");
                cb.EndText();
                //: Valor de código
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(60), pageSize.GetTop(22));
                cb.ShowText(Codigo);
                cb.EndText();

                // Versión
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetTop(35));
                cb.ShowText("Versión");
                cb.EndText();
                // :
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(65), pageSize.GetTop(35));
                cb.ShowText(":");
                cb.EndText();
                //: Valor de versión
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(60), pageSize.GetTop(35));
                cb.ShowText(Version);
                cb.EndText();

                // Vigencia
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetTop(48));
                cb.ShowText("Vigencia");
                cb.EndText();
                // :
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(65), pageSize.GetTop(48));
                cb.ShowText(":");
                cb.EndText();
                //: Valor de vigencia
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(60), pageSize.GetTop(48));
                cb.ShowText(Vigencia);
                cb.EndText();

                // Página
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetTop(61));
                cb.ShowText("Página");
                cb.EndText();
                // :
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(65), pageSize.GetTop(61));
                cb.ShowText(":");
                cb.EndText();
                //: Valor de página
                int pageN = writer.PageNumber;
                string text = "" + pageN + " de ";
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(60), pageSize.GetTop(61));
                cb.ShowText(text);
                cb.EndText();

                float len = bfTexto.GetWidthPoint(text, 8f);
                cb.AddTemplate(headerTemplate, pageSize.GetRight(60) + len, pageSize.GetTop(61));


                /*
                 ================================================
                 TABLA: CABERCERA
                ================================================
                */

                var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 12f, 76f, 12f });
                tbl.TotalWidth = pageSize.Width - 18;

                // LINEA 1
                var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidthBottom = 0, BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Title, parrafoTitulo)) { BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthBottom = 0, BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, BackgroundColor = new iTextSharp.text.BaseColor(255, 103, 43) };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidthBottom = 0, BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);

                //// LINEA 2
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidthTop = 0, BorderWidth = 1, PaddingTop = 14, PaddingBottom = 14 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(SubTitle, parrafoSubTitulo)) { BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthTop = 1, BorderWidth = 1, PaddingTop = 14, PaddingBottom = 14, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidthTop = 0, BorderWidth = 1, PaddingTop = 14, PaddingBottom = 14 };
                tbl.AddCell(c1);
                tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetTop(10), cb);
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
            //int pageN = writer.PageNumber;
            //string text = "Página " + pageN + "/";
            //float len = bfTexto.GetWidthPoint(text, 8);
            //iTextSharp.text.Rectangle pageSize = document.PageSize;
            //cb.SetRgbColorFill(100, 100, 100);
            //cb.BeginText();
            //cb.SetFontAndSize(bfTexto, 8);
            //cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetBottom(30));
            //cb.ShowText(text);
            //cb.EndText();
            //cb.AddTemplate(footerTemplate, pageSize.GetLeft(15) + len, pageSize.GetBottom(30));
        }
        public override void OnCloseDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnCloseDocument(writer, document);
            /*
                ==========================================================
                Codigo para que el número de página muestre en la cabecera
                ==========================================================
            */
            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bfTexto, 8);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            headerTemplate.EndText();

            /*
               =====================================================
               Codigo para que el número de página muestre en el pie
               =====================================================
           */
            //footerTemplate.BeginText();
            //footerTemplate.SetFontAndSize(bfTexto, 8);
            //footerTemplate.SetTextMatrix(0, 0);
            //footerTemplate.ShowText("" + (writer.PageNumber - 1));
            //footerTemplate.EndText();
        }
    }
}
