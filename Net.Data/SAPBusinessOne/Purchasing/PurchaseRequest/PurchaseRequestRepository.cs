using System;
using System.IO;
using SAPbobsCOM;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Create;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
using Net.Business.Entities.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
namespace Net.Data.SAPBusinessOne
{
    public class PurchaseRequestRepository : RepositoryBase<PurchaseRequestEntity>, IPurchaseRequestRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        public PurchaseRequestRepository(IConnectionSQL context, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }


        #region <<< CONSULTAS >>>

        public async Task<ResultadoTransaccionResponse<PurchaseRequestQueryEntity>> GetListByFilter(PurchaseRequestFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PurchaseRequestQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.PurchaseRequest
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
                .Select(n => new PurchaseRequestQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    DocType = n.DocType,
                    Canceled = n.CANCELED,
                    DocStatus = n.DocStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    ReqDate = n.ReqDate
                })
                .OrderByDescending(x => x.DocEntry).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
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

        public async Task<ResultadoTransaccionResponse<PurchaseRequestQueryEntity>> GetByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PurchaseRequestQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                string[] warehouse = await _db.Warehouses
                .Where(n => n.U_FIB_ALMLOG == "Y")
                .Select(n => n.WhsCode) // <-- seleccionas solo el campo string
                .AsNoTracking()
                .ToArrayAsync();

