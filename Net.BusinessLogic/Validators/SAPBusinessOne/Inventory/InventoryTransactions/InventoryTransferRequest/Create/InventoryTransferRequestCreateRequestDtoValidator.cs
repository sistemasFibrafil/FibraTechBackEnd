using FluentValidation;
using Net.BusinessLogic.Validators.SAPBusinessOne.Inventory.Picking.Create;
using Net.Business.DTO.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Inventory.InventoryTransactions.InventoryTransferRequest.Create
{
    public class InventoryTransferRequestCreateRequestDtoValidator : AbstractValidator<InventoryTransferRequestCreateRequestDto>
    {
        public InventoryTransferRequestCreateRequestDtoValidator()
        {
            RuleFor(x => x.DocDate)
                .NotEmpty()
                .WithMessage("La fecha del contabilización es obligatoria.");

            RuleFor(x => x.DocDueDate)
               .NotEmpty()
               .WithMessage("La fecha del entrega es obligatoria.");

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
                .SetValidator(new InventoryTransferRequestLinesCreateRequestDtoValidator());

            RuleFor(x => x.PickingLines)
                .NotEmpty()
                .When(x => x.U_FIB_IsPkg == "Y")
                .WithMessage("Si la solicitud de traslado es para generar picking, debe registrarse al menos una línea de picking.");

            RuleForEach(x => x.PickingLines)
                .SetValidator(new PickingInventoryTransferRequestCreateDtoValidator());
        }
    }
}
