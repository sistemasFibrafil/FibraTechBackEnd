using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Sales.Orders.Create;
namespace Net.BusinessLogic.Validators.SAPBusinessOne.Sales.Orders.Create
{
    public class OrdersLinesCreateRequestDtoValidator : AbstractValidator<OrdersLinesCreateRequestDto>
    {
        public OrdersLinesCreateRequestDtoValidator()
        {
            RuleFor(x => x.ItemCode)
                .NotEmpty()
                .When((line, context) =>
                {
                    var docType = context.RootContextData["DocType"]?.ToString();
                    return docType == "I";
                })
                .WithMessage("El código de artículo es obligatorio cuando el tipo de documento es Artículo.");

            RuleFor(x => x.AcctCode)
                .NotEmpty()
                .When((line, context) =>
                {
                    var docType = context.RootContextData["DocType"]?.ToString();
                    return docType == "S";
                })
                .WithMessage("La cuenta contable es obligatoria cuando el tipo de documento es Servicio.");

            RuleFor(x => x.WhsCode)
                .NotEmpty()
                .When((line, context) =>
                {
                    var docType = context.RootContextData["DocType"]?.ToString();
                    return docType == "I";
                })
                .WithMessage("El almacén es obligatorio cuando el tipo de documento es Artículo.");

            RuleFor(x => x.U_tipoOpT12)
                .NotEmpty()
                .WithMessage("El tipo de operación es obligatorio.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .Length(3)
                .WithMessage("La moneda debe tener 3 caracteres.");

            RuleFor(x => x.UnitMsr)
                .NotEmpty()
                .When((line, context) =>
                {
                    var docType = context.RootContextData["DocType"]?.ToString();
                    return docType == "I";
                })
                .WithMessage("La unidad de medida es obligatoria. Por favor, complete en la ventana “Datos Maestros del Artículo”, en la pestaña “Datos de ventas”.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .When((line, context) =>
                {
                    var docType = context.RootContextData["DocType"]?.ToString();
                    return docType == "I";
                })
                .WithMessage("La cantidad debe ser mayor a cero.");

            RuleFor(x => x.PriceBefDi)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El precio no puede ser negativo.");

            RuleFor(x => x.DiscPrcnt)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El porcentaje de descuento no puede ser negativo.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El precio tras descuento no puede ser negativo.");

            RuleFor(x => x.TaxCode)
                .NotEmpty()
                .WithMessage("El código de impuesto es obligatorio.");

            RuleFor(x => x.LineTotal)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El total de la línea no puede ser negativo.");
        }
    }
}
