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
    public class VehiclesRepository : RepositoryBase<VehiclesEntity>, IVehiclesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly IMapper _mapper;
        private readonly DataContextSAPBusinessOne _db;


        public VehiclesRepository(IConnectionSQL context, DataContextSAPBusinessOne db, IMapper mapper)
            : base(context)
        {
            _db = db;
            _mapper = mapper;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<VehiclesEntity>> GetListByFilter(VehiclesFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<VehiclesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.Vehicle
                .AsNoTracking()
                .Where(n => !string.IsNullOrWhiteSpace(n.U_BPP_VEPL) && !string.IsNullOrWhiteSpace(n.U_FIB_COTR) && n.U_FIB_COTR == value.U_FIB_COTR);


                // FILTRO DE TEXTO
                if (!string.IsNullOrWhiteSpace(value.SearchText))
                {
                    var filter = value.SearchText.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(n.U_BPP_VEMA, $"%{filter}%") ||
                        EF.Functions.Like(n.U_BPP_VEPL, $"%{filter}%") ||
                        EF.Functions.Like(n.U_BPP_VEMO, $"%{filter}%")
                    );
                }


                var list = await query
                .Select(n => new VehiclesEntity
                {
                    U_BPP_VEMA = n.U_BPP_VEMA,
                    U_BPP_VEPL = n.U_BPP_VEPL,
                    U_BPP_VEMO = n.U_BPP_VEMO,
                    U_BPP_VEPM = n.U_BPP_VEPM ?? 0
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
        public async Task<ResultadoTransaccionEntity<VehiclesEntity>> SetCreate(VehiclesCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<VehiclesEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            await using var trx = await _db.Database.BeginTransactionAsync();

            try
            {
                // NUEVO
                var maxCode = (await _db.Vehicle.Select(x => x.Code).ToListAsync()).Select(x => int.Parse(x)).Max();

                var nextCode = maxCode + 1;

                foreach (var line in value.Lines.Where(x => x.Record == 1))
                {
                    line.Code = nextCode.ToString();
                    nextCode++;

                    var entity = _mapper.Map<VehiclesEntity>(line);
                    _db.Vehicle.Add(entity);
                }

                // ACTUALIZAR
                foreach (var line in (value.Lines.Where(x => x.Record == 3)))
                {
                    var entity = await _db.Vehicle.FirstOrDefaultAsync(x => x.Code == line.Code)
                    ?? throw new Exception($"El vehículo con código '{line.Code}' no existe.");

                    var entry = _db.Entry(entity);
                    entry.CurrentValues.SetValues(line);
                }

                // ELIMINAR
                foreach (var line in (value.Lines.Where(x => x.Record == 4)))
                {
                    var entity = await _db.Vehicle.FirstOrDefaultAsync(x => x.Code == line.Code)
                    ?? throw new Exception($"El vehículo con código '{line.Code}' no existe.");

                    _db.Vehicle.Remove(entity);
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
