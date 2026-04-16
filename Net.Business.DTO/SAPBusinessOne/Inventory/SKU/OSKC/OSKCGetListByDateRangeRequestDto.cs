using System.Linq;
using System.Collections.Generic;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OSKCGetListByDateRangeRequestDto
    {
        public IEnumerable<OSKCDateRangeRequestDto> GetList { get; set; }
        public OSKCGetListByDateRangeRequestDto ReturnValue(IEnumerable<OSKCEntity> list)
        {
            IEnumerable<OSKCDateRangeRequestDto> responde = 
            
                from value in list
                select new OSKCDateRangeRequestDto
                {
                    Code = value.Code,
                    U_Number = value.U_Number,
                    U_SlpCode = value.U_SlpCode,
                    U_SlpName = value.U_SlpName,
                    U_Status = value.U_Status,
                    U_StatusName = value.U_StatusName,
                    U_DocDate = value.U_DocDate,
                    U_ItemCodeBase = value.U_ItemCodeBase,
                    U_ItemNameBase = value.U_ItemNameBase,
                    U_ItmsGrpCod = value.U_ItmsGrpCod,
                    U_ItmsGrpNam = value.U_ItmsGrpNam,
                    U_ItmsSGrpCod = value.U_ItmsSGrpCod,
                    U_ItmsSGrpNam = value.U_ItmsSGrpNam,
                    U_ItemName = value.U_ItemName,
                    U_CardCode = value.U_CardCode,
                    U_CardName = value.U_CardName,
                    U_UnitMsrCode = value.U_UnitMsrCode,
                    U_UnitMsrName = value.U_UnitMsrName,
                    U_Quantity = value.U_Quantity,
                    U_Wide = value.U_Wide,
                    U_Long = value.U_Long,
                    U_GrMtSq = value.U_GrMtSq,
                    U_ItemWeight = value.U_ItemWeight,
                    U_ColorCode = value.U_ColorCode,
                    U_ColorName = value.U_ColorName,
                    U_Laminate = value.U_Laminate,
                    U_LaminateName = value.U_LaminateName,
                    U_LamTypCode = value.U_LamTypCode,
                    U_LamTypName = value.U_LamTypName,
                    U_Linner = value.U_Linner,
                    U_LinnerName = value.U_LinnerName,
                    U_LinnWeight = value.U_LinnWeight,
                    U_Print = value.U_Print,
                    U_PrintName = value.U_PrintName,
                    U_PrintColCode = value.U_PrintColCode,
                    U_PrintColName = value.U_PrintColName,
                    U_UvByMonCode = value.U_UvByMonCode,
                    U_UvByMonName = value.U_UvByMonName,
                    U_PrjMonVol = value.U_PrjMonVol,
                    U_Price = value.U_Price,
                    U_Observations = value.U_Observations,
                }
            ;

            return new OSKCGetListByDateRangeRequestDto() { GetList = responde };
        }
    }
}
