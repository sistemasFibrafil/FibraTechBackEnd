using Net.Business.Entities.SAPBusinessOne;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Vehicles.Create;
using Net.Business.Entities.SAPBusinessOne.BusinessPartners.Vehicles.Create;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.BusinessPartners.Vehicles.Create
{
    public class VehiclesCreateMapper
    {
        public static VehiclesCreateEntity ToEntity(VehiclesCreateRequestDto dto)
        {
            return new VehiclesCreateEntity
            {
                CardCode = dto.CardCode,
                Lines = [.. dto.Lines.Select(l => new VehiclesLinesCreateEntity
                {
                    Code = l.Code,
                    Name = l.Name,
                    U_BPP_VEPL = l.U_BPP_VEPL,
                    U_BPP_VEMA = l.U_BPP_VEMA,
                    U_BPP_VEMO = l.U_BPP_VEMO,
                    U_BPP_VEAN = l.U_BPP_VEAN,
                    U_BPP_VECO = l.U_BPP_VECO,
                    U_BPP_VESE = l.U_BPP_VESE,
                    U_BPP_VEPM = l.U_BPP_VEPM,
                    U_FIB_COTR = l.U_FIB_COTR,
                    Record = l.Record,
                })]
            };
        }
    }
}
