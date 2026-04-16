using System;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class TaxGroupsRepository : RepositoryBase<TaxGroupsEntity>, ITaxGroupsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN  
        private readonly DataContextSAPBusinessOne _db;


        public TaxGroupsRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
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

        public async Task<ResultadoTransaccionEntity<TaxGroupsEntity>> GetByCardCode(TaxGroupsFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<TaxGroupsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await
                (
                    from d in _db.Addresses
                    join tg in _db.TaxGroups on d.TaxCode equals tg.Code into tgJoin
                    from tg in tgJoin.DefaultIfEmpty()
                    where d.CardCode == value.CardCode && d.AdresType == "S" && d.Address == value.Address
                    select new TaxGroupsEntity
                    {
                        Code = tg != null ? tg.Code : string.Empty,
                        Rate = tg != null ? tg.Rate : 0m
                    }
                 ).FirstOrDefaultAsync();

                if(data != null)
                {
                    if (string.IsNullOrEmpty(data.Code))
                    {
                        var exonerados = new HashSet<int> { 5, 24, 36 };
                        var taxCode = exonerados.Contains(value.SlpCode) ? "EXO" : "IGV18";

                        //// 🔹 Impuesto
                        var vatPrcnt = await _db.TaxGroups
                        .Where(n => n.Code == taxCode)
                        .Select(n => (decimal?)n.Rate)
                        .FirstOrDefaultAsync();

                        data = new TaxGroupsEntity
                        {
                            Code = taxCode ?? string.Empty,
                            Rate = vatPrcnt ?? 0
                        };
                    }
                }
                

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
