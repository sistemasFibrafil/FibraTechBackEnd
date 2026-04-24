using Net.Business.DTO.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create;
using Net.Business.Entities.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create;
namespace Net.BusinessLogic.Mappers.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create
{
    public class DocumentSeriesConfigurationCreateMapper
    {
        public static DocumentSeriesConfigurationCreateEntity ToEntity(DocumentSeriesConfigurationCreateRequestDto dto)
        {
            return new DocumentSeriesConfigurationCreateEntity
            {
                Code = dto.Code,
                U_IdUser = dto.U_IdUser,
                U_Active = dto.U_Active,

                Lines = [.. dto.Lines.Select(l => new DocumentSeriesConfigurationLinesCreateEntity
                {
                    Code = l.Code,
                    LineId = l.LineId,
                    U_Type = l.U_Type,
                    U_Series = l.U_Series,
                    U_SalesInvoices = l.U_SalesInvoices,
                    U_Delivery = l.U_Delivery,
                    U_Transfer = l.U_Transfer,
                    U_Default = l.U_Default,
                    U_Active = l.U_Active,
                    Record = l.Record
                })]
            };
        }
    }
}
