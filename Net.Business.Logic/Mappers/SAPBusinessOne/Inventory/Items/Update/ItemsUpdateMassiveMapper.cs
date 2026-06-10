using Net.Business.DTO.SAPBusinessOne.Inventory.Items.Update;
using Net.Business.Entities.SAPBusinessOne.Inventory.Items.Update;
namespace Net.Business.Logic.Mappers.SAPBusinessOne.Inventory.Items.Update
{
    public class ItemsUpdateMassiveMapper
    {
        public static List<ItemsUpdateMassiveEntity> ToEntity(List<ItemsUpdateMassiveRequestDto> dto)
        {
            return [.. dto.Select(l => new ItemsUpdateMassiveEntity
            {
                ItemCode = l.ItemCode,
                ItemName = l.ItemName,
                ItmsGrpCod = l.ItmsGrpCod,
                U_BPP_TIPEXIST = l.U_BPP_TIPEXIST,
                U_BPP_TIPUNMED = l.U_BPP_TIPUNMED,
                U_S_PartAranc1 = l.U_S_PartAranc1,
                U_S_PartAranc2 = l.U_S_PartAranc2,
                U_FIB_ECU = l.U_FIB_ECU,
                U_S_CCosto = l.U_S_CCosto,
                U_FIB_PESO = l.U_FIB_PESO,
                U_FIB_SGRUP = l.U_FIB_SGRUP,
                U_FIB_SGRUPO2 = l.U_FIB_SGRUPO2,
                U_FIB_LINNEG = l.U_FIB_LINNEG,
                U_UsrUpdate = l.U_UsrUpdate
            })];
        }
    }
}
