using System;
using AutoMapper;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class CargaSaldoInicialRepository : RepositoryBase<CargaSaldoInicialEntity>, ICargaSaldoInicialRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly IMapper _mapper;
        private readonly DataContextSAPBusinessOne _db;

        public CargaSaldoInicialRepository(IConnectionSQL context, IConfiguration configuration, DataContextSAPBusinessOne db, IMapper mapper)
            : base(context)
        {
            _db = db;
            _mapper = mapper;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<CargaSaldoInicialEntity>> GetListByFilter(CargaSaldoInicialFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<CargaSaldoInicialEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                value.Item = value.Item?.ToString().Trim() ?? string.Empty;

                var list = await _db.CargaSaldoInicial.Where(n => n.FechaSI >= value.StartDate && n.FechaSI <= value.EndDate && n.ItemCode.ToString().Contains(value.Item)).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.dataList = _mapper.Map<List<CargaSaldoInicialEntity>>(list);
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
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
