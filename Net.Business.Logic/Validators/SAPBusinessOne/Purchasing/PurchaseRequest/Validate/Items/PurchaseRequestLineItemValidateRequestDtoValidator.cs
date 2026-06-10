using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Purchasing.PurchaseRequest.Validate.Items
{
    public class PurchaseRequestLineItemValidateRequestDtoValidator : AbstractValidator<PurchaseRequestLinesItemsValidateRequestDto>
    {
        public PurchaseRequestLineItemValidateRequestDtoValidator()
        {
            RuleFor(x => x.ItemCode)
                .NotEmpty()
                .WithMessage("El código de artículo es obligatorio.");

            RuleFor(x => x.PqtReqDate)
                .NotEmpty()
                .WithMessage("La fecha necesaria es obligatoria.");

            RuleFor(x => x.FormatCode)
                .NotEmpty()
                .WithMessage("La cuenta contable es obligatoria.");

            RuleFor(x => x.OcrCode)
                .NotEmpty()
                .WithMessage("El centro de costo es obligatorio.");

            RuleFor(x => x.WhsCode)
                .NotEmpty()
                .WithMessage("El almacén es obligatorio.");

            RuleFor(x => x.U_tipoOpT12)
                .NotEmpty()
                .WithMessage("El código del tipo de operación es obligatorio.");

            RuleFor(x => x.U_FF_TIP_COM)
                .NotEmpty()
                .WithMessage("El código del tipo de compra es obligatorio.");

            RuleFor(x => x.UnitMsr)
                .NotEmpty()
                .WithMessage("La unidad de medida es obligatoria.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a cero.");
        }
    }
}
