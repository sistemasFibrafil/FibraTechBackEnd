using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Drafts.Update;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Drafts.Update
{
    public class DraftsUpdateRequestDtoValidatior : AbstractValidator<DraftsUpdateRequestDto>
    {
        public DraftsUpdateRequestDtoValidatior()
        {
            RuleFor(x => x.DocDate)
                .NotEmpty()
                .WithMessage("La fecha del contabilización es obligatoria.");

            RuleFor(x => x.DocDueDate)
                .NotEmpty()
                .WithMessage("La fecha de vencimiento es obligatoria.");

            RuleFor(x => x.TaxDate)
                .NotEmpty()
                .WithMessage("La fecha de documento es obligatoria.");

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

            RuleFor(x => x.Lines)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("El detalle del documento es obligatorio.")
                .NotEmpty()
                .WithMessage("El detalle del documento debe contener al menos un registro.");

            /*
             Guardamos DocType en el contexto para que
             los validators hijos puedan acceder
            */
            RuleFor(x => x)
                .Custom((dto, context) =>
                {
                    context.RootContextData["DocType"] = dto.DocType;
                });

            RuleForEach(x => x.Lines)
                .SetValidator(new DraftsLinesUpdateRequestDtoValidator());
        }
    }
}
