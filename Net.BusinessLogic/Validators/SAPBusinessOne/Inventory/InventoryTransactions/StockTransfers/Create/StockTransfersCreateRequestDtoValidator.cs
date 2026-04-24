using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Create;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Inventory.InventoryTransactions.StockTransfers.Create
{
    public class StockTransfersCreateRequestDtoValidator : AbstractValidator<StockTransfersCreateRequestDto>
    {
        public StockTransfersCreateRequestDtoValidator()
        {
            RuleFor(x => x.DocDate)
                .NotEmpty()
                .WithMessage("La fecha del contabilización es obligatoria.");

            RuleFor(x => x.TaxDate)
                .NotEmpty()
                .WithMessage("La fecha de documento es obligatoria.");

            RuleFor(x => x.Filler)
               .NotEmpty()
               .WithMessage("El almacen de origen es obligatoria.");

            RuleFor(x => x.ToWhsCode)
               .NotEmpty()
               .WithMessage("El almacen de destino es obligatoria.");

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


            RuleFor(x => x.Lines)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("El detalle del documento es obligatorio.")
                .NotEmpty()
                .WithMessage("El detalle del documento debe contener al menos un registro.");

            RuleForEach(x => x.Lines)
                .SetValidator(new StockTransfersLinesCreateRequestDtoValidator());
        }
    }
}
