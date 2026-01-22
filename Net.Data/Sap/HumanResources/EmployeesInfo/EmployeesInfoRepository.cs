using System;
using System.Linq;
using Net.Connection;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks; 
using Net.Business.Entities.Sap;
using Microsoft.EntityFrameworkCore;
namespace Net.Data.Sap
{
    public class EmployeesInfoRepository : RepositoryBase<EmployeesInfoEntity>, IEmployeesInfoRepository
    {
        // PARAMETROS DE COXIÓN
        private readonly DataContextSap _db;

        public EmployeesInfoRepository(IConnectionSQL context, DataContextSap db)
            : base(context)
        {
            _db = db;
        }


        public async Task<ResultadoTransaccionEntity<EmployeesInfoQueryEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<EmployeesInfoQueryEntity>();

            try
            {
                var data = await _db.EmployeesInfo
                .Where(s => s.Active == "Y")
                .Select(s => new EmployeesInfoQueryEntity
                {
                    empID = s.empID,
                    fullName = s.lastName + ", " + s.firstName
                })
                .ToListAsync();

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

        public async Task<ResultadoTransaccionEntity<EmployeesInfoQueryEntity>> GetById(EmployeesInfoEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<EmployeesInfoQueryEntity>();

            try
            {
                var data = await _db.EmployeesInfo
                .Where(s => s.empID == value.empID)
                .Select(s => new EmployeesInfoQueryEntity
                {
                    empID = s.empID,
                    fullName = s.lastName,
                    firstName = s.firstName,
                    middleName = s.middleName,
                    dept = s.dept,
                    branch = s.branch,
                    email = s.email,
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
