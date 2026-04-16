using System;
using Net.Business.Entities.SAPBusinessOne;
using System.ComponentModel.DataAnnotations;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OSKCCreateRequestDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_Number { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int U_SlpCode { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_Status { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.DateTime, ErrorMessage = "El formato de {0} no es válido.")]
        public DateTime U_DocDate { get; set; }
        public string U_ItemCodeBase { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int U_ItmsGrpCod { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_ItmsSGrpCod { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos.")]
        public string U_ItemName { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_CardCode { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_UnitMsrCode { get; set; }
        public decimal U_Quantity { get; set; }
        public decimal U_Wide { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public short U_UnitCode { get; set; }
        public decimal U_Long { get; set; }
        public decimal U_GrMtSq { get; set; }
        public decimal U_ItemWeight { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_ColorCode { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_Laminate { get; set; }
        public string U_LamTypCode { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_Linner { get; set; }
        public decimal U_LinnWeight { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_Print { get; set; }
        public string U_PrintColCode { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_Fuelle { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_UvByMonCode { get; set; }
        public decimal U_PrjMonVol { get; set; }
        public decimal U_Price { get; set; }
        public string U_Observations { get; set; }

        public OSKCEntity ReturnValue()
        {
            return new OSKCEntity
            {
                U_Number = U_Number,
                U_SlpCode = U_SlpCode,
                U_Status = U_Status,
                U_DocDate = U_DocDate,
                U_ItemCodeBase = U_ItemCodeBase,
                U_ItmsGrpCod = U_ItmsGrpCod,
                U_ItmsSGrpCod = U_ItmsSGrpCod,
                U_ItemName = U_ItemName,
                U_CardCode = U_CardCode,
                U_UnitMsrCode = U_UnitMsrCode,
                U_Quantity = U_Quantity,
                U_Wide = U_Wide,
                U_UnitCode = U_UnitCode,
                U_Long = U_Long,
                U_GrMtSq = U_GrMtSq,
                U_ItemWeight = U_ItemWeight,
                U_ColorCode = U_ColorCode,
                U_Laminate = U_Laminate,
                U_LamTypCode = U_LamTypCode,
                U_Linner = U_Linner,
                U_LinnWeight = U_LinnWeight,
                U_Print = U_Print,
                U_PrintColCode = U_PrintColCode,
                U_Fuelle = U_Fuelle,
                U_UvByMonCode = U_UvByMonCode,
                U_PrjMonVol = U_PrjMonVol,
                U_Price = U_Price,
                U_Observations = U_Observations
            };
        }
    }
}
