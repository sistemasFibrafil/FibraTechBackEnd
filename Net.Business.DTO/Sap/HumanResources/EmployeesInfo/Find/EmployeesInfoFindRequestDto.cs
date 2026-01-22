using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class EmployeesInfoFindRequestDto
    {
        public int empID { get; set; }

        public EmployeesInfoEntity ReturnValue()
        {
            return new EmployeesInfoEntity
            {
                empID = empID
            };
        }
    }
}
