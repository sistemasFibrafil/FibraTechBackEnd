using FluentValidation;
using Net.Business.DTO.SAPBusinessOne;
namespace Net.Business.Services.Validators.SAPBusinessOne
{
    public class PurchaseRequest1UpdateRequestDtoValidator : AbstractValidator<PurchaseRequest1UpdateRequestDto>
    {
        public PurchaseRequest1UpdateRequestDtoValidator()
        {
            RuleFor(x => x.ItemCode)
                .NotEmpty()
                .When((line, context) =>
                {
                    var docType = context.RootContextData["DocType"]?.ToString();
                    return docType == "I";
                })
                .WithMessage("El código de artículo es obligatorio.");


            RuleFor(x => x.PqtReqDate)
                .NotEmpty()
                .WithMessage("La fecha necesaria es obligatoria en el detalle del documento.");


            RuleFor(x => x.AcctCode)
                .NotEmpty()
                .WithMessage("La cuenta contable es obligatoria.");


            RuleFor(x => x.OcrCode)
                .NotEmpty()
                .WithMessage("El centro de costo es obligatoria.");


            RuleFor(x => x.WhsCode)
                .NotEmpty()
                .When((line, context) =>
                {
                    var docType = context.RootContextData["DocType"]?.ToString();
                    return docType == "I";
                })
                .WithMessage("El almacén es obligatorio.");


            RuleFor(x => x.U_tipoOpT12)
                .NotEmpty()
                .WithMessage("El tipo de operación es obligatorio.");


            RuleFor(x => x.U_FF_TIP_COM)
                .NotEmpty()
                .WithMessage("El tipo de compra es obligatorio.");


            RuleFor(x => x.UnitMsr)
                .NotEmpty()
                .When((line, context) =>
                {
                    var docType = context.RootContextData["DocType"]?.ToString();
                    return docType == "I";
                })
                .WithMessage("La unidad de medida es obligatoria. Por favor, complete en la ventana “Datos Maestros del Artículo”, en la pestaña “Datos de compras”.");


            RuleFor(x => x.UnitMsr)
                .NotEmpty()
                .When((line, context) =>
                {
                    var docType = context.RootContextData["DocType"]?.ToString();
                    return docType == "I";
                })
                .WithMessage("La unidad de medida es obligatoria.");


            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .When((line, context) =>
                {
                    var docType = context.RootContextData["DocType"]?.ToString();
                    return docType == "I";
                })
                .WithMessage("La cantidad debe ser mayor a cero.");
        }
    }
}
