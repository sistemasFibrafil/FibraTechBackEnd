using System;
namespace Net.Business.Entities.Sap
{
    /// <summary>
    /// Grupo de Socios de Negocio
    /// </summary>
    public class BusinessPartnerGroupsEntity
    {
        public Int16 GroupCode { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }
    }
}
