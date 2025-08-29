using System;
using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class OSKCFindByDateRequestDto
    {
        public DateTime StrDate { get; set; }
        public DateTime EndDate { get; set; }

        public OSKCEntity ReturnValue()
        {
            return new OSKCEntity
            {
                StrDate = this.StrDate,
                EndDate = this.EndDate,
            };
        }
    }
}
