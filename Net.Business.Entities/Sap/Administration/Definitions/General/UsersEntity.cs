namespace Net.Business.Entities.Sap
{
    /// <summary>
    /// Usuarios
    /// </summary>
    public class UsersEntity
    {
        public short USERID { get; set; }
        public string USER_CODE { get; set; }
        public string U_NAME { get; set; }
        public short? GROUPS { get; set; }
        public short? Department { get; set; }
        public short? Branch { get; set; }
        public string E_Mail { get; set; }
    }
}
