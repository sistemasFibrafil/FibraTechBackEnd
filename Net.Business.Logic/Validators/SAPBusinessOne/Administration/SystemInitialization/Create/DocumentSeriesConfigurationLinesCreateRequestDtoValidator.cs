using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Administration.SystemInitialization.Create
{
    public class DocumentSeriesConfigurationLinesCreateRequestDtoValidator : AbstractValidator<DocumentSeriesConfigurationLinesCreateRequestDto>
    {
        public DocumentSeriesConfigurationLinesCreateRequestDtoValidator()
        {
            RuleFor(x => x.U_Type).NotEmpty().WithMessage("El campo tipo es obligatorio");
            RuleFor(x => x.U_Series).NotEmpty().WithMessage("El campo Series es obligatorio.");
        }
    }
}
