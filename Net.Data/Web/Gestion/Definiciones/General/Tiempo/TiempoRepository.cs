using System;
using System.Linq;
using Net.Connection;
using System.Globalization;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
namespace Net.Data.Web
{
    public class TiempoRepository : RepositoryBase<StatusEntity>, ITiempoRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");
        private readonly CultureInfo cultureInfo = new("es-PE");


        public TiempoRepository(IConnectionSQL context)
            : base(context)
        {
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<AnioEntity>> GetListAnio()
        {
            var response = new List<AnioEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<AnioEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
            string peridoInicio = config.GetValue<string>("ParametrosTiempo:PeriodoInicio");

            int anioInicio = Convert.ToInt32(peridoInicio);
            int anioFinal = DateTime.Now.Year;

            for (int i = anioInicio; i <= anioFinal; i++)
            {
                response.Add(new AnioEntity { CodAnio = i, NomAnio = i.ToString() });
            }

            response = response.OrderByDescending(a => a.CodAnio).ToList();

            resultTransaccion.IdRegistro = 0;
            resultTransaccion.ResultadoCodigo = 0;
            resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
            resultTransaccion.dataList = response;

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<MesEntity>> GetListMes()
        {
            List<MesEntity> ListaItems = new();
            var response = new List<MesEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<MesEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            for (int i = 1; i <= 12; i++)
            {
                response.Add(new MesEntity { CodMes = i, NomMes = cultureInfo.DateTimeFormat.GetMonthName(i) });
            }

            if (ListaItems.Count > 0)
            {
                response.AddRange(ListaItems);
                response = response.OrderBy(m => m.CodMes).ToList();
            }

            resultTransaccion.IdRegistro = 0;
            resultTransaccion.ResultadoCodigo = 0;
            resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
            resultTransaccion.dataList = response;

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<SemanaEntity>> GetListSemana(FilterRequestEntity value)
        {
            var response = new List<SemanaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<SemanaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            response = GetListSemana2(value).OrderBy(s => s.CodSemana).ToList();

            resultTransaccion.IdRegistro = 0;
            resultTransaccion.ResultadoCodigo = 0;
            resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
            resultTransaccion.dataList = response;

            return resultTransaccion;
        }

        private List<SemanaEntity> GetListSemana2(FilterRequestEntity value)
        {
            // Se obtiene la semana según cada viernes
            var numero = 1;
            var listSemana = new List<SemanaEntity>();
            int daysInMonth = DateTime.DaysInMonth(value.Id1, value.Id2);

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(value.Id1, value.Id2, day);
                if (date.DayOfWeek == DayOfWeek.Friday)
                {
                    listSemana.Add(new SemanaEntity { CodSemana = numero, NomSemana = $"Semana {numero}" });
                    numero++;
                }
            }

            return listSemana;
        }
    }
}
