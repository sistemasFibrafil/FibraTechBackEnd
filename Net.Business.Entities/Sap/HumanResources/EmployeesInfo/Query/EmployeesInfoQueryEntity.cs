namespace Net.Business.Entities.Sap
{
    public class EmployeesInfoQueryEntity
    {
        public int empID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string fullName { get; set; }
        public short? dept { get; set; }
        public short? branch { get; set; }
        public string email { get; set; }
    }
}
