using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Sales.DeliveryNotes.Cancel;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Sales.DeliveryNotes.Cancel
{
    public class DeliveryNotesCancelRequestDtoValidator : AbstractValidator<DeliveryNotesCancelRequestDto>
    {
        public DeliveryNotesCancelRequestDtoValidator()
        {
            RuleFor(x => x.DocEntry)
                .GreaterThan(0).WithMessage("El DocEntry debe ser mayor a 0.");
            RuleFor(x => x.U_UsrCreate)
                .GreaterThan(0).WithMessage("El U_UsrCreate debe ser mayor a 0.");
            RuleFor(x => x.U_UsrCancel)
                .GreaterThan(0).WithMessage("El U_UsrCancel debe ser mayor a 0.");
        }
    }
}
