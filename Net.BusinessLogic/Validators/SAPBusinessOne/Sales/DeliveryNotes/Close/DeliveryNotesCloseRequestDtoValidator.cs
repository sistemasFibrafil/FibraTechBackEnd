using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Close;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Sales.DeliveryNotes.Close
{
    public class DeliveryNotesCloseRequestDtoValidator : AbstractValidator<DeliveryNotesCloseRequestDto>
    {
        public DeliveryNotesCloseRequestDtoValidator()
        {
            RuleFor(x => x.DocEntry)
                .GreaterThan(0).WithMessage("DocEntry debe ser mayor que 0.");

            RuleFor(x => x.U_UsrClose)
                .GreaterThan(0).WithMessage("DocEntry debe ser mayor que 0.");
        }
    }
}
