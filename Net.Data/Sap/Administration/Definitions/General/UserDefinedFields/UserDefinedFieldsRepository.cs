using System;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
namespace Net.Data.Sap
{
    public class UserDefinedFieldsRepository : RepositoryBase<UserDefinedFieldsEntity>, IUserDefinedFieldsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN  
        private readonly DataContextSap _db;
        
        public UserDefinedFieldsRepository(IConnectionSQL context, DataContextSap db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<UserDefinedFieldsQueryEntity>> GetList(UserDefinedFieldsEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UserDefinedFieldsQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query =
                from p in _db.UserDefinedFields1.AsNoTracking()
                join c in _db.UserDefinedFields.AsNoTracking() on new { p.TableID, p.FieldID } equals new { c.TableID, c.FieldID }
                where c.TableID == value.TableID && c.AliasID == value.AliasID
                select new { p, c };


                var list = await query
                .Select(x => new UserDefinedFieldsQueryEntity
                {
                    FldValue = x.p.FldValue,
                    Descr = x.p.Descr,
                    Dflt = x.c.Dflt
                })
                .Where(n => !string.IsNullOrEmpty(n.Descr))
                .OrderBy(n => n.FldValue)
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
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

        public async Task<ResultadoTransaccionEntity<UserDefinedFieldsQueryEntity>> GetListByFilter(UserDefinedFieldsFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UserDefinedFieldsQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query =
                from p in _db.UserDefinedFields1.AsNoTracking()
                join c in _db.UserDefinedFields.AsNoTracking() on new { p.TableID, p.FieldID } equals new { c.TableID, c.FieldID } 
                where c.TableID == value.TableID && c.AliasID == value.AliasID
                select new { p, c };


                // FILTRO
                if (!string.IsNullOrWhiteSpace(value.UserDefinedFields))
                {
                    var filter = value.UserDefinedFields.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(EF.Functions.Collate(x.p.FldValue!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.p.Descr!, GlobalVariables.CI), $"%{filter}%")
                    );
                }

                
                var list = await query
                .Select(x => new UserDefinedFieldsQueryEntity
                {
                    FldValue = x.p.FldValue,
                    Descr = x.p.Descr
                })
                .Where(n => !string.IsNullOrEmpty(n.Descr))
                .OrderBy(n => n.FldValue)
                .ToListAsync();


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
    }
}
