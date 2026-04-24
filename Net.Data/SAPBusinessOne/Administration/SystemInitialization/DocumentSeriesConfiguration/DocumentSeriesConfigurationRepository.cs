using System;
using SAPbobsCOM;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Find;
using Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Query;
using Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create;
using Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Entities;
namespace Net.Data.SAPBusinessOne.Administration
{
    public class DocumentSeriesConfigurationRepository : RepositoryBase<DocumentSeriesConfigurationEntity>, IDocumentSeriesConfigurationRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSeguridad _dbSeguridad;
        private readonly DataContextSAPBusinessOne _dbSAPBusinessOne;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        public DocumentSeriesConfigurationRepository(IConnectionSQL context, DataContextSAPBusinessOne dbSAPBusinessOne, DataContextSeguridad dbSeguridad, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _dbSeguridad = dbSeguridad;
            _dbSAPBusinessOne = dbSAPBusinessOne;
            _companyProviderSap = companyProviderSap;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionResponse<DocumentSeriesConfigurationQueryEntity>> GetById(DocumentSeriesConfigurationFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DocumentSeriesConfigurationQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var user = await _dbSeguridad.Usuario
                .Where(n => n.IdUsuario == value.IdUsuario)
                .Select(n => new UsuarioEntity
                {
                    IdUsuario = n.IdUsuario,
                    Usuario = n.Usuario,
                    Nombre = n.Nombre,
                    ApellidoPaterno = n.ApellidoPaterno,
                    ApellidoMaterno = n.ApellidoMaterno
                })
                .FirstOrDefaultAsync();


                var data = await _dbSAPBusinessOne.DocumentSeriesConfiguration
                .Where(n => n.Code == user.Usuario)
                .Select(n => new DocumentSeriesConfigurationQueryEntity
                {
                    Code = n.Code,
                    U_IdUser = n.U_IdUser,
                    U_Active = n.U_Active == "Y",

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines.Select(s => new DocumentSeriesConfiguration1QueryEntity
                    {
                        Code = s.Code,
                        LineId = s.LineId,
                        U_Type = s.U_Type,
                        U_Series = s.U_Series,
                        U_SalesInvoices = s.U_SalesInvoices == "Y",
                        U_Delivery = s.U_Delivery == "Y",
                        U_Transfer = s.U_Transfer == "Y",
                        U_Default = s.U_Default == "Y",
                        U_Active = s.U_Active == "Y",
                    }).ToList()
                })
                .FirstOrDefaultAsync();

                data ??= new DocumentSeriesConfigurationQueryEntity
                {
                    Code = user.Usuario,
                    U_IdUser = user.IdUsuario,
                    U_Active = true,
                    Lines = new List<DocumentSeriesConfiguration1QueryEntity>()
                };

                data.Nombre = user.Nombre;
                data.ApellidoPaterno = user.ApellidoPaterno;
                data.ApellidoMaterno = user.ApellidoMaterno;

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
        public async Task<ResultadoTransaccionResponse<DocumentSeriesConfigurationEntity>> SetCreate(DocumentSeriesConfigurationCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<DocumentSeriesConfigurationEntity>
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
                company = _companyProviderSap.GetCompany();

                oCompService = company.GetCompanyService();
                oGeneralService = oCompService.GetGeneralService("FIB_OCSD");
                

                if (await _dbSAPBusinessOne.DocumentSeriesConfiguration.AnyAsync(n => n.Code == value.Code))
                {
                    oGeneralDataParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                    oGeneralDataParams.SetProperty("Code", value.Code);
                    oGeneralData = oGeneralService.GetByParams(oGeneralDataParams);
                }
                else
                {
                    oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                    oGeneralData.SetProperty("Code", value.Code);
                }


                oGeneralData.SetProperty("U_IdUser", value.U_IdUser);
                oGeneralData.SetProperty("U_Active", value.U_Active);


                oGeneralDataCollection = oGeneralData.Child("FIB_CSD1");

                #region <<< NUEVO >>>

                foreach (var line in (value.Lines ?? Enumerable.Empty<DocumentSeriesConfigurationLinesCreateEntity>()).Where(x => x.Record == 1))
                {
                    var oGeneralDataLine = oGeneralDataCollection.Add();
                    oGeneralDataLine.SetProperty("U_Type", line.U_Type);
                    oGeneralDataLine.SetProperty("U_Series", line.U_Series);
                    oGeneralDataLine.SetProperty("U_SalesInvoices", line.U_SalesInvoices);
                    oGeneralDataLine.SetProperty("U_Delivery", line.U_Delivery);
                    oGeneralDataLine.SetProperty("U_Transfer", line.U_Transfer);
                    oGeneralDataLine.SetProperty("U_Default", line.U_Default);
                    oGeneralDataLine.SetProperty("U_Active", line.U_Active);
                }

                #endregion


                #region <<< EDITAR >>>

                foreach (var line in (value.Lines ?? Enumerable.Empty<DocumentSeriesConfigurationLinesCreateEntity>()).Where(x => x.Record == 3))
                {
                    var indice = oGeneralDataCollection.Cast<GeneralData>().ToList().FindIndex(x => (int)x.GetProperty("LineId") == line.LineId);
                    if (indice != -1)
                    {
                        var oGeneralDataLine = oGeneralDataCollection.Item(indice);
                        oGeneralDataLine.SetProperty("U_Type", line.U_Type);
                        oGeneralDataLine.SetProperty("U_Series", line.U_Series);
                        oGeneralDataLine.SetProperty("U_SalesInvoices", line.U_SalesInvoices);
                        oGeneralDataLine.SetProperty("U_Delivery", line.U_Delivery);
                        oGeneralDataLine.SetProperty("U_Transfer", line.U_Transfer);
                        oGeneralDataLine.SetProperty("U_Default", line.U_Default);
                        oGeneralDataLine.SetProperty("U_Active", line.U_Active);
                    }
                }

                #endregion


                #region <<< ELIMINAR >>>

                foreach (var line in (value.Lines ?? Enumerable.Empty<DocumentSeriesConfigurationLinesCreateEntity>()).Where(x => x.Record == 4))
                {
                    var indice = oGeneralDataCollection.Cast<GeneralData>().ToList().FindIndex(x => (int)x.GetProperty("LineId") == line.LineId);
                    if (indice != -1)
                    {
                        oGeneralDataCollection.Remove(indice);
                    }
                }

                #endregion


                if (await _dbSAPBusinessOne.DocumentSeriesConfiguration.AnyAsync(n => n.Code == value.Code))
                {
                    oGeneralService.Update(oGeneralData);
                }
                else
                {
                    oGeneralService.Add(oGeneralData);
                }


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Registro procesado con éxito.";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(oGeneralData, oGeneralDataCollection, oGeneralService, oCompService);
            }

            return await Task.FromResult(resultTransaccion);
        }
    }
}
