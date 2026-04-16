using System;
using AutoMapper;
using System.Data;
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
    public class DriversRepository : RepositoryBase<DriversEntity>, IDriversRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly IMapper _mapper;
        private readonly DataContextSAPBusinessOne _db;


        public DriversRepository(IConnectionSQL context, DataContextSAPBusinessOne db, IMapper mapper)
            : base(context)
        {
            _db = db;
            _mapper = mapper;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<DriversEntity>> GetListByFilter(DriversFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<DriversEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.Driver
                .AsNoTracking()
                .Where(n => !string.IsNullOrWhiteSpace(n.U_BPP_CHLI) && !string.IsNullOrWhiteSpace(n.U_FIB_COTR) && n.U_FIB_COTR == value.U_FIB_COTR);


                // FILTRO DE TEXTO
                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.U_BPP_CHLI, $"%{filter}%") ||
                        EF.Functions.Like(n.U_BPP_CHNO, $"%{filter}%") ||
                        EF.Functions.Like(n.U_FIB_CHAP, $"%{filter}%")
                    );
                }


                var list = await query
                .Select(n => new DriversEntity
                {
                    U_BPP_CHLI = n.U_BPP_CHLI,
                    U_BPP_CHNO = n.U_BPP_CHNO,
                    U_FIB_CHAP = n.U_FIB_CHAP,
                    U_FIB_CHTD = n.U_FIB_CHTD,
                    U_FIB_CHND = n.U_FIB_CHND
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
        public async Task<ResultadoTransaccionEntity<DriversEntity>> SetCreate(DriversCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<DriversEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            await using var trx = await _db.Database.BeginTransactionAsync();

            try
            {
                // NUEVO
                var maxCode = (await _db.Driver.Select(x => x.Code).ToListAsync()).Select(x => int.Parse(x)).Max();

                var nextCode = maxCode + 1;

                foreach (var line in value.Lines.Where(x => x.Record == 1))
                {
                    line.Code = nextCode.ToString();
                    nextCode++;

                    var entity = _mapper.Map<DriversEntity>(line);
                    _db.Driver.Add(entity);
                }

                // ACTUALIZAR
                foreach (var line in (value.Lines.Where(x => x.Record == 3)))
                {
                    var entity = await _db.Driver.FirstOrDefaultAsync(x => x.Code == line.Code)
                    ?? throw new Exception($"El conductor con código '{line.Code}' no existe.");

                    var entry = _db.Entry(entity);
                    entry.CurrentValues.SetValues(line);
                }

                // ELIMINAR
                foreach (var line in (value.Lines.Where(x => x.Record == 4)))
                {
                    var entity = await _db.Driver.FirstOrDefaultAsync(x => x.Code == line.Code)
                    ?? throw new Exception($"El conductor con código '{line.Code}' no existe.");

                    _db.Driver.Remove(entity);
                }

                await _db.SaveChangesAsync();

                await trx.CommitAsync();
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync();

                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
    }
}
