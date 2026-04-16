using FluentValidation;
using Net.Business.DTO.SAPBusinessOne;
namespace Net.Business.Services.Validators.SAPBusinessOne
{
    public class InventoryTransferRequest1CreateRequestDtoValidator : AbstractValidator<InventoryTransferRequest1CreateRequestDto>
    {
        public InventoryTransferRequest1CreateRequestDtoValidator()
        {
            RuleFor(x => x.ItemCode)
                .NotEmpty()
                .WithMessage("El código de artículo obligatorio.");


            RuleFor(x => x.FromWhsCod)
                .NotEmpty()
                .WithMessage("El almacén de origen es obligatorio.");


            RuleFor(x => x.WhsCode)
                .NotEmpty()
                .WithMessage("El almacén de destino es obligatorio.");


            RuleFor(x => x.U_tipoOpT12)
                .NotEmpty()
                .WithMessage("El tipo de operción es obligatorio.");


            RuleFor(x => x.UnitMsr)
                .NotEmpty()
                .WithMessage("La unidad de medida es obligatoria. Por favor, complete en la ventana “Datos Maestros del Artículo”, en la pestaña “Datos de inventario”.");


            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a cero.");
        }
    }
}
