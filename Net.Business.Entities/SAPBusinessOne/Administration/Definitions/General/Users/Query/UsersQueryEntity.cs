using System;
namespace Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Query
{
    public class UsersQueryEntity
    {
        public short UserId { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public short? Groups { get; set; }
        public short? Department { get; set; }
        public short? Branch { get; set; }
        public string? Email { get; set; }
    }
}
