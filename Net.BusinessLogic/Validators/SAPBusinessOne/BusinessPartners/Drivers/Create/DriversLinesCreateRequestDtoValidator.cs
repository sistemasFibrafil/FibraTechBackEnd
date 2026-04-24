using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Drivers.Create;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.BusinessPartners.Drivers.Create
{
    public class DriversLinesCreateRequestDtoValidator : AbstractValidator<DriversLinesCreateRequestDto>
    {
        public DriversLinesCreateRequestDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("El código del conductor es obligatorio.")
                .MaximumLength(20)
                .WithMessage("El código del conductor no debe exceder los 20 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre del conductor es obligatorio.")
                .MaximumLength(50)
                .WithMessage("El nombre del conductor no debe exceder los 50 caracteres.");
        }
    }
}
