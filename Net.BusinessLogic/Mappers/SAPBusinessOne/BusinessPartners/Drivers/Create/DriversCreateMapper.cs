using Net.Business.Entities.SAPBusinessOne;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Drivers.Create;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Driver.Create;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.BusinessPartners.Drivers.Create
{
    public class DriversCreateMapper
    {
        public static DriversCreateEntity ToEntity(DriversCreateRequestDto dto)
        {
            return new DriversCreateEntity
            {
                CardCode = dto.CardCode,
                Lines = [.. dto.Lines.Select(l => new DriversLinesCreateEntity
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
