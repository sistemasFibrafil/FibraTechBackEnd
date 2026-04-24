using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Inventory.Picking.Create;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Inventory.Picking.Create
{
    public class PickingInventoryTransferRequestCreateDtoValidator : AbstractValidator<InventoryTransferRequestPickingCreateRequestDto>
    {
        public PickingInventoryTransferRequestCreateDtoValidator()
        {
            RuleFor(x => x.U_ItemCode)
                .NotEmpty()
                .WithMessage("El código de artículo es obligatorio.");


            RuleFor(x => x.U_CodeBar)
                .NotEmpty()
                .WithMessage("El código de barrar de artículo es obligatorio.");


            RuleFor(x => x.U_FromWhsCod)
                .NotEmpty()
                .WithMessage("El almacén de origen es es obligatorio.");


            RuleFor(x => x.U_WhsCode)
                .NotEmpty()
                .WithMessage("El almacén de destino es es obligatorio.");

            RuleFor(x => x.U_UnitMsr)
                .NotEmpty()
                .WithMessage("La unidad de medida es obligatoria. Por favor, complete en la ventana “Datos Maestros del Artículo”, en la pestaña “Datos de inventario”.");


            RuleFor(x => x.U_Quantity)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a cero.");


            RuleFor(x => x.U_WeightKg)
                .GreaterThan(0)
                .WithMessage("El peso debe ser mayor a cero.");
        }
    }
}
