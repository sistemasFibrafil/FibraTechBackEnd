namespace Net.Business.DTO.SAPBusinessOne.BusinessPartners.ContactEmployees.Update
{
    public class ContactEmployeesUpdateRequestDto
    {
        public int CntctCode { get; set; }
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Title { get; set; }
        public string? Position { get; set; }
        public string? Address { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? MobilePhone { get; set; }
        public string? E_MailL { get; set; }
        public int Record { get; set; }
    }
}
