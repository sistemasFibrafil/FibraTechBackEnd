using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net.Business.Entities;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
namespace Net.Data.SAPBusinessOne
{
    public class AddressesRepository : RepositoryBase<AddressesEntity>, IAddressesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;


        public AddressesRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<AddressesEntity>> GetListByCode(AddressesEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<AddressesEntity>
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

        public async Task<ResultadoTransaccionEntity<AddressesQueryEntity>> GetByCode(AddressesEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<AddressesQueryEntity>
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
    }
}
