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
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class DocumentNumberingSeriesRepository : RepositoryBase<DocumentNumberingSeriesEntity>, IDocumentNumberingSeriesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;

        public DocumentNumberingSeriesRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<DocumentNumberingSeries1Entity>> GetNumero(DocumentNumberingSeriesFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<DocumentNumberingSeries1Entity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await (from onn in _db.DocumentNumberingSeries
                                  join nnm1 in _db.DocumentNumberingSeries1
                                  on new { onn.ObjectCode, Series = onn.DfltSeries, onn.DocSubType } equals new { nnm1.ObjectCode, nnm1.Series, nnm1.DocSubType }
                                  where onn.ObjectCode == value.ObjectCode && onn.DocSubType == value.DocSubType
                                  select new DocumentNumberingSeries1Entity
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
