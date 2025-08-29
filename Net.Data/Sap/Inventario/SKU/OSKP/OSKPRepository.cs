using System;
using SAPbobsCOM;
using AutoMapper;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Net.Data.Sap
{
    public class OSKPRepository : RepositoryBase<OSKPEntity>, IOSKPRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IMapper _am;
        private readonly DataContextFil _db;
        private readonly IConnectionSap _conSap;
        private readonly ConnectionSapEntity _cnDIAPI;

        public OSKPRepository(IConnectionSQL context, IConfiguration configuration, DataContextFil db, IMapper am)
            : base(context)
        {
            _aplicacionName = GetType().Name;
            _conSap = new ConnectionSap();
            _cnDIAPI = Utilidades.GetConDiApiSap(configuration, "EntornoConnectionDiApiSap:Entorno");
            _db = db;
            _am = am;
        }

        public Task<ResultadoTransaccionEntity<OSKPEntity>> SetCreate(OSKPEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKPEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            SAPbobsCOM.GeneralData oGeneralData = null;
            SAPbobsCOM.CompanyService oCompService = null;
            SAPbobsCOM.GeneralService oGeneralService = null;

            SAPbobsCOM.GeneralDataCollection oGeneralDataCollection1 = null;
            SAPbobsCOM.GeneralDataCollection oGeneralDataCollection2 = null;

            SAPbobsCOM.GeneralData oGeneralDataLine = null;

            SAPbobsCOM.Items oItem = null;

            try
            {
                var rpta = _conSap.ConnectToCompany(_cnDIAPI);

                if (rpta == "0")
                {
                    if (RepositoryBaseSap.oCompany.Connected)
                    {
                        if (!RepositoryBaseSap.oCompany.InTransaction)
                        {
                            RepositoryBaseSap.oCompany.StartTransaction();
                        }
                    }

                    oCompService = RepositoryBaseSap.oCompany.GetCompanyService();

                    oGeneralService = oCompService.GetGeneralService("FIB_OSKP");
                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralData.SetProperty("U_Number", value.U_Number);
                    oGeneralData.SetProperty("U_PrdStrDate", value.U_PrdStrDate);
                    oGeneralData.SetProperty("U_PrdEndDate", value.U_PrdEndDate);
                    oGeneralData.SetProperty("U_PrdEndHour", value.U_PrdEndHour);
                    oGeneralData.SetProperty("U_RollWeight", Convert.ToDouble(Math.Round(value.U_RollWeight, 6)));
                    oGeneralData.SetProperty("U_PrdForDetail", value.U_PrdForDetail);
                    oGeneralData.SetProperty("U_PrdPresBale", value.U_PrdPresBale);
                    oGeneralData.SetProperty("U_PrdFeaYes", value.U_PrdFeaYes);
                    oGeneralData.SetProperty("U_PrdFeaNo", value.U_PrdFeaNo);
                    oGeneralData.SetProperty("U_PrdFeaObs", value.U_PrdFeaObs);
                    oGeneralData.SetProperty("U_FeaQuaInd", value.U_FeaQuaInd);
                    oGeneralData.SetProperty("U_FeaQuaJus", value.U_FeaQuaJus);
                    if (value.U_CosStrDate != null)
                    {
                        oGeneralData.SetProperty("U_CosStrDate", value.U_CosStrDate);
                    }
                    if (value.U_CosEndDate != null)
                    {
                        oGeneralData.SetProperty("U_CosEndDate", value.U_CosEndDate);
                    }
                    if(value.U_CosEndHour != null)
                    {
                        oGeneralData.SetProperty("U_CosEndHour", Convert.ToDateTime(value.U_CosEndHour).ToString("yyyy-MM-ddTHH:mm:ss"));
                    }
                    oGeneralData.SetProperty("U_CosDetail", value.U_CosDetail);
                    oGeneralData.SetProperty("U_ValExcMar", value.U_ValExcMar);
                    oGeneralData.SetProperty("U_AprByExc", value.U_AprByExc);
                    oGeneralData.SetProperty("U_Observations", value.U_Observations);
                    oGeneralData.SetProperty("U_ItemCode", value.U_ItemCode);

                    oGeneralDataCollection1 = (SAPbobsCOM.GeneralDataCollection)oGeneralData.Child("FIB_SKP1");
                                        
                    foreach (var line in value.Line)
                    {
                        oGeneralDataLine = oGeneralDataCollection1.Add();
                        oGeneralDataLine.SetProperty("U_ProcessCode", line.U_ProcessCode);
                        oGeneralDataLine.SetProperty("U_Percentage1", Convert.ToDouble(Math.Round(line.U_Percentage1, 2)));
                        oGeneralDataLine.SetProperty("U_ItemCode", line.U_ItemCode);
                        oGeneralDataLine.SetProperty("U_Percentage2", Convert.ToDouble(Math.Round(line.U_Percentage2, 2)));
                    }
                    oGeneralService.Add(oGeneralData);

                    var oSKC = _db.OSKCView.Where(x => x.Code == value.U_Number).FirstOrDefault();

                    oItem = (SAPbobsCOM.Items)RepositoryBaseSap.oCompany.GetBusinessObject(BoObjectTypes.oItems);
                    oItem.ItemCode = value.U_ItemCode;
                    oItem.ItemName = oSKC.U_ItemName;
                    oItem.ItemsGroupCode = oSKC.U_ItmsGrpCod;
                    oItem.InventoryUOM = oSKC.U_UnitMsrCode;
                    oItem.SalesUnit = oSKC.U_UnitMsrCode;
                    oItem.PurchaseUnit = oSKC.U_UnitMsrCode;
                    oItem.IssueMethod = BoIssueMethod.im_Backflush;
                    oItem.UserFields.Fields.Item("U_FIB_SGRUP").Value = oSKC.U_ItmsGrpNam;
                    oItem.UserFields.Fields.Item("U_FIB_PESO").Value = Convert.ToDouble(Math.Round(oSKC.U_ItemWeight, 6));
                    oItem.UserFields.Fields.Item("U_FIB_COLOR").Value = oSKC.U_ColorCode;
                    var reg = oItem.Add();

                    if(reg != 0)
                    {
                        if (RepositoryBaseSap.oCompany is not null)
                        {
                            if (RepositoryBaseSap.oCompany.Connected)
                            {
                                if (RepositoryBaseSap.oCompany.InTransaction)
                                {
                                    RepositoryBaseSap.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                                }
                            }
                        }

                        RepositoryBaseSap.oCompany.GetLastError(out int errorCode, out string errorMessage);
                        resultTransaccion.IdRegistro = -1;
                        resultTransaccion.ResultadoCodigo = errorCode;
                        resultTransaccion.ResultadoDescripcion = errorMessage;
                        return Task.FromResult(resultTransaccion);
                    }

                    if (RepositoryBaseSap.oCompany is not null)
                    {
                        if (RepositoryBaseSap.oCompany.Connected)
                        {
                            if (RepositoryBaseSap.oCompany.InTransaction)
                            {
                                RepositoryBaseSap.oCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                            }
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Registro procesado con éxito ..!";
                }
                else
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = rpta;
                }                
            }
            catch (Exception ex)
            {
                if (RepositoryBaseSap.oCompany is not null)
                {
                    if (RepositoryBaseSap.oCompany.Connected)
                    {
                        if (RepositoryBaseSap.oCompany.InTransaction)
                        {
                            RepositoryBaseSap.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                    }
                }

                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            finally
            {
                #pragma warning disable CA1416
                if (oGeneralData != null) Marshal.ReleaseComObject(oGeneralData);
                if (oCompService != null) Marshal.ReleaseComObject(oCompService);
                if (oGeneralService != null) Marshal.ReleaseComObject(oGeneralService);
                if (oGeneralDataCollection1 != null) Marshal.ReleaseComObject(oGeneralDataCollection1);
                if (oGeneralDataCollection2 != null) Marshal.ReleaseComObject(oGeneralDataCollection2);
                if (oGeneralDataLine != null) Marshal.ReleaseComObject(oGeneralDataLine);
                if (oItem != null) Marshal.ReleaseComObject(oItem);
                if (RepositoryBaseSap.oCompany is not null)
                {
                    if (RepositoryBaseSap.oCompany.Connected)
                    {
                        _conSap.DisConnectToCompany();
                        Marshal.ReleaseComObject(RepositoryBaseSap.oCompany);
                        RepositoryBaseSap.oCompany = null;
                    }
                }
                #pragma warning restore CA1416
            }

            return Task.FromResult(resultTransaccion);
        }

        public Task<ResultadoTransaccionEntity<OSKPEntity>> SetUpdate(OSKPEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKPEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            SAPbobsCOM.GeneralData oGeneralData = null;
            SAPbobsCOM.CompanyService oCompService = null;
            SAPbobsCOM.GeneralService oGeneralService = null;
            SAPbobsCOM.GeneralDataParams oGeneralDataParams = null;

            SAPbobsCOM.GeneralDataCollection oGeneralDataCollection = null;


            try
            {
                var rpta = _conSap.ConnectToCompany(_cnDIAPI);

                if (rpta == "0")
                {
                    oCompService = RepositoryBaseSap.oCompany.GetCompanyService();

                    oGeneralService = oCompService.GetGeneralService("FIB_OSKP");

                    oGeneralDataParams = (SAPbobsCOM.GeneralDataParams)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);

                    oGeneralDataParams.SetProperty("DocEntry", value.DocEntry);

                    oGeneralData = oGeneralService.GetByParams(oGeneralDataParams);

                    oGeneralData.SetProperty("U_Number", value.U_Number);
                    oGeneralData.SetProperty("U_PrdStrDate", value.U_PrdStrDate);
                    oGeneralData.SetProperty("U_PrdEndDate", value.U_PrdEndDate);
                    oGeneralData.SetProperty("U_PrdEndHour", value.U_PrdEndHour);
                    oGeneralData.SetProperty("U_RollWeight", Convert.ToDouble(Math.Round(value.U_RollWeight, 6)));
                    oGeneralData.SetProperty("U_PrdForDetail", value.U_PrdForDetail);
                    oGeneralData.SetProperty("U_PrdPresBale", value.U_PrdPresBale);
                    oGeneralData.SetProperty("U_PrdFeaYes", value.U_PrdFeaYes);
                    oGeneralData.SetProperty("U_PrdFeaNo", value.U_PrdFeaNo);
                    oGeneralData.SetProperty("U_PrdFeaObs", value.U_PrdFeaObs);
                    oGeneralData.SetProperty("U_FeaQuaInd", value.U_FeaQuaInd);
                    oGeneralData.SetProperty("U_FeaQuaJus", value.U_FeaQuaJus);
                    if (value.U_CosStrDate != null)
                    {
                        oGeneralData.SetProperty("U_CosStrDate", value.U_CosStrDate);
                    }
                    if (value.U_CosEndDate != null)
                    {
                        oGeneralData.SetProperty("U_CosEndDate", value.U_CosEndDate);
                    }
                    if (value.U_CosEndHour != null)
                    {
                        oGeneralData.SetProperty("U_CosEndHour", Convert.ToDateTime(value.U_CosEndHour).ToString("yyyy-MM-ddTHH:mm:ss"));
                    }
                    oGeneralData.SetProperty("U_CosDetail", value.U_CosDetail);
                    oGeneralData.SetProperty("U_ValExcMar", value.U_ValExcMar);
                    oGeneralData.SetProperty("U_AprByExc", value.U_AprByExc);
                    oGeneralData.SetProperty("U_Observations", value.U_Observations);
                    oGeneralData.SetProperty("U_ItemCode", value.U_ItemCode);

                    oGeneralDataCollection = (SAPbobsCOM.GeneralDataCollection)oGeneralData.Child("FIB_SKP1");

                    foreach (var line in value.Line)
                    {
                        var indice = oGeneralDataCollection.Cast<SAPbobsCOM.GeneralData>().ToList().FindIndex(x => (int)x.GetProperty("LineId") == line.LineId);

                        var oGeneralDataLine = oGeneralDataCollection.Item(indice);
                        oGeneralDataLine.SetProperty("U_ProcessCode", line.U_ProcessCode);
                        oGeneralDataLine.SetProperty("U_Percentage1", Convert.ToDouble(Math.Round(line.U_Percentage1, 2)));
                        oGeneralDataLine.SetProperty("U_ItemCode", line.U_ItemCode);
                        oGeneralDataLine.SetProperty("U_Percentage2", Convert.ToDouble(Math.Round(line.U_Percentage2, 2)));
                    }
                    oGeneralService.Update(oGeneralData);

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Registro procesado con éxito ..!";
                }
                else
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = rpta;
                }
            }
            catch (Exception ex)
            {
                if (RepositoryBaseSap.oCompany is not null)
                {
                    if (RepositoryBaseSap.oCompany.Connected)
                    {
                        _conSap.DisConnectToCompany();
                        RepositoryBaseSap.oCompany = null;
                    }
                }

                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            finally
            {
                 #pragma warning disable CA1416
                if (oGeneralData != null) Marshal.ReleaseComObject(oGeneralData);
                if (oCompService != null) Marshal.ReleaseComObject(oCompService);
                if (oGeneralService != null) Marshal.ReleaseComObject(oGeneralService);
                if (oGeneralDataCollection != null) Marshal.ReleaseComObject(oGeneralDataCollection);
                if (RepositoryBaseSap.oCompany is not null)
                {
                    if (RepositoryBaseSap.oCompany.Connected)
                    {
                        _conSap.DisConnectToCompany();
                        Marshal.ReleaseComObject(RepositoryBaseSap.oCompany);
                        RepositoryBaseSap.oCompany = null;
                    }
                }
                #pragma warning restore CA1416
            }

            return Task.FromResult(resultTransaccion);
        }

        public Task<ResultadoTransaccionEntity<OSKPEntity>> SetDelete(OSKPEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKPEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            SAPbobsCOM.CompanyService oCompService = null;
            SAPbobsCOM.GeneralService oGeneralService = null;
            SAPbobsCOM.GeneralDataParams oGeneralParams = null;

            try
            {
                var rpta = _conSap.ConnectToCompany(_cnDIAPI);

                if (rpta == "0")
                {
                    oCompService = RepositoryBaseSap.oCompany.GetCompanyService();
                    oGeneralService = oCompService.GetGeneralService("FIB_OSKP");

                    oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);

                    oGeneralParams.SetProperty("DocEntry", value.DocEntry);
                    oGeneralService.Delete(oGeneralParams);

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Registro eliminado con éxito ..!";
                }
                else
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = rpta;
                }
            }
            catch (Exception ex)
            {
                if (RepositoryBaseSap.oCompany is not null)
                {
                    if (RepositoryBaseSap.oCompany.Connected)
                    {
                        _conSap.DisConnectToCompany();
                        RepositoryBaseSap.oCompany = null;
                    }
                }

                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            finally
            {
                #pragma warning disable CA1416
                if (oCompService != null) Marshal.ReleaseComObject(oCompService);
                if (oGeneralService != null) Marshal.ReleaseComObject(oGeneralService);
                if (RepositoryBaseSap.oCompany is not null)
                {
                    if (RepositoryBaseSap.oCompany.Connected)
                    {
                        _conSap.DisConnectToCompany();
                        Marshal.ReleaseComObject(RepositoryBaseSap.oCompany);
                        RepositoryBaseSap.oCompany = null;
                    }
                }
                #pragma warning restore CA1416
            }

            return Task.FromResult(resultTransaccion);
        }

        public async Task<ResultadoTransaccionEntity<OSKPEntity>> GetListByFiltro(OSKPEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKPEntity>();

            try
            {
                var filtro = value.Filtro == null ? "" : value.Filtro.Trim();
                var lista = await _db.OSKPView.Where(x=>x.U_ItemName.Contains(filtro) || x.U_CardName.Contains(filtro)).ToListAsync();
                var response = _am.Map<List<OSKPEntity>>(lista);

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                resultTransaccion.dataList = response;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<OSKPEntity>> GetByDocEntry(OSKPEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<OSKPEntity>();

            try
            {
                var lista = await _db.OSKPView.Where(x => x.DocEntry == value.DocEntry).FirstOrDefaultAsync();
                var response = _am.Map<OSKPEntity>(lista);

                var line = from T01 in _db.SKP1
                           join T02 in _db.Proceso on T01.U_ProcessCode equals T02.Code
                           join T03 in _db.OITM on T01.U_ItemCode equals T03.ItemCode
                           where T01.DocEntry == value.DocEntry
                           select new SKP1Entity
                           {
                               DocEntry = T01.DocEntry,
                               LineId = T01.LineId,
                               U_ProcessCode = T01.U_ProcessCode,
                               U_ProcessName= T02.Name,
                               U_Percentage1 = T01.U_Percentage1,
                               U_ItemCode = T01.U_ItemCode,
                               U_ItemName = T03.ItemName,
                               U_Percentage2 = T01.U_Percentage2,
                           };

                response.Line = await line.ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = response;
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
}
