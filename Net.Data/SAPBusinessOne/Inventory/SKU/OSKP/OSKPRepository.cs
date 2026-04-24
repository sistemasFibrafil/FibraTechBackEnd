using System;
using AutoMapper;
using SAPbobsCOM;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    /// <summary>
    /// Repositorio para interactuar con la tabla de usuario OSKP y el objeto general de SAP.
    /// </summary>
    public class OSKPRepository : RepositoryBase<OSKPEntity>, IOSKPRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IMapper _ma;
        private readonly DataContextSAPBusinessOne _dc;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        // --- Constantes para evitar "magic strings" y facilitar el mantenimiento ---
        private const string SapGeneralServiceName = "FIB_OSKP";
        private const string SapChildCollectionName = "FIB_SKP1";
        private const string SapDiApiConnectionConfig = "EntornoConnectionSapDiApi:Entorno";

        public OSKPRepository(IConnectionSQL context, IConfiguration configuration, DataContextSAPBusinessOne dc, IMapper ma, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _dc = dc;
            _ma = ma;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }

        /// <summary>
        /// Asigna las propiedades de la entidad al objeto GeneralData de SAP.
        /// </summary>
        private void SetGeneralDataProperties(GeneralData oGeneralData, OSKPEntity value)
        {
            oGeneralData.SetProperty("U_Number", value.U_Number);
            oGeneralData.SetProperty("U_ItemCode", value.U_ItemCode);
            oGeneralData.SetProperty("U_PrdStrDate", value.U_PrdStrDate);
            oGeneralData.SetProperty("U_PrdEndDate", value.U_PrdEndDate);
            oGeneralData.SetProperty("U_PrdEndHour", value.U_PrdEndHour);
            oGeneralData.SetProperty("U_RollWeight", Convert.ToDouble(Math.Round(value.U_RollWeight, 6)));
            if (value.U_PrdForDetail != null) oGeneralData.SetProperty("U_PrdForDetail", value.U_PrdForDetail);
            if (value.U_PrdPresBale != null) oGeneralData.SetProperty("U_PrdPresBale", value.U_PrdPresBale);
            if (value.U_PrdFeaYes != null) oGeneralData.SetProperty("U_PrdFeaYes", value.U_PrdFeaYes);
            if (value.U_PrdFeaNo != null) oGeneralData.SetProperty("U_PrdFeaNo", value.U_PrdFeaNo);
            if (value.U_PrdFeaObs != null) oGeneralData.SetProperty("U_PrdFeaObs", value.U_PrdFeaObs);
            if (value.U_FeaQuaInd != null) oGeneralData.SetProperty("U_FeaQuaInd", value.U_FeaQuaInd);
            if (value.U_FeaQuaJus != null) oGeneralData.SetProperty("U_FeaQuaJus", value.U_FeaQuaJus);
            if (value.U_CosStrDate != null) oGeneralData.SetProperty("U_CosStrDate", value.U_CosStrDate);
            if (value.U_CosEndDate != null) oGeneralData.SetProperty("U_CosEndDate", value.U_CosEndDate);
            if (value.U_CosEndHour != null) oGeneralData.SetProperty("U_CosEndHour", value.U_CosEndHour);
            if (value.U_CosDetail != null) oGeneralData.SetProperty("U_CosDetail", value.U_CosDetail);
        }

        // --- MÉTODOS PÚBLICOS OPTIMIZADOS ---

        /// <summary>
        /// Crea un nuevo registro OSKP y un nuevo artículo en SAP.
        /// </summary>
        public Task<ResultadoTransaccionResponse<OSKPEntity>> SetCreate(OSKPEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OSKPEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Items oItem = null;
            Company company = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            GeneralService oGeneralService = null;
            GeneralDataCollection oGeneralDataCollection = null;

            try
            {
                // Conexión a SAP
                company = _companyProviderSap.GetCompany();


                // Empieza la transacción en SAP
                if (!company.InTransaction) company.StartTransaction();


                oCompService = company.GetCompanyService();
                oGeneralService = oCompService.GetGeneralService(SapGeneralServiceName);
                oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                SetGeneralDataProperties(oGeneralData, value);

                oGeneralDataCollection = oGeneralData.Child(SapChildCollectionName);

                foreach (var line in value.Line)
                {
                    var oGeneralDataLine = oGeneralDataCollection.Add();
                    oGeneralDataLine.SetProperty("U_ProcessCode", line.U_ProcessCode);
                    oGeneralDataLine.SetProperty("U_Percentage1", Convert.ToDouble(Math.Round(line.U_Percentage1, 2)));
                    oGeneralDataLine.SetProperty("U_ItemCode", line.U_ItemCode);
                    oGeneralDataLine.SetProperty("U_Percentage2", Convert.ToDouble(Math.Round(line.U_Percentage2, 2)));
                }
                oGeneralService.Add(oGeneralData);

                var oSKC = _dc.OSKCView.FirstOrDefault(x => x.Code == value.U_Number);
                if (oSKC == null)
                {
                    throw new Exception($"No se encontró el SKU con código {value.U_Number}");
                }

                oItem = (Items)company.GetBusinessObject(BoObjectTypes.oItems);
                if (!oItem.GetByKey(value.U_ItemCode))
                {
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
                    if (oItem.Add() != 0)
                    {
                        company.GetLastError(out int errorCode, out string errorMessage);
                        throw new Exception($"No se pudo crear el artículo. {errorMessage} {errorCode}");
                    }
                }

                // Se finaliza la transacción en SAP
                if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_Commit);


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Registro procesado con éxito.";
            }
            catch (Exception ex)
            {
                if (company.Connected)
                {
                    if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_RollBack);
                }

                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(oItem, oGeneralData, oGeneralDataCollection, oGeneralService, oCompService);
            }

            return Task.FromResult(resultTransaccion);
        }

        /// <summary>
        /// Actualiza un registro OSKP existente en SAP.
        /// </summary>
        public Task<ResultadoTransaccionResponse<OSKPEntity>> SetUpdate(OSKPEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OSKPEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Company company = null;
            GeneralData oGeneralData = null;
            CompanyService oCompService = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralDataParams = null;
            GeneralDataCollection oGeneralDataCollection = null;

            try
            {
                // Conexión a SAP
                company = _companyProviderSap.GetCompany();


                // Empieza la transacción en SAP
                if (!company.InTransaction) company.StartTransaction();

                oCompService = company.GetCompanyService();
                oGeneralService = oCompService.GetGeneralService(SapGeneralServiceName);
                oGeneralDataParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGeneralDataParams.SetProperty("DocEntry", value.DocEntry);
                oGeneralData = oGeneralService.GetByParams(oGeneralDataParams);

                SetGeneralDataProperties(oGeneralData, value);

                oGeneralDataCollection = oGeneralData.Child(SapChildCollectionName);

                foreach (var line in value.Line)
                {
                    var indice = oGeneralDataCollection.Cast<GeneralData>().ToList().FindIndex(x => (int)x.GetProperty("LineId") == line.LineId);
                    if (indice != -1)
                    {
                        var oGeneralDataLine = oGeneralDataCollection.Item(indice);
                        oGeneralDataLine.SetProperty("U_ProcessCode", line.U_ProcessCode);
                        oGeneralDataLine.SetProperty("U_Percentage1", Convert.ToDouble(Math.Round(line.U_Percentage1, 2)));
                        oGeneralDataLine.SetProperty("U_ItemCode", line.U_ItemCode);
                        oGeneralDataLine.SetProperty("U_Percentage2", Convert.ToDouble(Math.Round(line.U_Percentage2, 2)));
                    }
                }
                oGeneralService.Update(oGeneralData);


                // Se finaliza la transacción en SAP
                if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_Commit);


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Registro actualizado con éxito.";
            }
            catch (Exception ex)
            {
                if (company.Connected)
                {
                    if (company.InTransaction) company.EndTransaction(BoWfTransOpt.wf_RollBack);
                }

                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.StackTrace.ToString();
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(oGeneralData, oGeneralDataParams, oGeneralDataCollection, oGeneralService, oCompService);
            }

            return Task.FromResult(resultTransaccion);
        }

        /// <summary>
        /// Elimina un registro OSKP de SAP.
        /// </summary>
        public Task<ResultadoTransaccionResponse<OSKPEntity>> SetDelete(OSKPEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OSKPEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            CompanyService oCompService = null;
            GeneralService oGeneralService = null;
            GeneralDataParams oGeneralParams = null;

            try
            {
                // Conexión a SAP
                var company = _companyProviderSap.GetCompany();


                oCompService = company.GetCompanyService();
                oGeneralService = oCompService.GetGeneralService(SapGeneralServiceName);
                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGeneralParams.SetProperty("DocEntry", value.DocEntry);
                oGeneralService.Delete(oGeneralParams);

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Registro eliminado con éxito.";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.StackTrace.ToString();
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(oGeneralParams, oGeneralService, oCompService);
            }

            return Task.FromResult(resultTransaccion);
        }

        /// <summary>
        /// Obtiene una lista de registros OSKP filtrados por nombre de artículo o cliente.
        /// </summary>
        public async Task<ResultadoTransaccionResponse<OSKPEntity>> GetListByFiltro(OSKPEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OSKPEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var filtro = value.Filtro ?? "";
                var lista = await _dc.OSKPView.Where(x => x.U_ItemName.Contains(filtro) || x.U_CardName.Contains(filtro)).ToListAsync();

                if (!lista.Any())
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = "No se encontraron registros con el filtro especificado.";
                    return resultTransaccion;
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {lista.Count}";
                resultTransaccion.dataList = _ma.Map<List<OSKPEntity>>(lista);
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
        /// Obtiene un registro OSKP por su DocEntry, incluyendo sus líneas.
        /// </summary>
        public async Task<ResultadoTransaccionResponse<OSKPEntity>> GetByDocEntry(OSKPEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OSKPEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _dc.OSKPView.FirstOrDefaultAsync(x => x.DocEntry == value.DocEntry);

                if (data == null)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = "No se encontró el registro.";
                    return resultTransaccion;
                }

                var lineas = await (from T01 in _dc.SKP1
                                    join T02 in _dc.Processes on T01.U_ProcessCode equals T02.Code
                                    join T03 in _dc.Items on T01.U_ItemCode equals T03.ItemCode
                                    where T01.DocEntry == value.DocEntry
                                    select new SKP1Entity
                                    {
                                        DocEntry = T01.DocEntry,
                                        LineId = T01.LineId,
                                        U_ProcessCode = T01.U_ProcessCode,
                                        U_ProcessName = T02.Name,
                                        U_Percentage1 = T01.U_Percentage1,
                                        U_ItemCode = T01.U_ItemCode,
                                        U_ItemName = T03.ItemName,
                                        U_Percentage2 = T01.U_Percentage2,
                                    }).ToListAsync();

                data.Line = lineas;

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = _ma.Map<OSKPEntity>(data);
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