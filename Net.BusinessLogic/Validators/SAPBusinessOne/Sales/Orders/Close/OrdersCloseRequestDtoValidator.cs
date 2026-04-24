using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Sales.Orders.Close;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Sales.Orders.Close
{
    public class OrdersCloseRequestDtoValidator : AbstractValidator<OrdersCloseRequestDto>
    {
        public OrdersCloseRequestDtoValidator()
        {
            RuleFor(x => x.DocEntry)
                .GreaterThan(0)
                .WithMessage("El DocEntry debe ser mayor a cero.");

            RuleFor(x => x.U_UsrClose)
                .GreaterThan(0)
                .WithMessage("El U_UsrClose debe ser mayor a cero.");
        }
    }
}
