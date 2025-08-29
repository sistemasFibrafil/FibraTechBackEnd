using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace Net.Data.Sap
{
    public class CampoDefinidoUsuarioRepository : RepositoryBase<CampoDefinidoUsuarioEntity>, ICampoDefinidoUsuarioRepository
    {
        // PARAMETROS DE COXIÓN  
        private readonly DataContextFil _db;
        
        public CampoDefinidoUsuarioRepository(IConnectionSQL context, DataContextFil db)
            : base(context)
        {
            _db = db;
        }

        public async Task<ResultadoTransaccionEntity<CampoDefinidoUsuario1Entity>> GetListByFiltro(CampoDefinidoUsuarioEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<CampoDefinidoUsuario1Entity>();

            try
            {
                var data = await (from p in _db.CampoDefinidoUsuario1
                                  join c in _db.CampoDefinidoUsuario on new { p.TableID, p.FieldID } equals new { c.TableID, c.FieldID }
                                  where c.TableID == value.TableID && c.AliasID == value.AliasID
                                  select new CampoDefinidoUsuario1Entity
                                   {
                                       FldValue = p.FldValue,
                                       Descr = p.Descr,
                                   }).ToListAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                resultTransaccion.dataList = data;
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
