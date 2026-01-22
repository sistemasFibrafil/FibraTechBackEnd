using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net.Business.Entities;
using Net.Business.Entities.Sap;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
namespace Net.Data.Sap
{
    public class TaxGroupsRepository : RepositoryBase<TaxGroupsEntity>, ITaxGroupsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN  
        private readonly DataContextSap _db;


        public TaxGroupsRepository(IConnectionSQL context, DataContextSap db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<TaxGroupsEntity>> GetListByFilter(string filter)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TaxGroupsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.TaxGroups
                .AsNoTracking();

                // FILTRO POR CODIGO O DESCRIPCION
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    filter = filter.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(EF.Functions.Collate(x.Code!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.Name!, GlobalVariables.CI), $"%{filter}%")
                    );
                }

                var list = await query
                .OrderBy(x => x.Code)
                .Select(n => new TaxGroupsEntity
                {
                    Code = n.Code,
                    Name = n.Name,
                    Rate = n.Rate,
                })
                .ToListAsync();

                //var list = await _db.TaxGroups.Where(x=>x.Name.ToUpper().Contains(filter)).ToListAsync();

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

        public async Task<ResultadoTransaccionEntity<TaxGroupsEntity>> GetBySplCode(int slpCode)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TaxGroupsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.Set<TaxGroupsEntity>().FromSqlRaw("EXEC GES_GetImpuestoBySlpCode @SlpCode = {0}", slpCode).FirstOrDefaultAsync();

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
    }
}
