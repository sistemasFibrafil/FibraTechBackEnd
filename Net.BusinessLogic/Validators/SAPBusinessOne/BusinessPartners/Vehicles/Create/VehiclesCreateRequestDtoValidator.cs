using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Vehicles.Create;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.BusinessPartners.Vehicles.Create
{
    public class VehiclesCreateRequestDtoValidator : AbstractValidator<VehiclesCreateRequestDto>
    {
        public VehiclesCreateRequestDtoValidator()
        {
            RuleFor(x => x.CardCode)
                .NotEmpty()
                .WithMessage("El código del cliente es obligatorio.")
                .MaximumLength(15)
                .WithMessage("El código del cliente no debe exceder los 15 caracteres.");
            RuleFor(x => x.Lines)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Las líneas de vehículos son obligatorias.")
                .Must(lines => lines != null && lines.Count > 0)
                .WithMessage("Debe haber al menos una línea de vehículo.");

            RuleForEach(x => x.Lines)
                .SetValidator(new VehiclesLinesCreateRequestDtoValidator());
        }
    }
}
