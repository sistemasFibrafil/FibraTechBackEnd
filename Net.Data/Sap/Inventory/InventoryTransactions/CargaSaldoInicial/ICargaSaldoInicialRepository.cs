using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Business.Entities;
using Net.Business.Entities.Sap;

namespace Net.Data.Sap
{
    public interface ICargaSaldoInicialRepository
    {
        Task<ResultadoTransaccionEntity<CargaSaldoInicialEntity>> GetListByFilter(CargaSaldoInicialFilterEntity value);
    }
}
