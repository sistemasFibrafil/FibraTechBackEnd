using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using Microsoft.EntityFrameworkCore;
namespace Net.Data.Sap
{
    public class CostCentersRepository : RepositoryBase<CostCentersEntity>, ICostCentersRepository
    {
        // PARAMETROS DE COXIÓN
        private readonly DataContextSap _db;

        public CostCentersRepository(IConnectionSQL context, DataContextSap db)
            : base(context)
        {
            _db = db;
        }


        public async Task<ResultadoTransaccionEntity<CostCentersEntity>> GetListByFilter(CostCentersFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<CostCentersEntity>();

            try
            {
                var query = _db.CostCenters
                .AsNoTracking()
                .Where(n => n.DimCode == 1);

                // FILTRO POR ACTIVO
                if (!string.IsNullOrWhiteSpace(value.Active))
                {
                    var active = value.Active.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(x => active.Contains(x.Active));
                }

                // FILTRO POR CENTRO DE COSTO
                if (!string.IsNullOrWhiteSpace(value.CostCenter))
                {
                    var filter = value.CostCenter.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(EF.Functions.Collate(x.OcrCode!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.OcrName!, GlobalVariables.CI), $"%{filter}%")
                    );
                }

                var list = await query
                .Select(x => new CostCentersEntity
                {
                    OcrCode = x.OcrCode,
                    OcrName = x.OcrName
                })
                .OrderBy(x => x.OcrCode)
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
