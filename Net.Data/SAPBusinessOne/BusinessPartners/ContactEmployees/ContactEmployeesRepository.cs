using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
using SAPbobsCOM;
using Net.Connection.ConnectionSAPBusinessOne;
using Microsoft.Extensions.Configuration;
namespace Net.Data.SAPBusinessOne
{
    public class ContactEmployeesRepository : RepositoryBase<ContactEmployeesEntity>, IContactEmployeesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        public ContactEmployeesRepository(IConnectionSQL context, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _companyProviderSap = companyProviderSap;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> GetListByFilter(ContactEmployeesFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ContactEmployeesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.ContactEmployees
                .AsNoTracking()
                .Where(n => n.CardCode == value.CardCode);


                // FILTRO DE TEXTO
                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.Name, $"%{filter}%") ||
                        EF.Functions.Like(n.FirstName, $"%{filter}%") ||
                        EF.Functions.Like(n.MiddleName, $"%{filter}%") ||
                        EF.Functions.Like(n.LastName, $"%{filter}%")
                    );
                }


                var list = await query
                .Select(n => new ContactEmployeesQueryEntity
                {
                    CntctCode = n.CntctCode,
                    CardCode = n.CardCode,
                    Name = n.Name,
                    FullName = (n.FirstName + " " + n.MiddleName + " " + n.LastName) ?? n.Name ?? string.Empty
                })
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
        public async Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> GetById(ContactEmployeesFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ContactEmployeesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.ContactEmployees
                .AsNoTracking()
                .Where(n => n.CardCode == value.CardCode && n.CntctCode == value.CntctCode)
                .Select(n => new ContactEmployeesQueryEntity
                {
                    CntctCode = n.CntctCode,
                    CardCode = n.CardCode,
                    Name = n.Name,
                    FullName = (n.FirstName + " " + n.MiddleName + " " + n.LastName) ?? n.Name ?? string.Empty
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
        public async Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> GetByCode(ContactEmployeesFindEntity value)
        {
            return await GetById(value);
        }

        public async Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> GetByCardCode(string cardCode)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ContactEmployeesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.ContactEmployees
                .AsNoTracking()
                .Where(n => n.CardCode == cardCode)
                .Select(n => new ContactEmployeesQueryEntity
                {
                    CntctCode = n.CntctCode,
                    CardCode = n.CardCode,
                    Name = n.Name,
                    FullName = (n.FirstName + " " + n.MiddleName + " " + n.LastName) ?? n.Name ?? string.Empty
                })
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

        public async Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> SetCreate(ContactEmployeesEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ContactEmployeesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            BusinessPartners bp = null;

            return await Task.Run(() =>
            {
                try
                {
                    var company = _companyProviderSap.GetCompany();
                    bp = company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                    if (!bp.GetByKey(value.CardCode))
                    {
                        company.GetLastError(out int errCode, out string errMsg);
                        throw new Exception($"No se encontró el socio de negocio {value.CardCode}. Error SAP {errCode}: {errMsg}");
                    }

                    // Agregar nuevo contacto
                    bp.ContactEmployees.Add();
                    bp.ContactEmployees.Name = value.Name;
                    bp.ContactEmployees.FirstName = value.FirstName;
                    bp.ContactEmployees.MiddleName = value.MiddleName;
                    bp.ContactEmployees.LastName = value.LastName;
                    bp.ContactEmployees.Position = value.Position;
                    bp.ContactEmployees.Phone1 = value.Tel1;
                    bp.ContactEmployees.MobilePhone = value.Cellolar;
                    bp.ContactEmployees.E_Mail = value.E_MailL;

                    int reg = bp.Update();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = $"Contacto {value.Name} agregado correctamente.";
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
                    _companyProviderSap.LiberarObjetosCOM(bp);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> SetUpdate(ContactEmployeesEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ContactEmployeesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            BusinessPartners bp = null;

            return await Task.Run(() =>
            {
                try
                {
                    var company = _companyProviderSap.GetCompany();
                    bp = company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                    if (!bp.GetByKey(value.CardCode))
                    {
                        company.GetLastError(out int errCode, out string errMsg);
                        throw new Exception($"No se encontró el socio de negocio {value.CardCode}. Error SAP {errCode}: {errMsg}");
                    }

                    // Buscar y actualizar contacto
                    bool found = false;
                    for (int i = 0; i < bp.ContactEmployees.Count; i++)
                    {
                        bp.ContactEmployees.SetCurrentLine(i);
                        // Sincronizamos por CntctCode o por Name si CntctCode es 0
                        if ((value.CntctCode > 0 && i == value.CntctCode) || (bp.ContactEmployees.Name == value.Name))
                        {
                            found = true;
                            if (!string.IsNullOrEmpty(value.FirstName)) bp.ContactEmployees.FirstName = value.FirstName;
                            if (!string.IsNullOrEmpty(value.MiddleName)) bp.ContactEmployees.MiddleName = value.MiddleName;
                            if (!string.IsNullOrEmpty(value.LastName)) bp.ContactEmployees.LastName = value.LastName;
                            if (!string.IsNullOrEmpty(value.Position)) bp.ContactEmployees.Position = value.Position;
                            if (!string.IsNullOrEmpty(value.Tel1)) bp.ContactEmployees.Phone1 = value.Tel1;
                            if (!string.IsNullOrEmpty(value.Cellolar)) bp.ContactEmployees.MobilePhone = value.Cellolar;
                            if (!string.IsNullOrEmpty(value.E_MailL)) bp.ContactEmployees.E_Mail = value.E_MailL;
                            break;
                        }
                    }

                    if (!found)
                    {
                        throw new Exception($"No se encontró el contacto {value.Name}");
                    }

                    int reg = bp.Update();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = $"Contacto {value.Name} actualizado correctamente.";
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
                    _companyProviderSap.LiberarObjetosCOM(bp);
                }

                return resultTransaccion;
            });
        }

        public async Task<ResultadoTransaccionEntity<ContactEmployeesQueryEntity>> SetDelete(int cntctCode)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ContactEmployeesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            return await Task.Run(() =>
            {
                try
                {
                    throw new Exception("La eliminación de contactos individuales no está soportada por SAP DI-API. Se debe realizar a través de la actualización del socio de negocio.");
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message;
                }

                return resultTransaccion;
            });
        }
    }
}
