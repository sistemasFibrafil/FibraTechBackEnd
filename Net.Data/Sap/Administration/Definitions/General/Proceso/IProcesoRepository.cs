using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Business.Entities;
using Net.Business.Entities.Sap;

namespace Net.Data.Sap
{
    public interface IProcesoRepository
    {
        Task<ResultadoTransaccionEntity<ProcesoEntity>> GetList();
        Task<ResultadoTransaccionEntity<ProcesoEntity>> GetListByFiltro(ProcesoEntity value);
    }
}
