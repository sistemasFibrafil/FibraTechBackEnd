using System.IO;
using Net.Business.Entities;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IPagoRecibidoRepository
    {
        Task<ResultadoTransaccionEntity<CobranzaCarteraVencidaByFilterSapEntity>> GetListCobranzaCarteraVencidaByFilter(PagoRecibidoByFilterEntity value);
        Task<ResultadoTransaccionEntity<MemoryStream>> GetCobranzaCarteraVencidaByFilterExcel(PagoRecibidoByFilterEntity value);
    }
}
