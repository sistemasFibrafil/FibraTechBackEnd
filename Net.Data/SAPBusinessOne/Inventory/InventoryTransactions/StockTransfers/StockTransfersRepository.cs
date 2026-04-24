using System;
using System.IO;
using SAPbobsCOM;
using AutoMapper;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Query;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Filter;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Create;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Update;
using Net.Business.Entities.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Entities;
namespace Net.Data.SAPBusinessOne
{
    public class StockTransfersRepository : RepositoryBase<StockTransfersEntity>, IStockTransfersRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        const string DB_ESQUEMA = "";
        const string SP_GET_TRANSFERENCIASTOCK_BY_DOCENTRY = DB_ESQUEMA + "INV_GetTransferenciaStockByDocEntry";
        const string SP_GET_LIST_TRANSFERENCIASTOCK_DETALLE_BY_DOCENTRY = DB_ESQUEMA + "INV_GetListTransferenciaStockDetalleByDocEntry";

        public StockTransfersRepository(IConnectionSQL context, IConfiguration configuration, DataContextSAPBusinessOne db, IMapper mapper, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }


        #region <<< CONSULTAS >>>

        public async Task<ResultadoTransaccionResponse<TransferenciaStockQueryEntity>> GetListByFilter(TransferenciaStockFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<TransferenciaStockQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.StockTransfers
                .AsNoTracking()
                .Where(x => x.DocDate >= value.StartDate && x.DocDate <= value.EndDate);

                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    bool hasLetter = filter.Any(char.IsLetter);
                    bool hasDigit = filter.Any(char.IsDigit);
                    bool hasHyphen = filter.Contains('-');

                    if (hasLetter)
                    {
                        query = query.Where(n =>
                        EF.Functions.Like(n.U_BPP_MDSD, $"%{filter}%") ||
                        EF.Functions.Like(n.U_BPP_MDSD + "-" + n.U_BPP_MDCD, $"%{filter}%")
                        );
                    }
                    else if (hasDigit && hasHyphen)
                    {
                        query = query.Where(n =>
                            EF.Functions.Like(n.U_BPP_MDSD + "-" + n.U_BPP_MDCD, $"%{filter}%")
                        );
                    }
                    else if (hasDigit && !hasLetter && !hasHyphen)
                    {
                        query = query.Where(n =>
                            EF.Functions.Like(n.DocNum.ToString(),$"%{filter}%") ||
                            EF.Functions.Like(n.U_BPP_MDSD, $"%{filter}%") ||
                            EF.Functions.Like(n.U_BPP_MDCD, $"%{filter}%")
                        );
                    }
                }

                if (!string.IsNullOrWhiteSpace(value.DocStatus))
                {
                    var docStatus = value.DocStatus.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(x => docStatus.Contains(x.DocStatus));
                }

