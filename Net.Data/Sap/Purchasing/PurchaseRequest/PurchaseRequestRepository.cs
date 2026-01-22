using System;
using SAPbobsCOM;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace Net.Data.Sap
{
    public class PurchaseRequestRepository : RepositoryBase<PurchaseRequestEntity>, IPurchaseRequestRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSap _db;
        private readonly CompanyProviderSap _companyProviderSap;

        public PurchaseRequestRepository(IConnectionSQL context, DataContextSap db, CompanyProviderSap companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }


        public async Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> GetListByFilter(PurchaseRequestFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PurchaseRequestEntity>
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
                .Select(n => new PurchaseRequestEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    DocStatus = n.DocStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    ReqDate = n.ReqDate,
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

        public async Task<ResultadoTransaccionEntity<PurchaseRequestQueryEntity>> GetByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PurchaseRequestQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
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
                    Lines = n.Lines.Select(s => new PurchaseRequest1QueryEntity
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
                        U_tipoOpT12Nam = s.TipoOperacion != null ? s.TipoOperacion.U_descrp : "",

                        U_FF_TIP_COM = s.U_FF_TIP_COM ?? "",
                        U_FF_TIP_COM_NAM = _db.UserDefinedFields
                                           .Where(c => c.TableID == "PRQ1" && c.AliasID == "FF_TIP_COM")
                                           .SelectMany(c => c.Lines)
                                           .Where(l => l.FldValue == (s.U_FF_TIP_COM ?? ""))
                                           .Select(l => l.Descr)
                                           .FirstOrDefault() ?? "",
                        UnitMsr = s.UnitMsr,
                        Quantity = s.Quantity,
                        OpenQty = s.OpenQty
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
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> SetCreate(PurchaseRequestCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PurchaseRequestEntity>
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


                    purchaseRequest = company.GetBusinessObject(BoObjectTypes.oPurchaseRequest);

                    #region <<< CABECERA >>>

                    purchaseRequest.DocDate = value.DocDate;
                    purchaseRequest.DocDueDate = value.DocDueDate;
                    purchaseRequest.TaxDate = value.TaxDate;
                    purchaseRequest.RequriedDate = value.ReqDate;
                    purchaseRequest.DocObjectCode = BoObjectTypes.oPurchaseRequest;
                    purchaseRequest.DocType = value.DocType == "I" ? BoDocumentTypes.dDocument_Items : BoDocumentTypes.dDocument_Service;

                    purchaseRequest.ReqType = value.ReqType;
                    purchaseRequest.Requester = value.Requester;
                    purchaseRequest.RequesterName = value.ReqName;
                    purchaseRequest.RequesterBranch = value.Branch;
                    purchaseRequest.RequesterDepartment = value.Department;
                    purchaseRequest.SendNotification = value.Notify == "Y" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                    purchaseRequest.RequesterEmail = value.Email;
                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    purchaseRequest.DocumentsOwner = value.OwnerCode;
                    purchaseRequest.Comments = value.Comments;

                    //if (value.U_UsrCreate != null) purchaseRequest.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                    #endregion

                    #region <<< DETALLE >>>

                    foreach (var line in value.Lines)
                    {
                       if (value.DocType == "I") purchaseRequest.Lines.ItemCode = line.ItemCode;
                        purchaseRequest.Lines.ItemDescription = line.Dscription;
                        purchaseRequest.Lines.LineVendor = line.LineVendor;
                        purchaseRequest.Lines.RequiredDate = line.PqtReqDate;
                        purchaseRequest.Lines.AccountCode = line.AcctCode;
                        purchaseRequest.Lines.CostingCode = line.OcrCode;
                        if (value.DocType == "I") purchaseRequest.Lines.WarehouseCode = line.WhsCode;
                        if (value.DocType == "I") purchaseRequest.Lines.Quantity = (double)line.Quantity;
                        if (line.U_tipoOpT12 != null || line.U_tipoOpT12 == "") purchaseRequest.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        if (line.U_FF_TIP_COM != null || line.U_FF_TIP_COM == "") purchaseRequest.Lines.UserFields.Fields.Item("U_FF_TIP_COM").Value = line.U_FF_TIP_COM;
                        purchaseRequest.Lines.Add();
                    }

                    #endregion

                    var reg = purchaseRequest.Add();

                    if (reg == 0)
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

        public async Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> SetUpdate(PurchaseRequestUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PurchaseRequestEntity>
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
                    purchaseRequest.DocObjectCode = BoObjectTypes.oPurchaseRequest;
                    
                    purchaseRequest.ReqType = value.ReqType;
                    purchaseRequest.Requester = value.Requester;
                    purchaseRequest.RequesterName = value.ReqName;
                    purchaseRequest.RequesterBranch = value.Branch;
                    purchaseRequest.RequesterDepartment = value.Department;
                    purchaseRequest.SendNotification = value.Notify == "Y" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                    purchaseRequest.RequesterEmail = value.Email;
                    // ===========================================================================================
                    // PIE
                    // ===========================================================================================
                    purchaseRequest.DocumentsOwner = value.OwnerCode;
                    purchaseRequest.Comments = value.Comments;

                    //if (value.U_UsrCreate != null) purchaseRequest.UserFields.Fields.Item("U_UsrUpate").Value = value.U_UsrCreate;

                    #endregion

                    #region <<< DETALLE >>>

                    // NUEVO: SE AGREGA LINEA
                    foreach (var line in value.Lines.Where(x => x.Record == 1))
                    {
                        purchaseRequest.Lines.Add();
                        if (value.DocType == "I") purchaseRequest.Lines.ItemCode = line.ItemCode;
                        purchaseRequest.Lines.ItemDescription = line.Dscription;
                        purchaseRequest.Lines.LineVendor = line.LineVendor;
                        purchaseRequest.Lines.RequiredDate = line.PqtReqDate;
                        purchaseRequest.Lines.AccountCode = line.AcctCode;
                        purchaseRequest.Lines.CostingCode = line.OcrCode;
                        if (value.DocType == "I") purchaseRequest.Lines.WarehouseCode = line.WhsCode;
                        if (value.DocType == "I") purchaseRequest.Lines.Quantity = (double)line.Quantity;
                        if (line.U_tipoOpT12 != null || line.U_tipoOpT12 == "") purchaseRequest.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        if (line.U_FF_TIP_COM != null || line.U_FF_TIP_COM == "") purchaseRequest.Lines.UserFields.Fields.Item("U_FF_TIP_COM").Value = line.U_FF_TIP_COM;
                    }

                    // EXISTE: SE MODIFICA LINEA
                    foreach (var line in value.Lines.Where(x => x.Record == 2 && x.LineStatus == "O"))
                    {
                        for (int i = 0; i < purchaseRequest.Lines.Count; i++)
                        {
                            purchaseRequest.Lines.SetCurrentLine(i);
                            if (purchaseRequest.Lines.LineNum == line.LineNum)
                            {
                                if (value.DocType == "I") purchaseRequest.Lines.ItemCode = line.ItemCode;
                                purchaseRequest.Lines.ItemDescription = line.Dscription;
                                purchaseRequest.Lines.LineVendor = line.LineVendor;
                                purchaseRequest.Lines.RequiredDate = line.PqtReqDate;
                                purchaseRequest.Lines.AccountCode = line.AcctCode;
                                purchaseRequest.Lines.CostingCode = line.OcrCode;
                                if (value.DocType == "I") purchaseRequest.Lines.WarehouseCode = line.WhsCode;
                                if (value.DocType == "I") purchaseRequest.Lines.Quantity = (double)line.Quantity;
                                if (line.U_tipoOpT12 != null || line.U_tipoOpT12 == "") purchaseRequest.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                                if (line.U_FF_TIP_COM != null || line.U_FF_TIP_COM == "") purchaseRequest.Lines.UserFields.Fields.Item("U_FF_TIP_COM").Value = line.U_FF_TIP_COM;
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

                    var reg = purchaseRequest.Update();

                    if (reg == 0)
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

        public async Task<ResultadoTransaccionEntity<PurchaseRequestEntity>> SetClose(PurchaseRequestCloseEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PurchaseRequestEntity>
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

                    purchaseRequest = company.GetBusinessObject(BoObjectTypes.oPurchaseRequest);

                    if (!purchaseRequest.GetByKey(value.DocEntry))
                    {
                        throw new Exception("No existe la solictud de compra.");
                    }

                    purchaseRequest.UserFields.Fields.Item("U_UsrUpdate").Value = value.U_UsrUpdate;

                    var regUpdate = purchaseRequest.Update();

                    if (regUpdate == 0)
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
    }
}
