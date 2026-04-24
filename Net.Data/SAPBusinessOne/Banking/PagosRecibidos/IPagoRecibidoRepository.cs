using System.IO;
using Net.CrossCotting;
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface IPagoRecibidoRepository
    {
        Task<ResultadoTransaccionResponse<CobranzaCarteraVencidaByFilterSapEntity>> GetListCobranzaCarteraVencidaByFilter(PagoRecibidoByFilterEntity value);
        Task<ResultadoTransaccionResponse<MemoryStream>> GetCobranzaCarteraVencidaByFilterExcel(PagoRecibidoByFilterEntity value);
    }
}
