using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Inventory.Items.Update;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Inventory.Items.Update
{
    public class ItemsLinesUpdateMassiveRequestDtoValidator : AbstractValidator<ItemsUpdateMassiveRequestDto>
    {
        public ItemsLinesUpdateMassiveRequestDtoValidator()
        {
            RuleFor(x => x.ItemCode)
                .NotEmpty()
                .WithMessage("El código de artículo es obligatorio.");

             RuleFor(x => x.U_UsrUpdate)
                .GreaterThan(0)
                .WithMessage("El usuario actualización es obligatorio.");
        }
    }
}
