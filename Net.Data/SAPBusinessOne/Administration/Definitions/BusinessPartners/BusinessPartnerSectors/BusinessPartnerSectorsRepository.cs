using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;

namespace Net.Data.SAPBusinessOne
{
    public class BusinessPartnerSectorsRepository : RepositoryBase<BusinessPartnerSectorsEntity>, IBusinessPartnerSectorsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");
        private readonly DataContextSAPBusinessOne _db;

        public BusinessPartnerSectorsRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionResponse<BusinessPartnerSectorsEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionResponse<BusinessPartnerSectorsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.BusinessPartnerSectors
                .AsNoTracking()
                .OrderBy(x => x.Codigo)
                .ToListAsync();

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
    }
}
