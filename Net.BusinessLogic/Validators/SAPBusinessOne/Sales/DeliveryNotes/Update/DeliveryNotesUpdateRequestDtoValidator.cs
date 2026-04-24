using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Update;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Sales.DeliveryNotes.Update
{
    public class DeliveryNotesUpdateRequestDtoValidator : AbstractValidator<DeliveryNotesUpdateRequestDto>
    {
        public DeliveryNotesUpdateRequestDtoValidator()
        {
            RuleFor(x => x.DocDueDate)
                .NotEmpty()
                .WithMessage("La fecha de vencimiento es obligatoria.");

            RuleFor(x => x.DocType)
                .NotEmpty()
                .WithMessage("El tipo de documento es obligatorio.")
                .Must(x => x == "I" || x == "S")
                .WithMessage("El tipo de documento debe ser 'I' (Artículo) o 'S' (Servicio).");

            RuleFor(x => x.CardCode)
                .NotEmpty()
                .WithMessage("El código del cliente es obligatorio.")
                .MaximumLength(15)
                .WithMessage("El código del cliente no debe exceder los 15 caracteres.");

            RuleFor(x => x.GroupNum)
                .NotEmpty()
                .WithMessage("La condición de pago es obligatoria.");

            RuleFor(x => x.U_STR_TVENTA)
                .NotEmpty()
                .WithMessage("El tipo de es obligatorio.");

            RuleFor(x => x.U_BPP_MDMT)
                .NotEmpty()
                .WithMessage("El motivo de traslado es obligatorio.");

            RuleFor(x => x.SlpCode)
                .NotNull()
                .WithMessage("El código de vendedor es obligatorio.")
                .Must(x => x == -1 || x > 0)
                .WithMessage("El código de vendedor debe ser -1 o un valor mayor a cero.");
        }
    }
}
