using Net.Business.Entities;
using Net.Business.Entities.SAPBusinessOne;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data.SAPBusinessOne
{
    public interface IUbigeoRepository
    {
        Task<ResultadoTransaccionEntity<UbigeoEntity>> GetList(string dpto, string prov, string dist);
        Task<ResultadoTransaccionEntity<string>> GetListProvincias(string dpto);
        Task<ResultadoTransaccionEntity<UbigeoEntity>> GetListDistritos(string dpto, string prov);
    }
}
