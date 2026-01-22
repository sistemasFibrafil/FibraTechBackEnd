using System;
using AutoMapper;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace Net.Data.Sap
{
    public class NumeracionDocumentoRepository : RepositoryBase<NumeracionDocumentoEntity>, INumeracionDocumentoRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly IMapper _mapper;
        private readonly DataContextSap _db;

        public NumeracionDocumentoRepository(IConnectionSQL context, DataContextSap db, IMapper mapper)
            : base(context)
        {
            _db = db;
            _mapper = mapper;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<NumeracionDocumento1Entity>> GetNumero(NumeracionDocumentoEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<NumeracionDocumento1Entity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await (from onn in _db.NumeracionDocumento
                                  join nnm1 in _db.NumeracionDocumento1
                                  on new { onn.ObjectCode, Series = onn.DfltSeries } equals new { nnm1.ObjectCode, nnm1.Series }
                                  where onn.ObjectCode == value.ObjectCode
                                  select new NumeracionDocumento1Entity
                                  {
                                      ObjectCode = onn.ObjectCode,
                                      SeriesName = nnm1.SeriesName,
                                      NextNumber = nnm1.NextNumber
                                  })
                                  .FirstOrDefaultAsync();

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
