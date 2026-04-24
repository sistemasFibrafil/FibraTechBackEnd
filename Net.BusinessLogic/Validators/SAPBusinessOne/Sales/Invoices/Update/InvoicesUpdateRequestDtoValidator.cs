using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Update;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Sales.Invoices.Update
{
    public class InvoicesUpdateRequestDtoValidator : AbstractValidator<InvoicesUpdateRequestDto>
    {
        public InvoicesUpdateRequestDtoValidator()
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

            RuleFor(x => x.DocCur)
                .NotEmpty()
                .WithMessage("La moneda del documento es obligatoria.")
                .Length(3)
                .WithMessage("La moneda del documento debe tener exactamente 3 caracteres.");

            RuleFor(x => x.DocRate)
                .GreaterThan(0)
                .WithMessage("El tipo de cambio debe ser mayor a cero.");

            RuleFor(x => x.GroupNum)
                .NotEmpty()
                .WithMessage("La condición de pago es obligatoria.");

            RuleFor(x => x.U_STR_TVENTA)
                .NotEmpty()
                .WithMessage("El tipo de es obligatorio.");

            RuleFor(x => x.SlpCode)
                .NotNull()
                .WithMessage("El código de vendedor es obligatorio.")
                .Must(x => x == -1 || x > 0)
                .WithMessage("El código de vendedor debe ser -1 o un valor mayor a cero.");
        }
    }
}
