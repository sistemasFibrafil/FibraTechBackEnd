using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.BusinessPartners.Vehicles.Create;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.BusinessPartners.Vehicles.Create
{
    public class VehiclesLinesCreateRequestDtoValidator : AbstractValidator<VehiclesLinesCreateRequestDto>
    {
        public VehiclesLinesCreateRequestDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("El código del vehículo es obligatorio.")
                .MaximumLength(20)
                .WithMessage("El código del vehículo no debe exceder los 20 caracteres.");
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre del vehículo es obligatorio.")
                .MaximumLength(50)
                .WithMessage("El nombre del vehículo no debe exceder los 50 caracteres.");
            RuleFor(x => x.U_BPP_VEPL)
                .NotEmpty()
                .WithMessage("La placa del vehículo es obligatoria.")
                .MaximumLength(10)
                .WithMessage("La placa del vehículo no debe exceder los 10 caracteres.");
            RuleFor(x => x.U_BPP_VEMA)
                .NotEmpty()
                .WithMessage("La marca del vehículo es obligatoria.")
                .MaximumLength(30)
                .WithMessage("La marca del vehículo no debe exceder los 30 caracteres.");
            RuleFor(x => x.U_BPP_VEMO)
                .NotEmpty()
                .WithMessage("El modelo del vehículo es obligatorio.")
                .MaximumLength(30)
                .WithMessage("El modelo del vehículo no debe exceder los 30 caracteres.");
            RuleFor(x => x.U_BPP_VEAN)
                .NotEmpty()
                .WithMessage("El año del vehículo es obligatorio.")
                .Must(year => int.TryParse(year, out int y) && y > 1900 && y <= DateTime.Now.Year)
                .WithMessage($"El año del vehículo debe ser un número entre 1900 y {DateTime.Now.Year}.");
        }
    }
}
