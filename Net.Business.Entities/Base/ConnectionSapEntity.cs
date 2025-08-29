namespace Net.Business.Entities
{
    public class ConnectionSapEntity
    {
        /// <summary>
        /// SQL: SERVER
        /// </summary>
        public string Server { get; set; }
        public string LicenseServer { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
        public string DbServerType { get; set; }

        /// <summary>
        /// SQL: SAP
        /// </summary>
        public string CompanyDB { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
