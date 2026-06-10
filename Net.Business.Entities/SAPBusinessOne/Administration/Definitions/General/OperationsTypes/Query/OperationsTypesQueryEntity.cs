namespace Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.OperationsTypes.Query
{
    public class OperationsTypesQueryEntity
    {
        public string Code { get; set; } = string.Empty;
        public string? U_descrp { get; set; }
        public string FullDescr => $"{Code} - {U_descrp}";
    }
}
