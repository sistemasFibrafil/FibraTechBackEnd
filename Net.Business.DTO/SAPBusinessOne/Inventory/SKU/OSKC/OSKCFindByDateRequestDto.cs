using System;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class OSKCFindByDateRequestDto
    {
        public DateTime StrDate { get; set; }
        public DateTime EndDate { get; set; }

        public OSKCEntity ReturnValue()
        {
            return new OSKCEntity
            {
                StrDate = StrDate,
                EndDate = EndDate,
            };
        }
    }
}
