using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Purchasing.PurchaseRequest.Validate.Items
{
    public class PurchaseRequestLinesItemsValidateRequestDtoValidator : AbstractValidator<List<PurchaseRequestLinesItemsValidateRequestDto>>
    {
        public PurchaseRequestLinesItemsValidateRequestDtoValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Debe enviar líneas para validar.");

            RuleFor(x => x)
                .Must(x => x != null && x.Count > 0)
                .WithMessage("Debe enviar al menos una línea para validar.");

            RuleForEach(x => x)
                .SetValidator(new PurchaseRequestLineItemValidateRequestDtoValidator());
        }
    }
}
