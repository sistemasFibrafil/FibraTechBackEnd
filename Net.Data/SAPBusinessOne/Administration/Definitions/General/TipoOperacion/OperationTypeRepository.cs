using System;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class OperationTypeRepository : RepositoryBase<OperationTypeEntity>, IOperationTypeRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN  
        private readonly DataContextSAPBusinessOne _db;


        public OperationTypeRepository(IConnectionSQL context, DataContextSAPBusinessOne dc)
            : base(context)
        {
            _db = dc;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionResponse<OperationTypeEntity>> GetListByFilter(TipoOperacionFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OperationTypeEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.OperationType
                .AsNoTracking();


                // FILTRO POR CODIGO O DESCRIPCION
                if (!string.IsNullOrWhiteSpace(value.TipoOperacion))
                {
                    var filter = value.TipoOperacion.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(EF.Functions.Collate(x.Code!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.U_descrp!, GlobalVariables.CI), $"%{filter}%")
                    );
                }


                var list = await query
                .OrderBy(x => x.Code)
                .Select(n => new OperationTypeEntity
                {
                    Code = n.Code,
                    U_descrp = n.U_descrp
                })
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
