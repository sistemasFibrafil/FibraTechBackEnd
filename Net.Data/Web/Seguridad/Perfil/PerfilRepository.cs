using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace Net.Data.Web
{
    public class PerfilRepository : RepositoryBase<PerilEntity>, IPerfilRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly DataContextSeg _dbSeg;

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "SEG_GetPerfilAll";
        const string SP_GET_ID = DB_ESQUEMA + "SEG_GetPerfilPorId";
        const string SP_INSERT = DB_ESQUEMA + "SEG_SetPerfilInsert";
        const string SP_DELETE = DB_ESQUEMA + "SEG_SetPerfilDelete";
        const string SP_UPDATE = DB_ESQUEMA + "SEG_SetPerfilUpdate";

        public PerfilRepository(IConnectionSQL context, DataContextSeg dbSeg)
            : base(context)
        {
            _dbSeg = dbSeg;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<PerilEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<PerilEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _dbSeg.Peril
                .Where(u => u.Eliminado == false)
                .Select(u => new PerilEntity
                {
                    IdPerfil = u.IdPerfil,
                    DescripcionPerfil = u.DescripcionPerfil,
                    Activo = u.Activo,
                }).ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
                resultTransaccion.dataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        public Task<IEnumerable<PerilEntity>> GetAll(PerilEntity entidad)
        {
            return Task.Run(() => FindAll(entidad, SP_GET));
        }
        public Task<PerilEntity> GetById(PerilEntity entidad)
        {
            return Task.Run(() => FindById(entidad, SP_GET_ID));
        }
        public async Task<int> Create(PerilEntity entidad)
        {

            return await Task.Run(() => Create(entidad, SP_INSERT));
        }
        public Task Update(PerilEntity entidad)
        {
            return Task.Run(() => Update(entidad, SP_UPDATE));
        }
        public Task Delete(PerilEntity entidad)
        {
            return Task.Run(() => Delete(entidad, SP_DELETE));
        }
    }
}