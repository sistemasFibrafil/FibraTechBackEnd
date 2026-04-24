using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Update;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Update
{
    public class StockTransfersUpdateRequestDtoValidator : AbstractValidator<StockTransfersUpdateRequestDto>
    {
        public StockTransfersUpdateRequestDtoValidator()
        {
            RuleFor(x => x.TaxDate)
                .NotEmpty()
                .WithMessage("La fecha de documento es obligatoria.");

            RuleFor(x => x.U_FIB_TIP_TRAS)
                .NotEmpty()
                .WithMessage("El tipo de traslado es obligatorio.");

            RuleFor(x => x.U_BPP_MDMT)
                .NotEmpty()
                .WithMessage("El motivo de traslado es obligatorio.");

            RuleFor(x => x.U_BPP_MDTS)
                .NotEmpty()
                .WithMessage("El tipo de salida es obligatorio.");

            RuleFor(x => x.SlpCode)
                .NotNull()
                .WithMessage("El código de vendedor es obligatorio.")
                .Must(x => x == -1 || x > 0)
                .WithMessage("El código de vendedor debe ser -1 o un valor mayor a cero.");
        }
    }
}
