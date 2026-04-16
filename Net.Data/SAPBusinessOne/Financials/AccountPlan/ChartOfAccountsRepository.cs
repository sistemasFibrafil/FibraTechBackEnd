using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class ChartOfAccountsRepository : RepositoryBase<ChartOfAccountsEntity>, IChartOfAccountsRepository
    {
        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;

        public ChartOfAccountsRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
        }


        public async Task<ResultadoTransaccionEntity<ChartOfAccountsQueryEntity>> GetListByFilter(ChartOfAccountsFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<ChartOfAccountsQueryEntity>();

            try
            {
                var query = _db.ChartOfAccounts
                .AsNoTracking()
                .Where(n => n.Postable == "Y");


                // FILTRO POR CUENTA CONTABLE
                if (!string.IsNullOrWhiteSpace(value.AccountingAccount))
                {
                    var filter = value.AccountingAccount.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(EF.Functions.Collate(x.Segment_0 + "-" + x.Segment_1 + "-" + x.Segment_2, GlobalVariables.CI),$"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.AcctName!, GlobalVariables.CI), $"%{filter}%")
                    );
                }


                var list = await query
                .OrderBy(x => x.Segment_0)
                .ThenBy(x => x.Segment_1)
                .ThenBy(x => x.Segment_2)
                .Select(n => new ChartOfAccountsQueryEntity
                {
                    AcctCode = n.AcctCode,
                    FormatCode = n.Segment_0 + "-" + n.Segment_1 + "-" + n.Segment_2,
                    AcctName = n.AcctName
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
