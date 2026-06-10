using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Validate;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Validate
{
    public class InventoryTransferRequestLineValidateRequestDtoValidator : AbstractValidator<InventoryTransferRequestLinesValidateRequestDto>
    {
        public InventoryTransferRequestLineValidateRequestDtoValidator()
        {
            RuleFor(x => x.ItemCode)
                .NotEmpty()
                .WithMessage("El código de artículo es obligatorio.");

            RuleFor(x => x.FromWhsCod)
                .NotEmpty()
                .WithMessage("El almacén de origen es obligatorio.");

            RuleFor(x => x.WhsCode)
                .NotEmpty()
                .WithMessage("El almacén de destino es obligatorio.");

            RuleFor(x => x.U_tipoOpT12)
                .NotEmpty()
                .WithMessage("El código del tipo de operación es obligatorio.");

            RuleFor(x => x.UnitMsr)
                .NotEmpty()
                .WithMessage("La unidad de medida es obligatoria.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a cero.");
        }
    }
}
