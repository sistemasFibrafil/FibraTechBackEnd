using FluentValidation;
using Net.Business.DTO.SAPBusinessOne.Purchasing.PurchaseRequest.Validate;
namespace Net.Business.Logic.Validators.SAPBusinessOne.Purchasing.PurchaseRequest.Validate.Services
{
    public class PurchaseRequestLineServiceValidateRequestDtoValidator : AbstractValidator<PurchaseRequestLinesServicesValidateRequestDto>
    {
        public PurchaseRequestLineServiceValidateRequestDtoValidator()
        {
            RuleFor(x => x.Dscription)
                .NotEmpty()
                .WithMessage("La descripción del servicio es obligatoria.");

            RuleFor(x => x.PqtReqDate)
                .NotEmpty()
                .WithMessage("La fecha necesaria es obligatoria.");

            RuleFor(x => x.FormatCode)
                .NotEmpty()
                .WithMessage("La cuenta contable es obligatoria.");

            RuleFor(x => x.OcrCode)
                .NotEmpty()
                .WithMessage("El centro de costo es obligatorio.");

            RuleFor(x => x.U_tipoOpT12)
                .NotEmpty()
                .WithMessage("El código del tipo de operación es obligatorio.");

            RuleFor(x => x.U_FF_TIP_COM)
                .NotEmpty()
                .WithMessage("El código del tipo de compra es obligatorio.");
        }
    }
}
