using System.ComponentModel.DataAnnotations.Schema;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class OperationTypeEntity
    {
        public string Code { get; set; } = string.Empty;
        public string? U_descrp { get; set; }
        [NotMapped]
        public string FullDescription => $"{Code} - {U_descrp}";
    }
}
