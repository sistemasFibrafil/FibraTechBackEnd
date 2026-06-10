using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Validate;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Validate
{
    public class InventoryTransferRequestLinesValidateRequestDtoValidator : AbstractValidator<List<InventoryTransferRequestLinesValidateRequestDto>>
    {
        public InventoryTransferRequestLinesValidateRequestDtoValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Debe enviar líneas para validar.");

            RuleFor(x => x)
                .Must(x => x != null && x.Count > 0)
                .WithMessage("Debe enviar al menos una línea para validar.");

            RuleForEach(x => x)
                .SetValidator(new InventoryTransferRequestLineValidateRequestDtoValidator());
        }
    }
}
