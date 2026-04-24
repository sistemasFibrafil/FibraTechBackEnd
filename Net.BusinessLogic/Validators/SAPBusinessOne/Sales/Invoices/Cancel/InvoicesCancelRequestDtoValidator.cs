using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Sales.Invoices.Cancel;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Sales.Invoices.Cancel
{
    public class InvoicesCancelRequestDtoValidator : AbstractValidator<InvoicesCancelRequestDto>
    {
        public InvoicesCancelRequestDtoValidator()
        {
            RuleFor(x => x.DocEntry)
                .GreaterThan(0)
                .WithMessage("El DocEntry debe ser mayor a cero.");

            RuleFor(x => x.U_UsrCreate)
                .GreaterThan(0)
                .WithMessage("El U_UsrCreate debe ser mayor a cero.");

            RuleFor(x => x.U_UsrCancel)
                .GreaterThan(0)
                .WithMessage("El U_UsrCancel debe ser mayor a cero.");
        }
    }
}
