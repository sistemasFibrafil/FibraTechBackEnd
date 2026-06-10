using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Close;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Purchasing.PurchaseRequest.Close
{
    public class PurchaseRequestCloseRequestDtoValidator : AbstractValidator<PurchaseRequestCloseRequestDto>
    {
        public PurchaseRequestCloseRequestDtoValidator()
        {
            RuleFor(x => x.DocEntry)
                .GreaterThan(0)
                .WithMessage("El DocEntry no es válido.");
            RuleFor(x => x.U_UsrClose)
                .GreaterThan(0)
                .WithMessage("El ID del usuario que cierra no es válido.");
        }
    }
}
