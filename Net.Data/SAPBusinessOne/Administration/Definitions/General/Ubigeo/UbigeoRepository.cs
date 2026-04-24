using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Net.Data.SAPBusinessOne
{
    public class UbigeoRepository : RepositoryBase<UbigeoEntity>, IUbigeoRepository
    {
        private string _aplicacionName;
        private readonly DataContextSAPBusinessOne _db;

        public UbigeoRepository(IConnectionSQL context, DataContextSAPBusinessOne db)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<UbigeoEntity>> GetList(string dpto, string prov, string dist)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UbigeoEntity>
            {
                NombreMetodo = "GetList",
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.Ubigeo.AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(dpto))
                {
                    query = query.Where(x => x.Departamento.Contains(dpto));
                }

                if (!string.IsNullOrEmpty(prov))
                {
                    query = query.Where(x => x.Provincia.Contains(prov));
                }

                if (!string.IsNullOrEmpty(dist))
                {
                    query = query.Where(x => x.Distrito.Contains(dist));
                }

                var data = await query.OrderBy(x => x.Code).ToListAsync();

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

        public async Task<ResultadoTransaccionEntity<string>> GetListProvincias(string dpto)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<string>
            {
                NombreMetodo = "GetListProvincias",
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.Ubigeo.AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(dpto))
                {
                    query = query.Where(x => x.Departamento.Contains(dpto));
                }

                var data = await query.Select(x => x.Provincia).Distinct().OrderBy(x => x).ToListAsync();

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

        public async Task<ResultadoTransaccionEntity<UbigeoEntity>> GetListDistritos(string dpto, string prov)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UbigeoEntity>
            {
                NombreMetodo = "GetListDistritos",
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _db.Ubigeo.AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(dpto))
                {
                    query = query.Where(x => x.Departamento.Contains(dpto));
                }

                if (!string.IsNullOrEmpty(prov))
                {
                    query = query.Where(x => x.Provincia.Contains(prov));
                }

                var data = await query.OrderBy(x => x.Distrito).ToListAsync();

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