                var data = await _db.PurchaseRequest
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new PurchaseRequestQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    ObjType = n.ObjType,
                    DocStatus = n.DocStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    ReqDate = n.ReqDate,
                    ReqType = n.ReqType,
                    Requester = n.Requester,
                    ReqName = n.ReqName,
                    Branch = n.Branch,
                    Department = n.Department,
                    Notify = n.Notify,
                    Email = n.Email,
                    DocType = n.DocType,
                    OwnerCode = n.OwnerCode,
                    Comments = n.Comments,

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines.Select(s => new PurchaseRequestLinesQueryEntity
                    {
                        DocEntry = s.DocEntry,
                        LineNum = s.LineNum,
                        ObjType = s.ObjType,
                        BaseType = s.BaseType,
                        BaseEntry = s.BaseEntry,
                        BaseLine = s.BaseLine,
                        LineStatus = s.LineStatus,
                        ItemCode = s.ItemCode,
                        Dscription = s.Dscription,
                        LineVendor = s.LineVendor,
                        PqtReqDate = s.PQTReqDate,
                        AcctCode = s.AcctCode,
                        FormatCode = s.ChartOfAccounts != null ? s.ChartOfAccounts.Segment_0 + "-" + s.ChartOfAccounts.Segment_1 + "-" + s.ChartOfAccounts.Segment_2 : "",
                        AcctName = s.ChartOfAccounts != null ? s.ChartOfAccounts.AcctName : "",
                        OcrCode = s.OcrCode,
                        WhsCode = s.WhsCode,
                        U_tipoOpT12 = s.U_tipoOpT12 ?? "",
                        U_FF_TIP_COM = s.U_FF_TIP_COM ?? "",
                        UnitMsr = s.UnitMsr,
                        OnHand = _db.ItemWarehouseInfo
                                 .Where(w => w.ItemCode == s.Item.ItemCode && warehouse.Contains(w.WhsCode))
                                 .Sum(w => (decimal?)w.OnHand) ?? 0,

                        Quantity = s.Quantity ?? 0,
                        OpenQty = s.OpenQty ?? 0
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
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }


        #endregion


        #region <<< OPERACIONES >>>

        public async Task<ResultadoTransaccionResponse<PurchaseRequestLinesQueryEntity>> SetValidateLinesItemsExcel(List<PurchaseRequestLinesItemsValidateEntity> value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PurchaseRequestLinesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            var lines = new List<PurchaseRequestLinesQueryEntity>();

            try
            {
                value ??= [];

                string[] warehouse = await _db.Warehouses
                    .AsNoTracking()
                    .Where(n => n.U_FIB_ALMLOG == "Y")
                    .Select(n => n.WhsCode)
                    .ToArrayAsync();

                for (int i = 0; i < value.Count; i++)
                {
                    #region <<< VALIDACIONES >>>

                    var line = value[i];
                    int nroLinea = i + 1;

                    if (!string.IsNullOrWhiteSpace(line.ItemCode))
                    {
                        _ = await _db.Items
                            .AsNoTracking()
                            .Where(x => x.ItemCode == line.ItemCode)
                            .Select(x => x.ItemCode)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: El código de artículo '{line.ItemCode}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.LineVendor))
                    {
                        _ = await _db.BusinessPartners
                            .AsNoTracking()
                            .Where(x => x.CardCode == line.LineVendor)
                            .Select(x => x.CardCode)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: El código de proveedor '{line.LineVendor}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.WhsCode))
                    {
                        _ = await _db.Warehouses
                            .AsNoTracking()
                            .Where(x => x.WhsCode == line.WhsCode)
                            .Select(x => x.WhsCode)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: El código de almacén '{line.WhsCode}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.FormatCode))
                    {
                        _ = await _db.ChartOfAccounts
                            .AsNoTracking()
                            .Where(x => ((x.Segment_0 ?? "") + "-" + (x.Segment_1 ?? "") + "-" + (x.Segment_2 ?? "")) == line.FormatCode)
                            .Select(x => x.AcctCode)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: La cuenta mayor '{line.FormatCode}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.OcrCode))
                    {
                        _ = await _db.CostCenters
                            .AsNoTracking()
                            .Where(x => x.OcrCode == line.OcrCode)
                            .Select(x => x.OcrCode)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: El código de centro de costo '{line.OcrCode}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.U_tipoOpT12))
                    {
                        _ = await _db.OperationsTypes
                            .AsNoTracking()
                            .Where(x => x.Code == line.U_tipoOpT12)
                            .Select(x => x.Code)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: El código de tipo de operación '{line.U_tipoOpT12}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.U_FF_TIP_COM))
                    {
                        _ = await (
                            from p in _db.UserDefinedFields1.AsNoTracking()
                            join c in _db.UserDefinedFields.AsNoTracking() on new { p.TableID, p.FieldID } equals new { c.TableID, c.FieldID }
                            where c.TableID == "PRQ1" && c.AliasID == "FF_TIP_COM" && p.FldValue == line.U_FF_TIP_COM
                            select p.FldValue
                        ).FirstOrDefaultAsync()
                        ?? throw new Exception($"Línea {nroLinea}: El código de tipo de compra '{line.U_FF_TIP_COM}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.UnitMsr))
                    {
                        _ = await _db.Items
                            .AsNoTracking()
                            .Where(x => x.BuyUnitMsr == line.UnitMsr)
                            .Select(x => x.BuyUnitMsr)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: La unidad de medida '{line.UnitMsr}' no es válida.");
                    }

                    #endregion


                    #region <<< OBTENER VALORES >>>

                    var itemName = await _db.Items
                        .AsNoTracking()
                        .Where(x => x.ItemCode == line.ItemCode)
                        .Select(x => x.ItemName)
                        .FirstOrDefaultAsync();


                    var chartOfAccounts = await _db.ChartOfAccounts
                        .AsNoTracking()
                        .Where(x => ((x.Segment_0 ?? "") + "-" + (x.Segment_1 ?? "") + "-" + (x.Segment_2 ?? "")) == line.FormatCode)
                        .Select(x => new
                        {
                            x.AcctCode,
                            x.AcctName
                        })
                        .FirstOrDefaultAsync();


                    var onHand = await _db.ItemWarehouseInfo
                        .AsNoTracking()
                        .Where(w =>
                            w.ItemCode == line.ItemCode &&
                            warehouse.Contains(w.WhsCode))
                        .SumAsync(w => (decimal?)w.OnHand) ?? 0;

                    #endregion


                    #region <<< AGREGAR A LA LISTA >>>

                    var l = new PurchaseRequestLinesQueryEntity 
                    {
                        ItemCode = line.ItemCode,
                        Dscription = itemName,
                        LineVendor = line.LineVendor,
                        PqtReqDate = line.PqtReqDate,
                        AcctCode = chartOfAccounts.AcctCode,
                        FormatCode = line.FormatCode,
                        AcctName = chartOfAccounts.AcctName,
                        OcrCode = line.OcrCode,
                        WhsCode = line.WhsCode,
                        U_tipoOpT12 = line.U_tipoOpT12,
                        U_FF_TIP_COM = line.U_FF_TIP_COM,
                        UnitMsr = line.UnitMsr,
                        OnHand = onHand,
                        Quantity = line.Quantity
                    };

                    lines.Add(l);

                    #endregion
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {value.Count}";
                resultTransaccion.DataList = lines;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<PurchaseRequestLinesQueryEntity>> SetValidateLinesServicsExcel(List<PurchaseRequestLinesServicesValidateEntity> value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PurchaseRequestLinesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            var lines = new List<PurchaseRequestLinesQueryEntity>();

            try
            {
                value ??= [];

                for (int i = 0; i < value.Count; i++)
                {
                    #region <<< VALIDACIONES >>>

                    var line = value[i];
                    int nroLinea = i + 1;

                    if (!string.IsNullOrWhiteSpace(line.LineVendor))
                    {
                        _ = await _db.BusinessPartners
                            .AsNoTracking()
                            .Where(x => x.CardCode == line.LineVendor)
                            .Select(x => x.CardCode)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: El código de proveedor '{line.LineVendor}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.FormatCode))
                    {
                        _ = await _db.ChartOfAccounts
                            .AsNoTracking()
                            .Where(x => ((x.Segment_0 ?? "") + "-" + (x.Segment_1 ?? "") + "-" + (x.Segment_2 ?? "")) == line.FormatCode)
                            .Select(x => x.AcctCode)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: La cuenta mayor '{line.FormatCode}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.OcrCode))
                    {
                        _ = await _db.CostCenters
                            .AsNoTracking()
                            .Where(x => x.OcrCode == line.OcrCode)
                            .Select(x => x.OcrCode)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: El código de centro de costo '{line.OcrCode}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.U_tipoOpT12))
                    {
                        _ = await _db.OperationsTypes
                            .AsNoTracking()
                            .Where(x => x.Code == line.U_tipoOpT12)
                            .Select(x => x.Code)
                            .FirstOrDefaultAsync()
                            ?? throw new Exception($"Línea {nroLinea}: El código de tipo de operación '{line.U_tipoOpT12}' no existe.");
                    }

                    if (!string.IsNullOrWhiteSpace(line.U_FF_TIP_COM))
                    {
                        _ = await (
                            from p in _db.UserDefinedFields1.AsNoTracking()
                            join c in _db.UserDefinedFields.AsNoTracking() on new { p.TableID, p.FieldID } equals new { c.TableID, c.FieldID }
                            where c.TableID == "PRQ1" && c.AliasID == "FF_TIP_COM" && p.FldValue == line.U_FF_TIP_COM
                            select p.FldValue
                        ).FirstOrDefaultAsync()
                        ?? throw new Exception($"Línea {nroLinea}: El código de tipo de compra '{line.U_FF_TIP_COM}' no existe.");
                    }

                    #endregion


                    #region <<< OBTENER VALORES >>>

                    var chartOfAccounts = await _db.ChartOfAccounts
                        .AsNoTracking()
                        .Where(x => ((x.Segment_0 ?? "") + "-" + (x.Segment_1 ?? "") + "-" + (x.Segment_2 ?? "")) == line.FormatCode)
                        .Select(x => new
                        {
                            x.AcctCode,
                            x.AcctName
                        })
                        .FirstOrDefaultAsync();

                    #endregion


                    #region <<< AGREGAR A LA LISTA >>>

                    var l = new PurchaseRequestLinesQueryEntity
                    {
                        Dscription = line.Dscription,
                        LineVendor = line.LineVendor,
                        PqtReqDate = line.PqtReqDate,
                        AcctCode = chartOfAccounts.AcctCode,
                        FormatCode = line.FormatCode,
                        AcctName = chartOfAccounts.AcctName,
                        OcrCode = line.OcrCode,
                        U_tipoOpT12 = line.U_tipoOpT12,
                        U_FF_TIP_COM = line.U_FF_TIP_COM,
                    };

                    lines.Add(l);

                    #endregion
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {value.Count}";
                resultTransaccion.DataList = lines;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<PurchaseRequestEntity>> SetCreate(PurchaseRequestCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PurchaseRequestEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };
            
            Documents purchaseRequest = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    // Se crea el objeto de solicitud de compra
                    purchaseRequest = company.GetBusinessObject(BoObjectTypes.oPurchaseRequest);


                    #region <<< CABECERA >>>

                    purchaseRequest.DocDate = value.DocDate;
                    purchaseRequest.DocDueDate = value.DocDueDate;
                    purchaseRequest.TaxDate = value.TaxDate;
                    purchaseRequest.RequriedDate = value.ReqDate;
                    purchaseRequest.DocObjectCode = BoObjectTypes.oPurchaseRequest;
                    purchaseRequest.DocType = value.DocType switch
                    {
                        "I" => BoDocumentTypes.dDocument_Items,
                        "S" => BoDocumentTypes.dDocument_Service,
                        _ => throw new ArgumentException($"DocType inválido para SAP Business One: '{value.DocType}'. Se esperaba 'I' (Artículo) o 'S' (Servicio)."),
                    };

                    purchaseRequest.ReqType = value.ReqType;
                    purchaseRequest.Requester = value.Requester;
                    purchaseRequest.RequesterName = value.ReqName;
                    purchaseRequest.RequesterBranch = value.Branch;
                    purchaseRequest.RequesterDepartment = value.Department;
                    purchaseRequest.SendNotification = value.Notify == "Y" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                    purchaseRequest.SendNotification = value.Notify switch
                    {
                        "Y" => BoYesNoEnum.tYES,
                        "N" => BoYesNoEnum.tNO,
                        _ => throw new ArgumentException($"SendNotification inválido para SAP Business One: '{value.DocType}'. Se esperaba 'Y' o 'N'."),
                    };
                    purchaseRequest.RequesterEmail = value.Email;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    purchaseRequest.DocumentsOwner = value.OwnerCode;
                    purchaseRequest.Comments = value.Comments;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    purchaseRequest.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                    #endregion


                    #region <<< DETALLE >>>

                    bool isItem = value.DocType == "I";

                    foreach (var line in value.Lines)
                    {
                        if (isItem)
                        {
                            purchaseRequest.Lines.ItemCode = line.ItemCode;
                            purchaseRequest.Lines.WarehouseCode = line.WhsCode;
                            purchaseRequest.Lines.Quantity = line.Quantity;
                        }

                        purchaseRequest.Lines.ItemDescription = line.Dscription;
                        purchaseRequest.Lines.LineVendor = line.LineVendor;
                        purchaseRequest.Lines.RequiredDate = line.PqtReqDate;
                        purchaseRequest.Lines.AccountCode = line.AcctCode;
                        purchaseRequest.Lines.CostingCode = line.OcrCode;

                        purchaseRequest.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        purchaseRequest.Lines.UserFields.Fields.Item("U_FF_TIP_COM").Value = line.U_FF_TIP_COM;
                        purchaseRequest.Lines.Add();
                    }

                    #endregion


                    
                    if (purchaseRequest.Add() == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "La solictud de compra registrada con éxito.";
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
                    _companyProviderSap.LiberarObjetosCOM(purchaseRequest);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionResponse<PurchaseRequestEntity>> SetUpdate(PurchaseRequestUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PurchaseRequestEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Documents purchaseRequest = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    // Se crea el objeto de solicitud de compra
                    purchaseRequest = company.GetBusinessObject(BoObjectTypes.oPurchaseRequest);

                    if (!purchaseRequest.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la solictud de compra.");
                    }

                    #region <<< CABECERA >>>

                    purchaseRequest.DocDate = value.DocDate;
                    purchaseRequest.DocDueDate = value.DocDueDate;
                    purchaseRequest.TaxDate = value.TaxDate;
                    purchaseRequest.RequriedDate = value.ReqDate;
                    
                    purchaseRequest.ReqType = value.ReqType;
                    purchaseRequest.Requester = value.Requester;
                    purchaseRequest.RequesterName = value.ReqName;
                    purchaseRequest.RequesterBranch = value.Branch;
                    purchaseRequest.RequesterDepartment = value.Department;
                    purchaseRequest.SendNotification = value.Notify switch
                    {
                        "Y" => BoYesNoEnum.tYES,
                        "N" => BoYesNoEnum.tNO,
                        _ => throw new ArgumentException($"SendNotification inválido para SAP Business One: '{value.DocType}'. Se esperaba 'Y' o 'N'."),
                    };
                    purchaseRequest.RequesterEmail = value.Email;

                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    purchaseRequest.DocumentsOwner = value.OwnerCode;
                    purchaseRequest.Comments = value.Comments;

                    // ===========================================================================================
                    // AUDITORÍA
                    // ===========================================================================================
                    purchaseRequest.UserFields.Fields.Item("U_UsrUpdate").Value = value.U_UsrUpdate;

                    #endregion


                    #region <<< DETALLE >>>

                    bool isItem = value.DocType == "I";

                    // NUEVO: SE AGREGA LINEA
                    foreach (var line in value.Lines.Where(x => x.Record == 1))
                    {
                        purchaseRequest.Lines.Add();

                        if (isItem)
                        {
                            purchaseRequest.Lines.ItemCode = line.ItemCode;
                            purchaseRequest.Lines.WarehouseCode = line.WhsCode;
                            purchaseRequest.Lines.Quantity = line.Quantity;
                        }

                        purchaseRequest.Lines.ItemDescription = line.Dscription;
                        purchaseRequest.Lines.LineVendor = line.LineVendor;
                        purchaseRequest.Lines.RequiredDate = line.PqtReqDate;
                        purchaseRequest.Lines.AccountCode = line.AcctCode;
                        purchaseRequest.Lines.CostingCode = line.OcrCode;

                        purchaseRequest.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        purchaseRequest.Lines.UserFields.Fields.Item("U_FF_TIP_COM").Value = line.U_FF_TIP_COM;
                    }

                    // EXISTE: SE MODIFICA LINEA
                    foreach (var line in value.Lines.Where(x => x.Record == 2 && x.LineStatus == "O"))
                    {
                        for (int i = 0; i < purchaseRequest.Lines.Count; i++)
                        {
                            purchaseRequest.Lines.SetCurrentLine(i);
                            if (purchaseRequest.Lines.LineNum == line.LineNum)
                            {
                                if (isItem)
                                {
                                    purchaseRequest.Lines.ItemCode = line.ItemCode;
                                    purchaseRequest.Lines.WarehouseCode = line.WhsCode;
                                    purchaseRequest.Lines.Quantity = line.Quantity;
                                }

                                purchaseRequest.Lines.ItemDescription = line.Dscription;
                                purchaseRequest.Lines.LineVendor = line.LineVendor;
                                purchaseRequest.Lines.RequiredDate = line.PqtReqDate;
                                purchaseRequest.Lines.AccountCode = line.AcctCode;
                                purchaseRequest.Lines.CostingCode = line.OcrCode;
                                
                                purchaseRequest.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                                purchaseRequest.Lines.UserFields.Fields.Item("U_FF_TIP_COM").Value = line.U_FF_TIP_COM;
                            }
                        }
                    }

                    // EXISTE: SE ELIMINA LINEA
                    foreach (var line in value.Lines.Where(x => x.Record == 3))
                    {
                        for (int i = 0; i < purchaseRequest.Lines.Count; i++)
                        {
                            purchaseRequest.Lines.SetCurrentLine(i);
                            if (purchaseRequest.Lines.LineNum == line.LineNum)
                            {
                                purchaseRequest.Lines.Delete();
                            }
                        }
                    }

                    #endregion



                    if (purchaseRequest.Update() == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "La solictud de compra actualizada con éxito.";
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
                    _companyProviderSap.LiberarObjetosCOM(purchaseRequest);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionResponse<PurchaseRequestEntity>> SetClose(PurchaseRequestCloseEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<PurchaseRequestEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Documents purchaseRequest = null;

            return await Task.Run(() =>
            {
                try
                {
                    // Conexión a SAP
                    var company = _companyProviderSap.GetCompany();


                    // Se crea el objeto de solicitud de compra
                    purchaseRequest = company.GetBusinessObject(BoObjectTypes.oPurchaseRequest);


                    if (!purchaseRequest.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la solictud de compra.");
                    }

                    purchaseRequest.UserFields.Fields.Item("U_UsrClose").Value = value.U_UsrClose;

                    

                    if (purchaseRequest.Update() == 0)
                    {
                        var regClose = purchaseRequest.Close();

                        if (regClose == 0)
                        {
                            resultTransaccion.IdRegistro = 0;
                            resultTransaccion.ResultadoCodigo = 0;
                            resultTransaccion.ResultadoDescripcion = "La solictud de compra cerrada con éxito ..!";
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
                    _companyProviderSap.LiberarObjetosCOM(purchaseRequest);
                }

                return resultTransaccion;
            });
        }

        #endregion


        #region <<< EXPORTACIONES >>>

        public Task<ResultadoTransaccionResponse<MemoryStream>> GetDownloadItemsTemplate()
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
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Plantilla" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                        ExportToExcel.ConstructCell("Codigo", CellValues.String),
                        ExportToExcel.ConstructCell("Proveedor", CellValues.String),
                        ExportToExcel.ConstructCell("Fecha necesaria", CellValues.String),
                        ExportToExcel.ConstructCell("Cuenta mayor", CellValues.String),
                        ExportToExcel.ConstructCell("Centro de costo", CellValues.String),
                        ExportToExcel.ConstructCell("Almacen", CellValues.String),
                        ExportToExcel.ConstructCell("Codigo tipo de operacion", CellValues.String),
                        ExportToExcel.ConstructCell("Codigo tipo de compra", CellValues.String),
                        ExportToExcel.ConstructCell("UM", CellValues.String),
                        ExportToExcel.ConstructCell("Cantidad", CellValues.String)
                    );
                    sheetData.AppendChild(row);

                    row = new Row();

                    row.Append(
                        ExportToExcel.ConstructCell("MIPP-HP550J", CellValues.String),
                        ExportToExcel.ConstructCell("", CellValues.String),
                        ExportToExcel.ConstructCell(DateTime.Now.ToString("yyyy-MM-dd"), CellValues.String),
                        ExportToExcel.ConstructCell("251000-00-00", CellValues.String),
                        ExportToExcel.ConstructCell("CH2-RAS", CellValues.String),
                        ExportToExcel.ConstructCell("CH2-SD", CellValues.String),
                        ExportToExcel.ConstructCell("02", CellValues.String),
                        ExportToExcel.ConstructCell("1", CellValues.String),
                        ExportToExcel.ConstructCell("KG", CellValues.String),
                        ExportToExcel.ConstructCell("1", CellValues.Number)
                    );
                    sheetData.AppendChild(row);

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

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

            return Task.FromResult(resultTransaccion);
        }

        public Task<ResultadoTransaccionResponse<MemoryStream>> GetDownloadServicesTemplate()
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
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Plantilla" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                        ExportToExcel.ConstructCell("Descripcion", CellValues.String),
                        ExportToExcel.ConstructCell("Proveedor", CellValues.String),
                        ExportToExcel.ConstructCell("Fecha necesaria", CellValues.String),
                        ExportToExcel.ConstructCell("Cuenta mayor", CellValues.String),
                        ExportToExcel.ConstructCell("Centro de costo", CellValues.String),
                        ExportToExcel.ConstructCell("Codigo tipo de operacion", CellValues.String),
                        ExportToExcel.ConstructCell("Codigo tipo de compra", CellValues.String)
                    );
                    sheetData.AppendChild(row);

                    row = new Row();

                    row.Append(
                        ExportToExcel.ConstructCell("1 LAPTOP LENOVO i5, 7GB DE RAM", CellValues.String),
                        ExportToExcel.ConstructCell("", CellValues.String),
                        ExportToExcel.ConstructCell(DateTime.Now.ToString("yyyy-MM-dd"), CellValues.String),
                        ExportToExcel.ConstructCell("251000-00-00", CellValues.String),
                        ExportToExcel.ConstructCell("CH2-RAS", CellValues.String),
                        ExportToExcel.ConstructCell("02", CellValues.String),
                        ExportToExcel.ConstructCell("1", CellValues.String)
                    );
                    sheetData.AppendChild(row);

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

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

            return Task.FromResult(resultTransaccion);
        }

        #endregion
    }
}
