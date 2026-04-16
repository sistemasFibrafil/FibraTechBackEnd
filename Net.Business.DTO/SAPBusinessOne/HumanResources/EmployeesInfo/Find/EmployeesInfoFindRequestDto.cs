using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
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
