using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Drivers.Create;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.BusinessPartners.Drivers.Create
{
    public class DriversCreateRequestDtoValidator : AbstractValidator<DriversCreateRequestDto>
    {
        public DriversCreateRequestDtoValidator()
        {
            RuleFor(x => x.CardCode)
                .NotEmpty()
                .WithMessage("El código del socio de negocios es obligatorio.")
                .MaximumLength(20)
                .WithMessage("El código del socio de negocios no debe exceder los 20 caracteres.");

            RuleFor(x => x.Lines)
                .NotEmpty()
                .WithMessage("La lista de conductores no puede estar vacía.")
                .Must(lines => lines != null && lines.Any())
                .WithMessage("La lista de conductores debe contener al menos un conductor.");

            RuleForEach(x => x.Lines).SetValidator(new DriversLinesCreateRequestDtoValidator());
        }

    }
}