                var list = await query
                .Select(n => new TransferenciaStockQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    DocStatus = n.DocStatus,
                    U_BPP_MDSD = n.U_BPP_MDSD,
                    U_BPP_MDCD = n.U_BPP_MDCD,
                    DocDate = n.DocDate,
                    TaxDate = n.TaxDate,
                    Filler = n.Filler,
                    ToWhsCode = n.ToWhsCode,
                    U_FIB_FromPkg = n.U_FIB_FromPkg,

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
        public async Task<ResultadoTransaccionResponse<TransferenciaStockQueryEntity>> GetByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<TransferenciaStockQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.StockTransfers
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new TransferenciaStockQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    DocStatus = n.DocStatus,
                    U_FIB_FromPkg = n.U_FIB_FromPkg,
                    U_BPP_MDTD = n.U_BPP_MDTD,
                    U_BPP_MDSD = n.U_BPP_MDSD,
                    U_BPP_MDCD = n.U_BPP_MDCD,
                    DocDate = n.DocDate,
                    TaxDate = n.TaxDate,

                    CardCode = n.CardCode,
                    CardName = n.CardName,
                    CntctCode = n.CntctCode,
                    Address = n.Address,
                    Filler = n.Filler,
                    ToWhsCode = n.ToWhsCode,

                    U_FIB_TIP_TRANS = n.U_FIB_TIP_TRANS,
                    U_FIB_COD_TRA = n.U_FIB_COD_TRA,
                    U_FIB_TIPDOC_TRA = n.U_FIB_TIPDOC_TRA,
                    U_BPP_MDRT = n.U_BPP_MDRT,
                    U_BPP_MDNT = n.U_BPP_MDNT,
                    U_BPP_MDVC = n.U_BPP_MDVC,
                    U_FIB_TIPDOC_COND = n.U_FIB_TIPDOC_COND,
                    U_FIB_NUMDOC_COD = n.U_FIB_NUMDOC_COD,
                    U_FIB_NOM_COND = n.U_FIB_NOM_COND,
                    U_FIB_APE_COND = n.U_FIB_APE_COND,
                    U_BPP_MDFN = n.U_BPP_MDFN,
                    U_BPP_MDFC = n.U_BPP_MDFC,

                    U_FIB_TIP_TRAS = n.U_FIB_TIP_TRAS,
                    U_BPP_MDMT = n.U_BPP_MDMT,
                    U_BPP_MDTS = n.U_BPP_MDTS,

                    SlpCode = n.SlpCode,
                    U_FIB_NBULTOS = n.U_FIB_NBULTOS ?? 0,
                    U_FIB_KG = n.U_FIB_KG ?? 0,
                    JrnlMemo = n.JrnlMemo,
                    Comments = n.Comments,
                    Lines = n.Lines.Select(s => new TransferenciaStock1QueryEntity
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
                        U_tipoOpT12Nam = s.OperationType != null ? s.OperationType.U_descrp : "",
                        UnitMsr = s.UnitMsr,
                        Quantity = s.Quantity,
                        OpenQty = s.OpenQty,
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

        #endregion


        #region <<< OPERACIONES >>>

        public async Task<ResultadoTransaccionResponse<StockTransfersEntity>> SetCreate(StockTransfersCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<StockTransfersEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            GeneralData oGeneralData = null;
            StockTransfer stockTransfer = null;
            CompanyService oCompService = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralParams = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    company = _companyProviderSap.GetCompany();


                    // Empieza la transacción en SAP
                    if (!company.InTransaction) company.StartTransaction();


                    // Se crea el objeto de la transferencia de stock
                    stockTransfer = company.GetBusinessObject(BoObjectTypes.oStockTransfer);


                    #region <<< CABECERA >>>

                    stockTransfer.DocDate = value.DocDate;
                    stockTransfer.TaxDate = value.TaxDate;
                    stockTransfer.DocObjectCode = BoObjectTypes.oStockTransfer;

                    stockTransfer.UserFields.Fields.Item("U_FIB_FromPkg").Value =
                    value.PickingLines.Count switch
                    {
                        0 => "N",
                        _ => "Y"
                    };

                    // ===========================================================================================
                    // SUNAT
                    // ===========================================================================================
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDTD").Value = value.U_BPP_MDTD;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDSD").Value = value.U_BPP_MDSD;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDCD").Value = value.U_BPP_MDCD;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    stockTransfer.CardCode = value.CardCode;
                    stockTransfer.CardName = value.CardName;
                    stockTransfer.ContactPerson = value.CntctCode;
                    stockTransfer.Address = value.Address;                    

                    // ALMACEN
                    // ===========================================================================================
                    stockTransfer.FromWarehouse = value.Filler;
                    stockTransfer.ToWarehouse = value.ToWhsCode;

                    // ===========================================================================================
                    // TRANSPORTISTA
                    // ===========================================================================================
                    stockTransfer.UserFields.Fields.Item("U_FIB_TIP_TRANS").Value = value.U_FIB_TIP_TRANS;
                    stockTransfer.UserFields.Fields.Item("U_FIB_COD_TRA").Value = value.U_FIB_COD_TRA;
                    stockTransfer.UserFields.Fields.Item("U_FIB_TIPDOC_TRA").Value = value.U_FIB_TIPDOC_TRA;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDVC").Value = value.U_BPP_MDVC;

                    // ===========================================================================================
                    // CONDUCTOR
                    // ===========================================================================================
                    stockTransfer.UserFields.Fields.Item("U_FIB_TIPDOC_COND").Value = value.U_FIB_TIPDOC_COND;
                    stockTransfer.UserFields.Fields.Item("U_FIB_NUMDOC_COD").Value = value.U_FIB_NUMDOC_COD;
                    stockTransfer.UserFields.Fields.Item("U_FIB_NOM_COND").Value = value.U_FIB_NOM_COND;
                    stockTransfer.UserFields.Fields.Item("U_FIB_APE_COND").Value = value.U_FIB_APE_COND;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDFN").Value = value.U_BPP_MDFN;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDFC").Value = value.U_BPP_MDFC;

                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    stockTransfer.UserFields.Fields.Item("U_FIB_TIP_TRAS").Value = value.U_FIB_TIP_TRAS;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDMT").Value = value.U_BPP_MDMT;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDTS").Value = value.U_BPP_MDTS;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    stockTransfer.SalesPersonCode = value.SlpCode;
                    stockTransfer.UserFields.Fields.Item("U_FIB_NBULTOS").Value = value.U_FIB_NBULTOS;
                    stockTransfer.UserFields.Fields.Item("U_FIB_KG").Value = value.U_FIB_KG;
                    stockTransfer.JournalMemo = value.JrnlMemo;
                    stockTransfer.Comments = value.Comments;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    stockTransfer.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;


