using System;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using SAPbobsCOM;
using Net.Connection.ConnectionSAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class AddressesRepository : RepositoryBase<AddressesEntity>, IAddressesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;


        public AddressesRepository(IConnectionSQL context, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _companyProviderSap = companyProviderSap;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionResponse<AddressesEntity>> GetListByCode(AddressesEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<AddressesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.Addresses
                .AsNoTracking()
                .Where(n=>n.AdresType == value.AdresType && n.CardCode == value.CardCode)
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
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<AddressesQueryEntity>> GetByCode(AddressesEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<AddressesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.Addresses
                .AsNoTracking()
                .Where(n => n.AdresType == value.AdresType && n.CardCode == value.CardCode && n.Address == value.Address)
                .Select(n => new AddressesQueryEntity
                {
                    Address = n.Address,
                    CardCode = n.CardCode,
                    Street = n.Street,
                    LineNum = n.LineNum,
                    AdresType = n.AdresType,
                    TaxCode = n.TaxCode,
                    FullAddress = $"{ Utilidades.ToSapCase(n.Street) } {n.States.Name} {n.County} - { Utilidades.ToSapCase(n.City) }"
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
        public async Task<ResultadoTransaccionEntity<AddressesEntity>> SetCreate(AddressesEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<AddressesEntity>
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

                    // Agregar nueva dirección
                    bp.Addresses.AddressName = value.Address;
                    bp.Addresses.AddressType = value.AdresType == "S" ? BoAddressType.bo_ShipTo : BoAddressType.bo_BillTo;
                    bp.Addresses.Street = value.Street;

                    int reg = bp.Update();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = $"Dirección {value.Address} agregada correctamente.";
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

        public async Task<ResultadoTransaccionEntity<AddressesEntity>> SetUpdate(AddressesEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<AddressesEntity>
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

                    // Buscar y actualizar dirección
                    bool found = false;
                    for (int i = 0; i < bp.Addresses.Count; i++)
                    {
                        bp.Addresses.SetCurrentLine(i);
                        if (bp.Addresses.AddressName == value.Address)
                        {
                            found = true;
                            if (!string.IsNullOrEmpty(value.Street)) bp.Addresses.Street = value.Street;
                            break;
                        }
                    }

                    if (!found)
                    {
                        throw new Exception($"No se encontró la dirección {value.Address}");
                    }

                    int reg = bp.Update();

                    if (reg == 0)
                    {
                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = $"Dirección {value.Address} actualizada correctamente.";
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

        public async Task<ResultadoTransaccionEntity<AddressesEntity>> SetDelete(string cardCode, string address)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<AddressesEntity>
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

                    if (!bp.GetByKey(cardCode))
                    {
                        company.GetLastError(out int errCode, out string errMsg);
                        throw new Exception($"No se encontró el socio de negocio {cardCode}. Error SAP {errCode}: {errMsg}");
                    }

                    // SAP DI-API no permite eliminar direcciones individualmente de forma directa vía bp.Addresses.Remove()
                    // pero se puede intentar buscar la línea y lanzarla.
                    throw new Exception("La eliminación de direcciones individuales no está soportada por SAP DI-API. Se debe realizar a través de la actualización del socio de negocio.");
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
    }
}
