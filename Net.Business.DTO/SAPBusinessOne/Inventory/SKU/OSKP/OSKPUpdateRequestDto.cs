using System;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne;
using System.ComponentModel.DataAnnotations;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OSKPUpdateRequestDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int DocEntry { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_Number { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime U_PrdStrDate { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime U_PrdEndDate { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime U_PrdEndHour { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public decimal U_RollWeight { get; set; }
        public string U_PrdForDetail { get; set; }
        public string U_PrdPresBale { get; set; }
        public string U_PrdFeaYes { get; set; }
        public string U_PrdFeaNo { get; set; }
        public string U_PrdFeaObs { get; set; }
        public string U_FeaQuaInd { get; set; }
        public string U_FeaQuaJus { get; set; }
        public DateTime? U_CosStrDate { get; set; } = null;
        public DateTime? U_CosEndDate { get; set; } = null;
        public DateTime? U_CosEndHour { get; set; } = null;
        public string U_CosDetail { get; set; }
        public string U_ValExcMar { get; set; }
        public string U_AprByExc { get; set; }
        public string U_Observations { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_ItemCode { get; set; }
        public List<SKP1UpdateDto> Line { get; set; } = new List<SKP1UpdateDto>();


        public OSKPEntity ReturnValue()
        {
            var value = new OSKPEntity()
            {
                DocEntry = DocEntry,
                U_Number = U_Number,
                U_PrdStrDate = U_PrdStrDate,
                U_PrdEndDate = U_PrdEndDate,
                U_PrdEndHour = U_PrdEndHour,
                U_RollWeight = U_RollWeight,
                U_PrdForDetail = U_PrdForDetail,
                U_PrdPresBale = U_PrdPresBale,
                U_PrdFeaYes = U_PrdFeaYes,
                U_PrdFeaNo = U_PrdFeaNo,
                U_PrdFeaObs = U_PrdFeaObs,
                U_FeaQuaInd = U_FeaQuaInd,
                U_FeaQuaJus = U_FeaQuaJus,
                U_CosStrDate = U_CosStrDate,
                U_CosEndDate = U_CosEndDate,
                U_CosEndHour = U_CosEndHour,
                U_CosDetail = U_CosDetail,
                U_ValExcMar = U_ValExcMar,
                U_AprByExc = U_AprByExc,
                U_Observations = U_Observations,
                U_ItemCode = U_ItemCode
            };

            foreach (var linea in Line)
            {
                value.Line.Add(new SKP1Entity()
                {
                    DocEntry = DocEntry,
                    LineId = linea.LineId,
                    U_ProcessCode = linea.U_ProcessCode,
                    U_Percentage1 = linea.U_Percentage1,
                    U_ItemCode = linea.U_ItemCode,
                    U_Percentage2 = linea.U_Percentage2
                });
            }

            return value;
        }
    }
    public class SKP1UpdateDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int LineId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_ProcessCode { get; set; }
        public decimal U_Percentage1 { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string U_ItemCode { get; set; }
        public decimal U_Percentage2 { get; set; }
    }
}
