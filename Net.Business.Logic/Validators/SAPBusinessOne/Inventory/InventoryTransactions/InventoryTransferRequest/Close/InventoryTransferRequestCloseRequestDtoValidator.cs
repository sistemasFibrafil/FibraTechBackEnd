using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Close;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Close
{
    public class InventoryTransferRequestCloseRequestDtoValidator : AbstractValidator<InventoryTransferRequestCloseRequestDto>
    {
        public InventoryTransferRequestCloseRequestDtoValidator()
        {
            RuleFor(x => x.DocEntry)
                .GreaterThan(0)
                .WithMessage("El DocEntry no es válido.");
            RuleFor(x => x.U_UsrClose)
                .GreaterThan(0)
                .WithMessage("El ID del usuario que cierre no es válido.");
        }
    }
}