                    // Se valida que el número de correlativo no sea vacío
                    if (string.IsNullOrWhiteSpace(value.U_BPP_MDCD)) throw new Exception("El número de correlativo no debe ser vacío.");

                    // Se obtiene la serie de numeración
                    var serieDocumento = _db.DocumentNumberingSeriesSunat
                    .Where(x => x.U_BPP_NDTD == value.U_BPP_MDTD && x.U_BPP_NDSD == value.U_BPP_MDSD)
                    .FirstOrDefault() ?? throw new Exception("Tipo y Serie de documento no existe.");

                    #endregion


                    #region <<< DETALLE >>>

                    foreach (var line in value.Lines)
                    {
                        if(line.BaseEntry != null) stockTransfer.Lines.BaseEntry = (int)line.BaseEntry;
                        if (line.BaseLine != null) stockTransfer.Lines.BaseLine = (int)line.BaseLine;
                        if (line.BaseType != null) stockTransfer.Lines.BaseType = InvBaseDocTypeEnum.InventoryTransferRequest;

                        stockTransfer.Lines.ItemCode = line.ItemCode;
                        stockTransfer.Lines.ItemDescription = line.Dscription;
                        stockTransfer.Lines.FromWarehouseCode = line.FromWhsCod;
                        stockTransfer.Lines.WarehouseCode = line.WhsCode;
                        stockTransfer.Lines.Quantity = line.Quantity;

                        stockTransfer.Lines.UserFields.Fields.Item("U_FIB_FromPkg").Value = line.U_FIB_FromPkg;
                        stockTransfer.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        stockTransfer.Lines.UserFields.Fields.Item("U_FIB_NBulto").Value = line.U_FIB_NBulto;
                        stockTransfer.Lines.UserFields.Fields.Item("U_FIB_PesoKg").Value = line.U_FIB_PesoKg;
                        stockTransfer.Lines.Add();
                    }

                    #endregion


                    var reg = stockTransfer.Add();

