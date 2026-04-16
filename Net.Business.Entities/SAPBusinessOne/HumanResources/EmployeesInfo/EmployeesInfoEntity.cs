namespace Net.Business.Entities.SAPBusinessOne
{
    /// <summary>
    /// Empleados
    /// </summary>
    public class EmployeesInfoEntity
    {
        public int empID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public short? dept { get; set; }
        public short? branch { get; set; }
        public string email { get; set; }
        public string Active { get; set; }
    }
}
