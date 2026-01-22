using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
namespace Net.Data.Sap
{
    public interface IPagoRecibidoRepository
    {
        Task<ResultadoTransaccionEntity<CobranzaCarteraVencidaByFilterSapEntity>> GetListCobranzaCarteraVencidaByFilter(PagoRecibidoByFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetCobranzaCarteraVencidaByFilterExcel(PagoRecibidoByFilterEntity value);
    }
}
