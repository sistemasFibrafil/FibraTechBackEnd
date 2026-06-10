using System;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.OperationsTypes.Query;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.OperationsTypes.Filter;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.OperationsTypes.Entities;
namespace Net.Data.SAPBusinessOne
{
    public class OperationsTypesRepository : RepositoryBase<OperationsTypesEntity>, IOperationsTypesRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN  
        private readonly DataContextSAPBusinessOne _db;


        public OperationsTypesRepository(IConnectionSQL context, DataContextSAPBusinessOne dc)
            : base(context)
        {
            _db = dc;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionResponse<OperationsTypesQueryEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OperationsTypesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.OperationsTypes
                .AsNoTracking()
                .OrderBy(n => n.Code)
                .Select(n => new OperationsTypesQueryEntity
                {
                    Code = n.Code,
                    U_descrp = n.U_descrp
                })
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
                resultTransaccion.DataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<OperationsTypesQueryEntity>> GetListByFilter(OperationsTypesFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OperationsTypesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.OperationsTypes
                .AsNoTracking();


                // FILTRO POR CODIGO O DESCRIPCION
                if (!string.IsNullOrWhiteSpace(value.TipoOperacion))
                {
                    var filter = value.TipoOperacion.Trim();

                    query = query.Where(n =>
                        EF.Functions.Like(EF.Functions.Collate(n.Code!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(n.U_descrp!, GlobalVariables.CI), $"%{filter}%")
                    );
                }


                var list = await query
                .OrderBy(n => n.Code)
                .Select(n => new OperationsTypesQueryEntity
                {
                    Code = n.Code,
                    U_descrp = n.U_descrp
                })
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", list.Count);
                resultTransaccion.DataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<OperationsTypesQueryEntity>> GetByCode(string code)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<OperationsTypesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.OperationsTypes
                .AsNoTracking()
                .Where(n => n.Code == code)
                .Select(n => new OperationsTypesQueryEntity
                {
                    Code = n.Code,
                    U_descrp = n.U_descrp
                })
                .FirstOrDefaultAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.Data = data;
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
