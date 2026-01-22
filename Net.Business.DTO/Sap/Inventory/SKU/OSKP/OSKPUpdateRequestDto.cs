using System;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Net.Business.DTO.Sap
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
                DocEntry = this.DocEntry,
                U_Number = this.U_Number,
                U_PrdStrDate = this.U_PrdStrDate,
                U_PrdEndDate = this.U_PrdEndDate,
                U_PrdEndHour = this.U_PrdEndHour,
                U_RollWeight = this.U_RollWeight,
                U_PrdForDetail = this.U_PrdForDetail,
                U_PrdPresBale = this.U_PrdPresBale,
                U_PrdFeaYes = this.U_PrdFeaYes,
                U_PrdFeaNo = this.U_PrdFeaNo,
                U_PrdFeaObs = this.U_PrdFeaObs,
                U_FeaQuaInd = this.U_FeaQuaInd,
                U_FeaQuaJus = this.U_FeaQuaJus,
                U_CosStrDate = this.U_CosStrDate,
                U_CosEndDate = this.U_CosEndDate,
                U_CosEndHour = this.U_CosEndHour,
                U_CosDetail = this.U_CosDetail,
                U_ValExcMar = this.U_ValExcMar,
                U_AprByExc = this.U_AprByExc,
                U_Observations = this.U_Observations,
                U_ItemCode = this.U_ItemCode
            };

            foreach (var linea in Line)
            {
                value.Line.Add(new SKP1Entity()
                {
                    DocEntry = this.DocEntry,
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
