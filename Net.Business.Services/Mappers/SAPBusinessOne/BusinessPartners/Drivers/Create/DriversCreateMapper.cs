using System.Linq;
using Net.Business.DTO.SAPBusinessOne;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.Services.Mappers.SAPBusinessOne
{
    public class DriversCreateMapper
    {
        public static DriversCreateEntity ToEntity(DriversCreateRequestDto dto)
        {
            return new DriversCreateEntity
            {
                CardCode = dto.CardCode,
                Lines = [.. dto.Lines.Select(l => new Drivers1CreateEntity
                {
                    Code = l.Code,
                    Name = l.Name,
                    U_BPP_CHNO = l.U_BPP_CHNO,
                    U_FIB_CHAP = l.U_FIB_CHAP,
                    U_FIB_CHTD = l.U_FIB_CHTD,
                    U_FIB_CHND = l.U_FIB_CHND,
                    U_BPP_CHLI = l.U_BPP_CHLI,
                    U_FIB_COTR = l.U_FIB_COTR,
                    Record = l.Record,
                })]
            };
        }
    }
}
