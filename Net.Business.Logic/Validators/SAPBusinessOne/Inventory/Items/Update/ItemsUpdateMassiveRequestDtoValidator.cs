using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Inventory.Items.Update;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Inventory.Items.Update
{
    public class ItemsUpdateMassiveRequestDtoValidator : AbstractValidator<List<ItemsUpdateMassiveRequestDto>>
    {
        public ItemsUpdateMassiveRequestDtoValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Debe enviar líneas para actualizar.");

            RuleFor(x => x)
                .Must(x => x != null && x.Count > 0)
                .WithMessage("Debe enviar al menos una línea para actualizar.");
            RuleForEach(x => x)
                .SetValidator(new ItemsLinesUpdateMassiveRequestDtoValidator());
        }
    }
}
