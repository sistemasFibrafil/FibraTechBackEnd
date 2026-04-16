using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class PaymentTermsTypesRepository : RepositoryBase<PaymentTermsTypesEntity>, IPaymentTermsTypesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;


        public PaymentTermsTypesRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }



        public async Task<ResultadoTransaccionEntity<PaymentTermsTypesEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PaymentTermsTypesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.PaymentTermsTypes
                .AsNoTracking()
                .Select(n => new PaymentTermsTypesEntity
                {
                    GroupNum = n.GroupNum,
                    PymntGroup = n.PymntGroup
                }
                )
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

        public async Task<ResultadoTransaccionEntity<PaymentTermsTypesEntity>> GetByCode(short groupNum)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PaymentTermsTypesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.PaymentTermsTypes
                .AsNoTracking()
                .Where(n => n.GroupNum == groupNum)
                .Select(n => new PaymentTermsTypesEntity
                {
                    GroupNum = n.GroupNum,
                    PymntGroup = n.PymntGroup,
                    ExtraMonth = n.ExtraMonth,
                    ExtraDays = n.ExtraDays
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
