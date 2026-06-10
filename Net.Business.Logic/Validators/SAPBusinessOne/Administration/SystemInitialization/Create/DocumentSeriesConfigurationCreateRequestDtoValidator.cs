using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Administration.SystemInitialization.DocumentSeriesConfiguration.Create;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Administration.SystemInitialization.Create
{
    public class DocumentSeriesConfigurationCreateRequestDtoValidator : AbstractValidator<DocumentSeriesConfigurationCreateRequestDto>
    {
        public DocumentSeriesConfigurationCreateRequestDtoValidator()
        {
            RuleFor(x => x.U_IdUser)
                .GreaterThan(0)
                .WithMessage("El ID del usuario no es válido.");
        }
    }
}