                    if (reg == 0)
                    {
                        // Se genera un nuevo número de correlativo
                        serieDocumento.U_BPP_NDCD = (int.Parse(value.U_BPP_MDCD) + 1).ToString();

                        // Se marca solo el campo U_BPP_NDCD como modificado
                        _db.Entry(serieDocumento).Property(e => e.U_BPP_NDCD).IsModified = true;

                        if (!_db.ChangeTracker.HasChanges()) throw new Exception("No se detectaron cambios.");

                        // Se actualiza el número de correlativo en la serie de numeración
                        _db.SaveChangesAsync();


                        if (value.PickingLines.Count > 0)
                        {
                            // Se obtiene el docEntry del objeto que se acaba de crear
                            var key = company.GetNewObjectKey();
                            var docEntry = key == null ? 0 : int.Parse(key);


                            stockTransfer = company.GetBusinessObject(BoObjectTypes.oStockTransfer);


                            // Se obtiene la transferencia de stock registrado
                            if (!stockTransfer.GetByKey(docEntry)) throw new Exception("No existe la transferencia de stock.");


                            oCompService = company.GetCompanyService();

                            oGeneralService = oCompService.GetGeneralService("FIB_OPKG");


                            foreach (var line in value.PickingLines)
                            {
                                for (int i = 0; i < stockTransfer.Lines.Count; i++)
                                {
                                    stockTransfer.Lines.SetCurrentLine(i);
                                    if (stockTransfer.Lines.BaseEntry == line.U_BaseEntry && stockTransfer.Lines.BaseLine == line.U_BaseLine)
                                    {
                                        oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                        oGeneralParams.SetProperty("DocEntry", line.DocEntry);
                                        oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                                        
                                        oGeneralData.SetProperty("U_TrgetEntry", stockTransfer.Lines.DocEntry);
                                        oGeneralData.SetProperty("U_TargetType", BoObjectTypes.oStockTransfer);
                                        oGeneralData.SetProperty("U_TrgetLine", stockTransfer.Lines.LineNum);
                                        oGeneralData.SetProperty("U_Status", line.U_Status);
                                        oGeneralData.SetProperty("U_UsrUpdate", line.U_UsrUpdate);
                                        oGeneralService.Update(oGeneralData);
                                        oGeneralService.Close(oGeneralParams);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                    }


                    // Se finaliza la transacción en SAP
                    if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_Commit);


                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "La transferencia de stock registrada con éxito.";
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
                    _companyProviderSap.LiberarObjetosCOM(stockTransfer, oGeneralData, oCompService, oGeneralService, oGeneralParams);
                }

                return resultTransaccion;
            });
        }
        public async Task<ResultadoTransaccionResponse<StockTransfersEntity>> SetUpdate(StockTransfersUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<StockTransfersEntity>
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


                    // Se crea el objeto de la transferecncia de stock
                    stockTransfer = company.GetBusinessObject(BoObjectTypes.oStockTransfer);


                    if (!stockTransfer.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la transferencia de stock.");
                    }

                    #region <<< CABECERA >>>

                    var numeroDocumento = stockTransfer.UserFields.Fields.Item("U_BPP_MDCD").Value;

                    stockTransfer.TaxDate = value.TaxDate;

                    // ===========================================================================================
                    // SUNAT
                    // ===========================================================================================
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDTD").Value = value.U_BPP_MDTD;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDSD").Value = value.U_BPP_MDSD;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDCD").Value = value.U_BPP_MDCD;

                    // ===========================================================================================
                    // SOCIO DE NEGOCIO
                    // ===========================================================================================
                    stockTransfer.CardCode = value.CardCode;

                    // ===========================================================================================
                    // TRANSPORTISTA
                    // ===========================================================================================
                    stockTransfer.UserFields.Fields.Item("U_FIB_TIP_TRANS").Value = value.U_FIB_TIP_TRANS;
                    stockTransfer.UserFields.Fields.Item("U_FIB_COD_TRA").Value = value.U_FIB_COD_TRA;
                    stockTransfer.UserFields.Fields.Item("U_FIB_TIPDOC_TRA").Value = value.U_FIB_TIPDOC_TRA;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDVC").Value = value.U_BPP_MDVC;

                    // ===========================================================================================
                    // CONDUCTOR
                    // ===========================================================================================
                    stockTransfer.UserFields.Fields.Item("U_FIB_TIPDOC_COND").Value = value.U_FIB_TIPDOC_COND;
                    stockTransfer.UserFields.Fields.Item("U_FIB_NUMDOC_COD").Value = value.U_FIB_NUMDOC_COD;
                    stockTransfer.UserFields.Fields.Item("U_FIB_NOM_COND").Value = value.U_FIB_NOM_COND;
                    stockTransfer.UserFields.Fields.Item("U_FIB_APE_COND").Value = value.U_FIB_APE_COND;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDFN").Value = value.U_BPP_MDFN;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDFC").Value = value.U_BPP_MDFC;

                    // ===========================================================================================
                    // OTROS
                    // ===========================================================================================
                    stockTransfer.UserFields.Fields.Item("U_FIB_TIP_TRAS").Value = value.U_FIB_TIP_TRAS;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDMT").Value = value.U_BPP_MDMT;
                    stockTransfer.UserFields.Fields.Item("U_BPP_MDTS").Value = value.U_BPP_MDTS;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    stockTransfer.SalesPersonCode = value.SlpCode;
                    stockTransfer.UserFields.Fields.Item("U_FIB_NBULTOS").Value = value.U_FIB_NBULTOS;
                    stockTransfer.UserFields.Fields.Item("U_FIB_KG").Value = value.U_FIB_KG;
                    stockTransfer.JournalMemo = value.JrnlMemo;
                    stockTransfer.Comments = value.Comments;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    stockTransfer.UserFields.Fields.Item("U_UsrUpdate").Value = value.U_UsrUpdate;


                    // Se valida que el número de correlativo no sea vacío
                    if (value.U_BPP_MDCD == null || value.U_BPP_MDCD == "") throw new Exception("El número de correlativo no debe ser vacío.");

                    // Se obtiene la serie de numeración
                    var serieDocumento = _db.DocumentNumberingSeriesSunat
                    .Where(x => x.U_BPP_NDTD == value.U_BPP_MDTD && x.U_BPP_NDSD == value.U_BPP_MDSD)
                    .FirstOrDefault() ?? throw new Exception("Tipo y Serie de documento no existe.");

                    #endregion


                    var reg = stockTransfer.Update();

                    if (reg == 0)
                    {
                        if (numeroDocumento != value.U_BPP_MDCD)
                        {
                            // Se genera un nuevo número de correlativo
                            serieDocumento.U_BPP_NDCD = (int.Parse(value.U_BPP_MDCD) + 1).ToString();

                            // Se marca solo el campo U_BPP_NDCD como modificado
                            _db.Entry(serieDocumento).Property(e => e.U_BPP_NDCD).IsModified = true;

                            if (!_db.ChangeTracker.HasChanges()) throw new Exception("No se detectaron cambios.");

                            // Se actualiza el número de correlativo en la serie de numeración
                            _db.SaveChangesAsync();
                        }

                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "La transferencia de stock actualizada con éxito.";
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

        #endregion


        #region <<< IMPRESIONES >>>

        public async Task<ResultadoTransaccionResponse<MemoryStream>> GetFormatoPdfByDocEntry(int id)
        {
            var header = new StockTransfersPrintEntity();
            var linea = new List<StockTransfers1PrintEntity>();
            var resultTransaccion = new ResultadoTransaccionResponse<MemoryStream>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_TRANSFERENCIASTOCK_BY_DOCENTRY, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@DocEntry", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            header = context.Convert<StockTransfersPrintEntity>(reader);
                        }
                    }

                    iTextSharp.text.Document doc = new iTextSharp.text.Document();
                    doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                    doc.SetMargins(10f, 10f, 70f, 10f);
                    MemoryStream ms = new MemoryStream();
                    iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                    write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                    // Our custom Header and Footer is done using Event Handler
                    var pageEventHelperTransferencia = new PageEventHelperTransferencia();
                    write.PageEvent = pageEventHelperTransferencia;

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
                    pageEventHelperTransferencia.Title = header.Title;
                    pageEventHelperTransferencia.SubTitle = header.SubTitle;
                    pageEventHelperTransferencia.Codigo = header.Codigo;
                    pageEventHelperTransferencia.Version = header.Version;
                    pageEventHelperTransferencia.Vigencia = header.Vigencia;

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


                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_TRANSFERENCIASTOCK_DETALLE_BY_DOCENTRY, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@DocEntry", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            linea = (List<StockTransfers1PrintEntity>)context.ConvertTo<StockTransfers1PrintEntity>(reader);
                        }
                    }

                    //============================
                    //TABLA: 2 - Cabecera del deatalle
                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 5f, 16f, 62f, 6f, 6f, 5f }) { WidthPercentage = 100 };
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("# SOL", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
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
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.NumSolicitud.ToString(), parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        tbl.AddCell(c1);
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

        #endregion
    }


    #region <<< IMPRESIONES >>>

    public class PageEventHelperTransferencia : iTextSharp.text.pdf.PdfPageEventHelper
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

    #endregion
}
