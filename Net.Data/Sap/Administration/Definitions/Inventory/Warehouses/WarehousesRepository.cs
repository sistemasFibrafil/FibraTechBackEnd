using System;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace Net.Data.Sap
{
    public class WarehousesRepository : RepositoryBase<WarehousesEntity>, IWarehousesRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSap _db;
               


        public WarehousesRepository(IConnectionSQL context, DataContextSap db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<WarehousesQueryEntity>> GetListByInactive(WarehousesEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<WarehousesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.Warehouses
                .AsNoTracking();


                // FILTRO POR INACTIVO
                if (!string.IsNullOrWhiteSpace(value.Inactive))
                {
                    var inactuve = value.Inactive.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(x => inactuve.Contains(x.Inactive));
                }


                var list = await query
                .Select(x => new WarehousesQueryEntity
                {
                    WhsCode = x.WhsCode,
                    FullDescr = x.WhsCode + " - " + x.WhsName
                })
                .OrderBy(x => x.WhsCode)
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
        public async Task<ResultadoTransaccionEntity<WarehousesQueryEntity>> GetListProduccion()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<WarehousesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _db.Warehouses
                .Where(n => n.Inactive == "N" && n.U_FIB_ALMPRO == "Y")
                .Select(n => new WarehousesQueryEntity
                {
                    WhsCode = n.WhsCode,
                    FullDescr = n.WhsCode + " - " + n.WhsName
                })
                .OrderBy(x => x.WhsCode)
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
        public async Task<ResultadoTransaccionEntity<WarehousesQueryEntity>> GetListByItem(WarehousesByItemFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<WarehousesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query =
                from n in _db.Warehouses
                join d in _db.ItemWarehouseInfo on n.WhsCode equals d.WhsCode
                where d.ItemCode == value.ItemCode
                select new
                {
                    n.WhsCode,
                    n.WhsName,
                    n.Inactive,
                    d.OnHand
                };


                // FILTRO POR INACTIVO
                if (!string.IsNullOrWhiteSpace(value.Inactive))
                {
                    var inactuve = value.Inactive.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(x => inactuve.Contains(x.Inactive));
                }


                // FILTRO POR ALMACÉN
                if (!string.IsNullOrWhiteSpace(value.Warehouse))
                {
                    var filter = value.Warehouse.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(EF.Functions.Collate(x.WhsCode!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.WhsName!, GlobalVariables.CI), $"%{filter}%")
                    );
                }


                var list = await query
                .Select(x => new WarehousesQueryEntity
                {
                    WhsCode = x.WhsCode,
                    WhsName = x.WhsName,
                    OnHand = x.OnHand
                })
                .OrderBy(x => x.WhsCode)
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
        public async Task<ResultadoTransaccionEntity<WarehousesQueryEntity>> GetListByWhsCodeAndItemCode(WarehousesByItemFindEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<WarehousesQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query =
                from n in _db.Warehouses
                join d in _db.ItemWarehouseInfo on n.WhsCode equals d.WhsCode
                where d.ItemCode == value.ItemCode
                select new
                {
                    n.WhsCode,
                    n.WhsName,
                    n.Inactive,
                    d.OnHand,
                    d.IsCommited,
                    d.OnOrder
                };


                // FILTRO POR INACTIVO
                if (!string.IsNullOrWhiteSpace(value.Inactive))
                {
                    var inactuve = value.Inactive.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(x => inactuve.Contains(x.Inactive));
                }


                // FILTRO POR ALMACÉN
                if (!string.IsNullOrWhiteSpace(value.WhsCode))
                {
                    var whsCode = value.WhsCode.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    query = query.Where(x => whsCode.Contains(x.WhsCode));
                }


                var list = await query
                .Select(n => new WarehousesQueryEntity
                {
                    WhsCode = n.WhsCode,
                    WhsName = n.WhsName,
                    OnHand = n.OnHand,
                    IsCommited = n.IsCommited,
                    OnOrder = n.OnOrder,
                    Available = n.OnHand - n.IsCommited + n.OnOrder
                })
                .OrderBy(x => x.WhsCode)
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
    }
}
