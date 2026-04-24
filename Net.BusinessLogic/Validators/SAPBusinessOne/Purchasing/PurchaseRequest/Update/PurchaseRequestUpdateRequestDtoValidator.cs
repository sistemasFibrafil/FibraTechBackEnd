using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Update;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Purchasing.PurchaseRequest.Update
{
    public class PurchaseRequestUpdateRequestDtoValidator : AbstractValidator<PurchaseRequestUpdateRequestDto>
    {
        public PurchaseRequestUpdateRequestDtoValidator()
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

            RuleFor(x => x.ReqDate)
                .NotEmpty()
                .WithMessage("La fecha de necesaria es obligatoria.");

            RuleFor(x => x.ReqType)
                .NotEmpty()
                .WithMessage("El tipo de solicitante es obligatorio.");

            RuleFor(x => x.Requester)
                .NotEmpty()
                .WithMessage("El solicitante es obligatorio.");

            RuleFor(x => x.DocType)
                .NotEmpty()
                .WithMessage("El tipo de documento es obligatorio.")
                .Must(x => x == "I" || x == "S")
                .WithMessage("El tipo de documento debe ser 'I' (Artículo) o 'S' (Servicio).");

            RuleFor(x => x.OwnerCode)
                .NotNull()
                .WithMessage("El propitario es obligatorio.");


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
                .SetValidator(new PurchaseRequest1UpdateRequestDtoValidator());
        }
    }
}
